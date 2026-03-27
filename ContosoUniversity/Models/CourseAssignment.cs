using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ContosoUniversity.Models
{
    public class CourseAssignment
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Instructor")]
        public int InstructorID { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Course")]
        public int CourseID { get; set; }

        // Navigation properties
        //public virtual ICollection<Instructor> Instructors { get; set; }
        //public virtual ICollection<Course> Courses { get; set; }
        // Navigation properties MUST appear after FK properties
        public virtual Instructor Instructor { get; set; }
        public virtual Course Course { get; set; }


    }
}