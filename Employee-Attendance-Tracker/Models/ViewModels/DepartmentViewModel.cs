using System.ComponentModel.DataAnnotations;

namespace Employee_Attendance_Tracker.Models.ViewModels
{
    public class DepartmentViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Department Name")]
        public string Name { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 4)]
        [RegularExpression(@"^[A-Z]{4}$", ErrorMessage = "Code must be exactly 4 uppercase letters")]
        [Display(Name = "Department Code")]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Location { get; set; }

        [Display(Name = "Employee Count")]
        public int EmployeeCount { get; set; }
    }
}
