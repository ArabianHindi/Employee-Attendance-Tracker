using Microsoft.AspNetCore.Mvc.Rendering;

namespace Employee_Attendance_Tracker.Models.ViewModels
{
    public class AttendanceFilterViewModel
    {
        public int? DepartmentId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SearchTerm { get; set; }

        public SelectList Departments { get; set; }
        public SelectList Employees { get; set; }

        public List<AttendanceViewModel> FilteredAttendances { get; set; } = new List<AttendanceViewModel>();

        // Pagination properties
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    }
}
