using System.ComponentModel.DataAnnotations;

namespace Employee_Attendance_Tracker.Models.Entities
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeCode { get; set; }

        [Required]
        [RegularExpression(@"^([A-Za-z]{2,}\s){3}[A-Za-z]{2,}$")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public virtual Department Department { get; set; }
        public virtual ICollection<Attendance> Attendances { get; set; }
    }
}
