using Labb1_EF.Data;
using Labb1_EF.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Labb1_EF.Utility
{
    public static class DbInitializer
    {
        private static readonly Random _random = new();

        public static async Task Initialize(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            if (context.Employees.Any())
            {
                return;   // Database has already been seeded
            }

            // Define roles
            string[] roleNames = { "Employee", "Chef" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Create and add users to roles
            var users = new IdentityUser[]
                {
                    new IdentityUser { UserName = "emma.andersson@technova.com", Email = "emma.andersson@technova.com", EmailConfirmed = true },
                    new IdentityUser { UserName = "johan.lindberg@technova.com", Email = "johan.lindberg@technova.com", EmailConfirmed = true },
                    new IdentityUser { UserName = "anna.karlsson@technova.com", Email = "anna.karlsson@technova.com", EmailConfirmed = true },
                    new IdentityUser { UserName = "erik.nilsson@technova.com", Email = "erik.nilsson@technova.com", EmailConfirmed = true },
                    new IdentityUser { UserName = "maria.gustafsson@technova.com", Email = "maria.gustafsson@technova.com", EmailConfirmed = true },
                    new IdentityUser { UserName = "amanda.eriksson@technova.com", Email = "amanda.eriksson@technova.com", EmailConfirmed = true },
                    new IdentityUser { UserName = "sara.persson@technova.com", Email = "sara.persson@technova.com", EmailConfirmed = true },
                    new IdentityUser { UserName = "mattias.berg@technova.com", Email = "mattias.berg@technova.com", EmailConfirmed = true },
                    new IdentityUser { UserName = "linnea.larsson@technova.com", Email = "linnea.larsson@technova.com", EmailConfirmed = true },
                    new IdentityUser { UserName = "daniel.svensson@technova.com", Email = "daniel.svensson@technova.com", EmailConfirmed = true },
                    new IdentityUser { UserName = "hanna.olofsson@technova.com", Email = "hanna.olofsson@technova.com", EmailConfirmed = true },
                    new IdentityUser { UserName = "peter.johansson@technova.com", Email = "peter.johansson@technova.com", EmailConfirmed = true },
                    new IdentityUser { UserName = "jonas.pettersson@technova.com", Email = "jonas.pettersson@technova.com", EmailConfirmed = true },
                    new IdentityUser { UserName = "magnus.bjork@technova.com", Email = "magnus.bjork@technova.com", EmailConfirmed = true },
                    new IdentityUser { UserName = "ida.eklund@technova.com", Email = "ida.eklund@technova.com", EmailConfirmed = true }
                };


            foreach (var user in users)
            {
                var result = await userManager.CreateAsync(user, "Password123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Employee");
                }
            }

            var managers = new IdentityUser[]
            {
                new IdentityUser { UserName = "josefin@mikaelsson.nu", Email = "josefin@mikaelsson.nu", EmailConfirmed = true }
                // Add more managers as needed
            };

            foreach (var manager in managers)
            {
                var result = await userManager.CreateAsync(manager, "Password123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(manager, "Chef");
                }
            }

            // Save changes to get user IDs
            await context.SaveChangesAsync();

            // Create employees linked to the users
            var employees = new List<Employee>();
            foreach (var user in users)
            {
                employees.Add(new Employee
                {
                    ApplicationUserId = user.Id,
                    FirstName = user.UserName.Split('@')[0].Split('.')[0], // Extract first name from email
                    LastName = user.UserName.Split('@')[0].Split('.').Length > 1 ? user.UserName.Split('@')[0].Split('.')[1] : "Lastname",
                    BirthDate = new DateTime(_random.Next(1960, 2000), _random.Next(1, 12), _random.Next(1, 28)), // Example birth date
                    Email = user.Email
                });
            }

            foreach (var manager in managers)
            {
                employees.Add(new Employee
                {
                    ApplicationUserId = manager.Id,
                    FirstName = manager.UserName.Split('@')[0].Split('.')[0], // Extract first name from email
                    LastName = manager.UserName.Split('@')[0].Split('.').Length > 1 ? manager.UserName.Split('@')[0].Split('.')[1] : "Lastname",
                    BirthDate = new DateTime(_random.Next(1960, 2000), _random.Next(1, 12), _random.Next(1, 28)), // Example birth date
                    Email = manager.Email
                });
            }

            await context.Employees.AddRangeAsync(employees);
            await context.SaveChangesAsync();

            // Create leave applications for some of the employees
            var leaveApplications = new List<LeaveApplication>();
            for (int i = 0; i < 25; i++) // Create 25 leave applications
            {
                var applicationDates = GenerateRandomDates();
                var randomEmployee = employees[_random.Next(employees.Count)]; // Select a random employee
                var randomStatus = (ApplicationStatus)_random.Next(Enum.GetValues(typeof(ApplicationStatus)).Length); // Randomly choose an application status

                leaveApplications.Add(new LeaveApplication
                {
                    StartDate = applicationDates.startDate,
                    EndDate = applicationDates.endDate,
                    AppliedDate = applicationDates.appliedDate,
                    LeaveType = (LeaveType)_random.Next(Enum.GetValues(typeof(LeaveType)).Length),
                    ApplicationStatus = randomStatus,
                    FkEmployeeId = randomEmployee.EmployeeId
                });
            }

            await context.LeaveApplications.AddRangeAsync(leaveApplications);
            await context.SaveChangesAsync();
        }

        private static (DateTime appliedDate, DateTime startDate, DateTime endDate) GenerateRandomDates()
        {
            DateTime appliedDate = DateTime.Now.AddDays(-_random.Next(1, 120));
            DateTime startDate = appliedDate.AddDays(_random.Next(1, 30));
            DateTime endDate = startDate.AddDays(_random.Next(1, 90));

            return (appliedDate, startDate, endDate);
        }
    }
}