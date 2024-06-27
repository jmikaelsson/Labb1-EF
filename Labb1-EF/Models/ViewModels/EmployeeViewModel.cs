using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Labb1_EF.Models.ViewModels
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
           
        public string? FirstName { get; set; }
                
        public string? LastName { get; set; }
                
        public DateTime BirthDate { get; set; }

        public List<LeaveApplicationViewModel>? LeaveApplications { get; set; }
    }
}
