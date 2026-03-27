using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models
{
    public class Course
    {
       
        [Key]
        public int CourseID { get; set; }

        [Required, StringLength(100)]
        public string Title { get; set; }

        public int Credits { get; set; }
        public virtual Department Department { get; set; }

        public virtual ICollection<Instructor> Instructors { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        public virtual ICollection<CourseAssignment> CourseAssignments { get; set; }

        public int DepartmentID { get; set; }

    }
}