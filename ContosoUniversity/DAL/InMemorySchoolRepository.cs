using ContosoUniversity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;

namespace ContosoUniversity.DAL
{
    public class InMemorySchoolRepository : ISchoolRepository
    {
        private List<Student> _students;
        private List<Course> _courses;
        private List<Enrollment> _enrollments;
        private List<Department> _departments;
        private List<Instructor> _instructors;
        private List<CourseAssignment> _courseAssignments;
        private List<OfficeAssignment> _officeAssignments;

        public InMemorySchoolRepository()
        {
            // -----------------------------
            // 1. Students
            // -----------------------------

            _students = new List<Student>
        {
            new Student { ID = 1, FirstMidName = "Carson",   LastName = "Alexander", EnrollmentDate = DateTime.Now },
            new Student { ID = 2, FirstMidName = "Meredith", LastName = "Alonso",    EnrollmentDate = DateTime.Now },
            new Student { ID = 3, FirstMidName = "Arturo",   LastName = "Anand",     EnrollmentDate = DateTime.Parse("2021-09-01") },
            new Student { ID = 4, FirstMidName = "Gytis",    LastName = "Barzdukas", EnrollmentDate = DateTime.Parse("2022-09-01") },
            new Student { ID = 5, FirstMidName = "Yan",      LastName = "Li",        EnrollmentDate = DateTime.Parse("2022-09-01") },
            new Student { ID = 6, FirstMidName = "Peggy",    LastName = "Justice",   EnrollmentDate = DateTime.Parse("2023-09-01") },
            new Student { ID = 7, FirstMidName = "Laura",    LastName = "Norman",     EnrollmentDate = DateTime.Parse("2023-09-01") },
            new Student { ID = 8, FirstMidName = "Nino",     LastName = "Olivetto",   EnrollmentDate = DateTime.Parse("2024-09-01") }
        };

            // -----------------------------
            // 2. Courses
            // -----------------------------

            _courses = new List<Course>
        {
            new Course { CourseID = 1050, Title = "Chemistry", Credits = 3 },
            new Course { CourseID = 4022, Title = "Microeconomics", Credits = 3 },
            new Course { CourseID = 4041, Title = "Macroeconomics", Credits = 3 },
            new Course { CourseID = 1045, Title = "Calculus",       Credits = 4 },
            new Course { CourseID = 3141, Title = "Trigonometry",   Credits = 4 },
            new Course { CourseID = 2021, Title = "Composition",    Credits = 3 },
            new Course { CourseID = 2042, Title = "Literature",     Credits = 4 }
        };

            // -----------------------------
            // 3. Enrollments
            // -----------------------------

            _enrollments = new List<Enrollment>
        {
            new Enrollment { EnrollmentID = 1, StudentID = 1, CourseID = 1050, Grade = Grade.A },
            new Enrollment { StudentID = 1, CourseID = 1050, Grade = Grade.A },
            new Enrollment { StudentID = 1, CourseID = 4022, Grade = Grade.C },
            new Enrollment { StudentID = 1, CourseID = 4041, Grade = Grade.B },
            new Enrollment { StudentID = 2, CourseID = 1045, Grade = Grade.B },
            new Enrollment { StudentID = 2, CourseID = 3141, Grade = Grade.F },
            new Enrollment { StudentID = 2, CourseID = 2021, Grade = Grade.F },

            new Enrollment { StudentID = 3, CourseID = 1050 },
            new Enrollment { StudentID = 4, CourseID = 1050 },
            new Enrollment { StudentID = 4, CourseID = 4022, Grade = Grade.F },
            new Enrollment { StudentID = 5, CourseID = 4041, Grade = Grade.C },
            new Enrollment { StudentID = 6, CourseID = 2021 },
            new Enrollment { StudentID = 7, CourseID = 2042, Grade = Grade.A }
        };

            // -----------------------------
            // 4. Instructors
            // ----------------------------

            _instructors = new List<Instructor>
        {
            new Instructor { FirstMidName = "Kim",    LastName = "Abercrombie", HireDate = DateTime.Parse("2015-03-11") },
            new Instructor { FirstMidName = "Fadi",   LastName = "Fakhouri",    HireDate = DateTime.Parse("2016-07-06") },
            new Instructor { FirstMidName = "Roger",  LastName = "Harui",       HireDate = DateTime.Parse("2014-07-01") },
            new Instructor { FirstMidName = "Candace",LastName = "Kapoor",      HireDate = DateTime.Parse("2017-01-15") },
            new Instructor { FirstMidName = "Roger",  LastName = "Zheng",       HireDate = DateTime.Parse("2018-02-12") }
        };

            // -----------------------------
            // 5. Departments
            // -----------------------------

            _departments = new List<Department>
        {
            new Department { Name = "English",     Budget = 350000, StartDate = DateTime.Parse("2019-09-01") },
            new Department { Name = "Mathematics", Budget = 100000, StartDate = DateTime.Parse("2019-09-01") },
            new Department { Name = "Engineering", Budget = 350000, StartDate = DateTime.Parse("2019-09-01") },
            new Department { Name = "Economics",   Budget = 100000, StartDate = DateTime.Parse("2019-09-01") }
        };

            // -----------------------------
            // 6. CourseAssignments
            // -----------------------------

            _courseAssignments = new List<CourseAssignment>
        {
            new CourseAssignment { CourseID = 1050, InstructorID = 1 },
            new CourseAssignment { CourseID = 1050, InstructorID = 2 },
            new CourseAssignment { CourseID = 4022, InstructorID = 2 },
            new CourseAssignment { CourseID = 4041, InstructorID = 3 },
            new CourseAssignment { CourseID = 1045, InstructorID = 4 },
            new CourseAssignment { CourseID = 3141, InstructorID = 4 },
            new CourseAssignment { CourseID = 2021, InstructorID = 5 },
            new CourseAssignment { CourseID = 2042, InstructorID = 5 }
        };

            _officeAssignments = new List<OfficeAssignment>
            {
                new OfficeAssignment { InstructorID = 1, Location = "Smith 17" },
                new OfficeAssignment { InstructorID = 2, Location = "Gowan 27" },
                new OfficeAssignment { InstructorID = 3, Location = "Thompson 304" }
            };

        }

