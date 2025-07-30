using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Employee_Attendance_Tracker.Models.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Employee Code")]
        public int EmployeeCode { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        [RegularExpression(@"^([A-Za-z]{2,}\s){3}[A-Za-z]{2,}$",
            ErrorMessage = "Full name must contain exactly 4 names, each at least 2 characters")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public decimal AttendancePercentage { get; set; }

        public SelectList Departments { get; set; }
    }
}
