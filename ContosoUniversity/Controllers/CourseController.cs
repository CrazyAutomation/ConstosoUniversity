using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using System.Data.Entity.Infrastructure;

namespace ContosoUniversity.Controllers
{
    public class CourseController : Controller
    {
        private readonly ISchoolRepository repo = new InMemorySchoolRepository();

        // GET: Course
        public ActionResult Index(int? selectedDepartment)
        {
            var departments = repo.Departments
                .OrderBy(d => d.Name)
                .ToList();

            ViewBag.SelectedDepartment = new SelectList(
                departments, "DepartmentID", "Name", selectedDepartment);

            int departmentId = selectedDepartment ?? 0;

            var courses = repo.Courses
                .Include(c => c.Department)
                .Where(c => !selectedDepartment.HasValue || c.DepartmentID == departmentId)
                .OrderBy(c => c.CourseID)
                .ToList();

            return View(courses);
        }

        // GET: Course/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var course = repo.Courses.FirstOrDefault(c => c.CourseID == id.Value);

            if (course == null)
                return HttpNotFound();

            return View(course);
        }

        // GET: Course/Create
        public ActionResult Create()
        {
            PopulateDepartmentsDropDownList();
            return View();
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,Title,Credits,DepartmentID")] Course course)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // ISchoolRepository exposes IQueryable<T> for Courses, which doesn't provide Add().
                    // Use the repository's Entry to mark the entity as Added so SaveChanges will insert it.
                    repo.Entry(course).State = EntityState.Added;
                    repo.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again later.");
            }

            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        // GET: Course/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var course = repo.Courses.FirstOrDefault(c => c.CourseID == id.Value);

            if (course == null)
                return HttpNotFound();

            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        // POST: Course/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var courseToUpdate = repo.Courses.FirstOrDefault(c => c.CourseID == id.Value);

            if (courseToUpdate == null)
                return HttpNotFound();

            if (TryUpdateModel(courseToUpdate, "",
                new[] { "Title", "Credits", "DepartmentID" }))
            {
                try
                {
                    repo.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again later.");
                }
            }

            PopulateDepartmentsDropDownList(courseToUpdate.DepartmentID);
            return View(courseToUpdate);
        }

        // GET: Course/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var course = repo.Courses.FirstOrDefault(c => c.CourseID == id.Value);

            if (course == null)
                return HttpNotFound();

            return View(course);
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var course = repo.Courses.FirstOrDefault(c => c.CourseID == id);

            if (course != null)
            {
                repo.Entry(course).State = EntityState.Deleted;
                repo.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // GET: Course/UpdateCourseCredits
        public ActionResult UpdateCourseCredits()
        {
            return View();
        }

        // POST: Course/UpdateCourseCredits
        [HttpPost]
        public ActionResult UpdateCourseCredits(int? multiplier)
        {
            if (multiplier.HasValue)
            {
                // IRepository doesn't expose Database.ExecuteSqlCommand; update entities and save.
                var courses = repo.Courses.ToList();
                foreach (var c in courses)
                {
                    c.Credits = c.Credits * multiplier.Value;
                    repo.Entry(c).State = EntityState.Modified;
                }

                ViewBag.RowsAffected = repo.SaveChanges();
            }

            return View();
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departments = repo.Departments
                .OrderBy(d => d.Name)
                .ToList();

            ViewBag.DepartmentID = new SelectList(
                departments, "DepartmentID", "Name", selectedDepartment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                repo.Dispose();

            base.Dispose(disposing);
        }
    }
}
