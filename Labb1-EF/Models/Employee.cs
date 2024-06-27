using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Labb1_EF.Models
{
    public class Employee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Förnamn måste anges")]
        [StringLength(30, ErrorMessage = "Namnet får inte vara längre än 30 tecken")]
        [Display(Name = "Förnamn")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Efternamn måste anges")]
        [StringLength(30, ErrorMessage = "Namnet får inte vara längre än 30 tecken")]
        [Display(Name = "Efternamn")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Födelsedata är obligatoriskt att ange")]
        [Display(Name = "Födelsedata")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "E-post är obligatoriskt att ange")]
        [Display(Name = "E-post")]
        public string Email { get; set; }

        public string ApplicationUserId { get; set; }


        // Navigation property for related entities
        public ICollection<LeaveApplication> LeaveApplications { get; set; }
    }
}
