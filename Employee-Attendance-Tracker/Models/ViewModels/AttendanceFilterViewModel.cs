using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Employee_Attendance_Tracker.Models.ViewModels
{
    public class AttendanceFilterViewModel
    {
        public int? DepartmentId { get; set; }
        public int? EmployeeId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public SelectList? Departments { get; set; }
        public SelectList? Employees { get; set; }

        public List<AttendanceViewModel> FilteredAttendances { get; set; } = new();
    }
}