        public IQueryable<Student> Students => _students.AsQueryable();
        public IQueryable<Course> Courses => _courses.AsQueryable();
        public IQueryable<Enrollment> Enrollments => _enrollments.AsQueryable();
        public IQueryable<Instructor> Instructors => _instructors.AsQueryable();
        public IQueryable<Department> Departments => _departments.AsQueryable();
        public IQueryable<CourseAssignment> CourseAssignments => _courseAssignments.AsQueryable();

        // ISchoolRepository / IDisposable implementations for in-memory store

        public int SaveChanges()
        {
            // in-memory store: nothing to persist
            return 0;
        }

        public Task<int> SaveChangesAsync()
        {
            return Task.FromResult(0);
        }

        public void AddDepartment(Department department)
        {
            if (department == null) throw new ArgumentNullException(nameof(department));
            int nextId = 1;
            if (_departments.Any(d => d.DepartmentID != 0))
            {
                nextId = _departments.Max(d => d.DepartmentID) + 1;
            }
            else if (_departments.Count > 0)
            {
                // if existing DepartmentID values are default (0), still assign sequential ids starting at 1
                nextId = _departments.Count + 1;
            }
            department.DepartmentID = nextId;
            _departments.Add(department);
        }

        public void DeleteInstructor(Instructor instructor)
        {
            if (instructor == null) return;
            _instructors.Remove(instructor);
            // Optionally remove course assignments referencing this instructor's id if Instructor has an ID property.
            // Keep removal conservative to avoid assumptions about Instructor shape.
        }

        public DbEntityEntry Entry(object entity)
        {
            // Not supported for the in-memory repository; implemented to satisfy the interface.
            throw new NotSupportedException("DbEntityEntry is not supported by the in-memory repository.");
        }

        public void Dispose()
        {
            // nothing to dispose for in-memory collections
        }
    }

}