using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using ContosoUniversity.ViewModels;

namespace ContosoUniversity.Controllers
{
    public class InstructorController : Controller
    {
        private readonly ISchoolRepository repo = new InMemorySchoolRepository();

        // GET: Instructor
        public ActionResult Index(int? id, int? courseID)
        {
            var viewModel = new InstructorIndexData
            {
                Instructors = repo.Instructors
                    .Include(i => i.OfficeAssignment)
                    .Include(i => i.Courses.Select(c => c.Department))
                    .OrderBy(i => i.LastName)
            };

            if (id.HasValue)
            {
                ViewBag.InstructorID = id.Value;

                var instructor = viewModel.Instructors
                    .FirstOrDefault(i => i.ID == id.Value);

                viewModel.Courses = instructor?.Courses;
            }

            if (courseID.HasValue)
            {
                ViewBag.CourseID = courseID.Value;

                viewModel.Enrollments = repo.Enrollments
                    .Where(e => e.Course.CourseID == courseID.Value)
                    .Include(e => e.Student)
                    .ToList();
            }

            return View(viewModel);
        }

        // GET: Instructor/Details/5
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var instructor = repo.Instructors.FirstOrDefault(i => i.ID == id.Value);

            if (instructor == null)
                return HttpNotFound();

            return View(instructor);
        }

        // GET: Instructor/Create
        public ActionResult Create()
        {
            var instructor = new Instructor
            {
                Courses = new List<Course>()
            };

            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        // POST: Instructor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "LastName,FirstMidName,HireDate,OfficeAssignment")]
            Instructor instructor,
            string[] selectedCourses)
        {
            if (selectedCourses != null)
            {
                instructor.Courses = selectedCourses
                    .Select(id => repo.Courses.FirstOrDefault(c => c.CourseID == int.Parse(id)))
                    .Where(c => c != null)
                    .ToList();
            }

            if (ModelState.IsValid)
            {
                // repo.AddInstructor(instructor);
                // repo.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        // GET: Instructor/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var instructor = repo.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .FirstOrDefault(i => i.ID == id.Value);

            if (instructor == null)
                return HttpNotFound();

            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        // POST: Instructor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int? id, string[] selectedCourses)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var instructorToUpdate = repo.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .FirstOrDefault(i => i.ID == id.Value);

            if (instructorToUpdate == null)
                return HttpNotFound();

            if (TryUpdateModel(instructorToUpdate, "",
                new[] { "LastName", "FirstMidName", "HireDate", "OfficeAssignment" }))
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment?.Location))
                        instructorToUpdate.OfficeAssignment = null;

                    UpdateInstructorCourses(selectedCourses, instructorToUpdate);

                    repo.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("",
                        "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
                }
            }

            PopulateAssignedCourseData(instructorToUpdate);
            return View(instructorToUpdate);
        }

        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructor)
        {
            if (selectedCourses == null)
            {
                instructor.Courses = new List<Course>();
                return;
            }

            var selectedHS = new HashSet<string>(selectedCourses);
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.CourseID));

            foreach (var course in repo.Courses)
            {
                bool isSelected = selectedHS.Contains(course.CourseID.ToString());
                bool isAssigned = instructorCourses.Contains(course.CourseID);

                if (isSelected && !isAssigned)
                    instructor.Courses.Add(course);

                if (!isSelected && isAssigned)
                    instructor.Courses.Remove(course);
            }
        }

        // GET: Instructor/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var instructor = repo.Instructors.FirstOrDefault(i => i.ID == id.Value);

            if (instructor == null)
                return HttpNotFound();

            return View(instructor);
        }

        // POST: Instructor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var instructor = repo.Instructors
                .Include(i => i.OfficeAssignment)
                .FirstOrDefault(i => i.ID == id);

            if (instructor == null)
                return HttpNotFound();

            instructor.OfficeAssignment = null;
            repo.DeleteInstructor(instructor);

            var department = repo.Departments
                .FirstOrDefault(d => d.InstructorID == id);

            if (department != null)
                department.InstructorID = null;

            repo.SaveChanges();
            return RedirectToAction("Index");
        }

        private void PopulateAssignedCourseData(Instructor instructor)
        {
            var allCourses = repo.Courses;
            var instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.CourseID));

            var viewModel = allCourses.Select(course => new AssignedCourseData
            {
                CourseID = course.CourseID,
                Title = course.Title,
                Assigned = instructorCourses.Contains(course.CourseID)
            }).ToList();

            ViewBag.Courses = viewModel;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                repo.Dispose();

            base.Dispose(disposing);
        }
    }
}
