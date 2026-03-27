using System;
using System.Collections.Generic;

namespace ContosoUniversity.Models
{
    public class Instructor
    {
        public static int InstructorID { get; internal set; }
    
        public int ID { get; set; }
        public DateTime HireDate { get; set; }
        public string FirstMidName { get; set; }
        public string LastName { get; set; }

        // Singular navigation property expected by the controller
        public virtual OfficeAssignment OfficeAssignment { get; set; }

        // Keep the plural collection as well in case other parts of the app rely on it
        public virtual ICollection<OfficeAssignment> OfficeAssignments { get; set; } = new List<OfficeAssignment>();

        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
        public virtual ICollection<CourseAssignment> CourseAssignments { get; set; } = new List<CourseAssignment>();

        public string FullName => string.IsNullOrEmpty(FirstMidName) && string.IsNullOrEmpty(LastName)
            ? string.Empty
            : $"{LastName}, {FirstMidName}";
    }
}