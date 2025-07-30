using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Employee_Attendance_Tracker.Models.ViewModels
{
    public class AttendanceViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [Display(Name = "Status")]
        public bool IsPresent { get; set; }

        public string EmployeeName { get; set; }
        public string DepartmentName { get; set; }

        public SelectList Employees { get; set; }
        public SelectList Departments { get; set; }
    }
}
