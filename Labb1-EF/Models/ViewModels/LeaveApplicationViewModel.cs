using System.ComponentModel.DataAnnotations;

namespace Labb1_EF.Models.ViewModels
{
    public class LeaveApplicationViewModel
    {
        public int EmployeeId { get; set; }
        public Employee? employee { get; set; }
        public int ApplicationId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Leave Type")]
        public LeaveType LeaveType { get; set; }


        [Required]
        [Display(Name = "Apply Date")]
        public DateTime AppliedDate { get; set; }
        
        [Required]
        [Display(Name = "Application Status")]
        public ApplicationStatus ApplicationStatus { get; set; }
    }

}
