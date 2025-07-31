using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Employee_Attendance_Tracker.Models.ViewModels
{
    public class AttendanceViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Employee is required")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public bool IsPresent { get; set; }

        public string? EmployeeName { get; set; }
        public string? DepartmentName { get; set; }

        // For dropdowns
        public SelectList? Employees { get; set; }
    }
}
