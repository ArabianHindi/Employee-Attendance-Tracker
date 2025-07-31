using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee_Attendance_Tracker.Models.Entities
{
    public class Attendance
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public bool IsPresent { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; } = null!;
    }
}
