using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Labb1_EF.Data;
using Labb1_EF.Models;
using Labb1_EF.Models.ViewModels;
using Labb1_EF.Utility;

namespace Labb1_EF.Controllers
{
    [Authorize]
    public class LeaveApplicationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public LeaveApplicationController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: LeaveApplication
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["DateSortParm"] = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return NotFound(); // Or any appropriate action if user is not found
            }

            IQueryable<LeaveApplication> leaveApplicationsQuery = _context.LeaveApplications.Include(l => l.Employee);

            switch (sortOrder)
            {
                case "date_desc":
                    leaveApplicationsQuery = leaveApplicationsQuery.OrderByDescending(l => l.AppliedDate);
                    break;
                default:
                    leaveApplicationsQuery = leaveApplicationsQuery.OrderBy(l => l.AppliedDate);
                    break;
            }

            List<LeaveApplication> leaveApplications;

                leaveApplications = await leaveApplicationsQuery
                    .Where(l => l.Employee.ApplicationUserId == currentUser.Id).ToListAsync();


            var leaveApplicationViewModels = leaveApplications.Select(la => new LeaveApplicationViewModel
            {
                EmployeeId = la.FkEmployeeId,
                employee = la.Employee,
                StartDate = la.StartDate,
                EndDate = la.EndDate,
                LeaveType = la.LeaveType,
                AppliedDate = la.AppliedDate,
                ApplicationId = la.ApplicationId, // Assuming you have an Id property in LeaveApplication
                ApplicationStatus = la.ApplicationStatus // Assuming you have an ApplicationStatus property
            }).ToList();

            return View(leaveApplicationViewModels);
        }
        // GET: LeaveApplication
        public async Task<IActionResult> ManagerIndex(string sortOrder)
        {
            ViewData["DateSortParm"] = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return NotFound(); // Or any appropriate action if user is not found
            }

            IQueryable<LeaveApplication> leaveApplicationsQuery = _context.LeaveApplications.Include(l => l.Employee);

            switch (sortOrder)
            {
                case "date_desc":
                    leaveApplicationsQuery = leaveApplicationsQuery.OrderByDescending(l => l.AppliedDate);
                    break;
                default:
                    leaveApplicationsQuery = leaveApplicationsQuery.OrderBy(l => l.AppliedDate);
                    break;
            }

            List<LeaveApplication> leaveApplications;
            if (User.IsInRole(SD.Role_Manager))
            {
                leaveApplications = await leaveApplicationsQuery.ToListAsync();
            }
            else
            {
                return Forbid(); // If user doesn't have a valid role
            }

            var leaveApplicationViewModels = leaveApplications.Select(la => new LeaveApplicationViewModel
            {
                EmployeeId = la.FkEmployeeId,
                employee = la.Employee,
                StartDate = la.StartDate,
                EndDate = la.EndDate,
                LeaveType = la.LeaveType,
                AppliedDate = la.AppliedDate,
                ApplicationId = la.ApplicationId, // Assuming you have an Id property in LeaveApplication
                ApplicationStatus = la.ApplicationStatus // Assuming you have an ApplicationStatus property
            }).ToList();

            return View(leaveApplicationViewModels);
        }

        // GET: LeaveApplication/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplications
                .FirstOrDefaultAsync(m => m.ApplicationId == id);
            if (leaveApplication == null)
            {
                return NotFound();
            }
            return View(leaveApplication);
        }

        // GET: LeaveApplications/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name");
            return View();
        }
        // POST: LeaveApplications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,StartDate,EndDate,LeaveType,AppliedDate,ApplicationStatus")] LeaveApplicationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound(); // Eller annan lämplig åtgärd om användaren inte hittas
                }

                var employee = await _context.Employees.SingleOrDefaultAsync(e => e.ApplicationUserId == user.Id);
                if (employee == null)
                {
                    // Hantera fallet där Employee inte hittas
                    return NotFound(); // Eller returnera en annan vy eller åtgärd
                }

                var leaveApplication = new LeaveApplication
                {
                    FkEmployeeId = employee.EmployeeId,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,
                    LeaveType = viewModel.LeaveType,
                    AppliedDate = DateTime.Now,
                    ApplicationStatus = ApplicationStatus.Skapad
                };

                _context.Add(leaveApplication);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", viewModel.EmployeeId);
            return View(viewModel);
        }



        // GET: LeaveApplication/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplications
                .Include(l => l.Employee)
                .FirstOrDefaultAsync(m => m.ApplicationId == id);

            if (leaveApplication == null)
            {
                return NotFound();
            }

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName", leaveApplication.FkEmployeeId);
            return View(leaveApplication);
        }

        // POST: LeaveApplication/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ApplicationId,FkEmployeeId,StartDate,EndDate,LeaveType,ApplicationStatus, AppliedDate")] LeaveApplication leaveApplication)
        {
            if (id != leaveApplication.ApplicationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var employee = await _context.Employees.FindAsync(leaveApplication.FkEmployeeId);
                    if (employee == null)
                    {
                        ModelState.AddModelError("FkEmployeeId", "Ogiltig anställd.");
                        ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName", leaveApplication.FkEmployeeId);
                        return View(leaveApplication);
                    }

                    _context.Update(leaveApplication);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveApplicationExists(leaveApplication.ApplicationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "FullName", leaveApplication.FkEmployeeId);
            return View(leaveApplication);
        }




        // GET: LeaveApplication/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveApplication = await _context.LeaveApplications
                .FirstOrDefaultAsync(m => m.ApplicationId == id);
            if (leaveApplication == null)
            {
                return NotFound();
            }

            return View(leaveApplication);
        }

        // POST: LeaveApplication/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leaveApplication = await _context.LeaveApplications.FindAsync(id);
            _context.LeaveApplications.Remove(leaveApplication);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: LeaveApplication/LastMonth
        public async Task<IActionResult> LastMonth(string sortOrder)
        {
            ViewData["DateSortParm"] = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                return NotFound(); // Or any appropriate action if user is not found
            }

            var oneMonthAgo = DateTime.Now.AddMonths(-1);
            IQueryable<LeaveApplication> leaveApplicationsQuery = _context.LeaveApplications
                .Include(l => l.Employee)
                .Where(l => l.AppliedDate >= oneMonthAgo); // Filter to get applications from last month only

            switch (sortOrder)
            {
                case "date_desc":
                    leaveApplicationsQuery = leaveApplicationsQuery.OrderByDescending(l => l.AppliedDate);
                    break;
                default:
                    leaveApplicationsQuery = leaveApplicationsQuery.OrderBy(l => l.AppliedDate);
                    break;
            }

            List<LeaveApplication> leaveApplications;
            if (User.IsInRole(SD.Role_Manager))
            {
                leaveApplications = await leaveApplicationsQuery.ToListAsync();
            }
            else if (User.IsInRole(SD.Role_Employee))
            {
                leaveApplications = await leaveApplicationsQuery
                    .Where(l => l.Employee.ApplicationUserId == currentUser.Id).ToListAsync();
            }
            else
            {
                return Forbid(); // If user doesn't have a valid role
            }

            var leaveApplicationViewModels = leaveApplications.Select(la => new LeaveApplicationViewModel
            {
                EmployeeId = la.FkEmployeeId,
                StartDate = la.StartDate,
                EndDate = la.EndDate,
                LeaveType = la.LeaveType,
                AppliedDate = la.AppliedDate
            }).ToList();

            return PartialView("_LastMonth", leaveApplicationViewModels);
        }

        private bool LeaveApplicationExists(int id)
        {
            return _context.LeaveApplications.Any(e => e.ApplicationId == id);
        }
    }
}
