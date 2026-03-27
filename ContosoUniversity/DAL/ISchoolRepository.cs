using System;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using ContosoUniversity.Models;

namespace ContosoUniversity.DAL
{
    public interface ISchoolRepository : IDisposable
    {
        IQueryable<Student> Students { get; }
        IQueryable<Course> Courses { get; }
        IQueryable<Enrollment> Enrollments { get; }
        IQueryable<Department> Departments { get; }
        IQueryable<CourseAssignment> CourseAssignments { get; }
        IQueryable<Instructor> Instructors { get; }

        // Synchronous save (existing)
        int SaveChanges();

        // Async save for controllers that use async
        Task<int> SaveChangesAsync();

        // Add helpers for entities that are not writable via IQueryable
        void AddDepartment(Department department);

        // Add repository method to remove an instructor (implemented by concrete repositories)
        void DeleteInstructor(Instructor instructor);

        // Expose EF entry API so callers (controllers) can set original values or states
        // Implementations should return the underlying DbEntityEntry for the given entity.
        DbEntityEntry Entry(object entity);
    }
}