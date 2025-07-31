using Employee_Attendance_Tracker.Models.Entities;

namespace Employee_Attendance_Tracker.Data
{
    public class DataSeeder
    {
        public static void SeedData(ApplicationDbContext context)
        {
            // Ensure database is created
            context.Database.EnsureCreated();

            // Check if data already exists
            if (context.Departments.Any())
            {
                return;
            }

            // Seed Departments
            var departments = new List<Department>
            {
                new Department
                {
                    Name = "Human Resources",
                    Code = "HRMG",
                    Location = "Building A, Floor 2"
                },
                new Department
                {
                    Name = "Information Technology",
                    Code = "TECH",
                    Location = "Building B, Floor 3"
                },
                new Department
                {
                    Name = "Finance",
                    Code = "FINC",
                    Location = "Building A, Floor 1"
                },
                new Department
                {
                    Name = "Marketing",
                    Code = "MKTG",
                    Location = "Building C, Floor 2"
                },
                new Department
                {
                    Name = "Operations",
                    Code = "OPER",
                    Location = "Building B, Floor 1"
                }
            };

            context.Departments.AddRange(departments);
            context.SaveChanges();

            // Seed Employees
            var employees = new List<Employee>
            {
                new Employee
                {
                    EmployeeCode = 1,
                    FullName = "Ahmed Mohamed Ali Hassan",
                    Email = "ahmed.hassan@company.com",
                    DepartmentId = departments.First(d => d.Code == "TECH").Id
                },
                new Employee
                {
                    EmployeeCode = 2,
                    FullName = "Fatma Mahmoud Ibrahim Salem",
                    Email = "fatma.salem@company.com",
                    DepartmentId = departments.First(d => d.Code == "HRMG").Id
                },
                new Employee
                {
                    EmployeeCode = 3,
                    FullName = "Mohamed Khaled Ahmed Fouad",
                    Email = "mohamed.fouad@company.com",
                    DepartmentId = departments.First(d => d.Code == "TECH").Id
                },
                new Employee
                {
                    EmployeeCode = 4,
                    FullName = "Noha Essam Mohamed Rashad",
                    Email = "noha.rashad@company.com",
                    DepartmentId = departments.First(d => d.Code == "FINC").Id
                },
                new Employee
                {
                    EmployeeCode = 5,
                    FullName = "Omar Yasser Ali Mansour",
                    Email = "omar.mansour@company.com",
                    DepartmentId = departments.First(d => d.Code == "MKTG").Id
                },
                new Employee
                {
                    EmployeeCode = 6,
                    FullName = "Sara Ahmed Mostafa Saleh",
                    Email = "sara.saleh@company.com",
                    DepartmentId = departments.First(d => d.Code == "OPER").Id
                },
                new Employee
                {
                    EmployeeCode = 7,
                    FullName = "Karim Hassan Mohamed Zaki",
                    Email = "karim.zaki@company.com",
                    DepartmentId = departments.First(d => d.Code == "TECH").Id
                },
                new Employee
                {
                    EmployeeCode = 8,
                    FullName = "Mona Adel Ibrahim Farouk",
                    Email = "mona.farouk@company.com",
                    DepartmentId = departments.First(d => d.Code == "HRMG").Id
                }
            };

            context.Employees.AddRange(employees);
            context.SaveChanges();

            // Seed Attendance Records for the current month
            var attendanceRecords = new List<Attendance>();
            var currentDate = DateTime.Now.Date;
            var startOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var random = new Random();

            // Generate attendance for each employee for the past 15 days
            foreach (var employee in employees)
            {
                for (int i = 0; i < 15; i++)
                {
                    var attendanceDate = startOfMonth.AddDays(i);

                    // Skip weekends (Saturday = 6, Sunday = 0)
                    if (attendanceDate.DayOfWeek == DayOfWeek.Friday || attendanceDate.DayOfWeek == DayOfWeek.Saturday)
                        continue;

                    // Skip future dates
                    if (attendanceDate > currentDate)
                        continue;

                    // Generate random attendance (85% chance of being present)
                    bool isPresent = random.NextDouble() < 0.85;

                    attendanceRecords.Add(new Attendance
                    {
                        EmployeeId = employee.Id,
                        Date = attendanceDate,
                        IsPresent = isPresent
                    });
                }
            }

            context.Attendances.AddRange(attendanceRecords);
            context.SaveChanges();
        }
    }
}
