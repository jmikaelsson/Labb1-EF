using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Labb1_EF.Models
{
    public class LeaveApplication
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApplicationId { get; set; }

        [Required]
        [ForeignKey("Employee")]
        public int FkEmployeeId { get; set; }

        public Employee? Employee { get; set; }

        [Required(ErrorMessage = "Startdatum för ledighet måste anges")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Slutdatum för ledighet måste anges")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Ledighetstyp måste anges")]
        public LeaveType LeaveType { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AppliedDate { get; set; }

        public ApplicationStatus ApplicationStatus { get; set; }
    }

    public enum LeaveType
    {
        Semester,
        Tjänstledig,
        Föräldraledig,
        VAB
    }

    public enum ApplicationStatus
    {
        Skapad,
        Godkänd,
        Nekad
    }
}
