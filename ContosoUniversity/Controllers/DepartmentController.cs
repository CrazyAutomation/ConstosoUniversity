using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;

namespace ContosoUniversity.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ISchoolRepository repo = new InMemorySchoolRepository();

        // GET: Department
        //public async Task<ActionResult> Index()
        //{
        //   var departments = repo.Departments.Include(d => d.Administrator);
        //    return View(departments.ToList());

        //}

        public ActionResult Index(int? selectedDepartment)
        {
            // Build a list of departments for the select list (all departments, ordered by name)
            var departmentsForSelect = repo.Departments
                .OrderBy(d => d.Name)
                .ToList();

            ViewBag.SelectedDepartment = new SelectList(
                departmentsForSelect, "DepartmentID", "Name", selectedDepartment);

            int departmentId = selectedDepartment ?? 0;

            // Build the filtered list to display (including Administrator navigation property)
            var departments = repo.Departments
                .Include(d => d.Administrator)
                .Where(d => !selectedDepartment.HasValue || d.DepartmentID == departmentId)
                .OrderBy(d => d.Name)
                .ToList();

            return View(departments);
        }

        // GET: Department/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var department = repo.Departments.FirstOrDefault(d => d.DepartmentID == id.Value);

            if (department == null)
                return HttpNotFound();

            return View(department);
        }

        // GET: Department/Create
        public ActionResult Create()
        {
            PopulateInstructorDropDownList();
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(
            [Bind(Include = "DepartmentID,Name,Budget,StartDate,InstructorID")]
            Department department)
        {
            if (!ModelState.IsValid)
            {
                PopulateInstructorDropDownList(department.InstructorID);
                return View(department);
            }

            repo.AddDepartment(department);
            await repo.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // GET: Department/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var department = repo.Departments.FirstOrDefault(d => d.DepartmentID == id.Value);

            if (department == null)
                return HttpNotFound();

            PopulateInstructorDropDownList(department.InstructorID);
            return View(department);
        }

        // POST: Department/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, byte[] rowVersion)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            string[] fieldsToBind = { "Name", "Budget", "StartDate", "InstructorID", "RowVersion" };

            var departmentToUpdate = await repo.Departments
                .SingleOrDefaultAsync(d => d.DepartmentID == id);

            if (departmentToUpdate == null)
            {
                // Department was deleted by another user
                var deletedDepartment = new Department();
                TryUpdateModel(deletedDepartment, fieldsToBind);

                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The department was deleted by another user.");

                PopulateInstructorDropDownList(deletedDepartment.InstructorID);
                return View(deletedDepartment);
            }

            if (TryUpdateModel(departmentToUpdate, fieldsToBind))
            {
                try
                {
                    repo.Entry(departmentToUpdate).OriginalValues["RowVersion"] = rowVersion;
                    await repo.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    HandleConcurrencyConflict(ex, departmentToUpdate);
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("",
                        "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
                }
            }

            PopulateInstructorDropDownList(departmentToUpdate.InstructorID);
            return View(departmentToUpdate);
        }

        // GET: Department/Delete/5
        public async Task<ActionResult> Delete(int? id, bool? concurrencyError)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var department = await repo.Departments
                .SingleOrDefaultAsync(d => d.DepartmentID == id);

            if (department == null)
            {
                if (concurrencyError.GetValueOrDefault())
                    return RedirectToAction("Index");

                return HttpNotFound();
            }

            if (concurrencyError.GetValueOrDefault())
            {
                ViewBag.ConcurrencyErrorMessage =
                    "The record you attempted to delete was modified by another user. "
                    + "The delete operation was canceled and the current values are displayed. "
                    + "If you still want to delete this record, click Delete again.";
            }

            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(Department department)
        {
            try
            {
                repo.Entry(department).State = EntityState.Deleted;
                await repo.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true, id = department.DepartmentID });
            }
            catch (DataException)
            {
                ModelState.AddModelError(string.Empty,
                    "Unable to delete. Try again, and if the problem persists contact your system administrator.");
                return View(department);
            }
        }

        // ------------------------------
        // Helpers
        // ------------------------------

        private void PopulateInstructorDropDownList(object selectedInstructor = null)
        {
            ViewBag.InstructorID = new SelectList(
                repo.Instructors.OrderBy(i => i.LastName),
                "ID",
                "FullName",
                selectedInstructor);
        }

        private void HandleConcurrencyConflict(DbUpdateConcurrencyException ex, Department departmentToUpdate)
        {
            var entry = ex.Entries.Single();
            var clientValues = (Department)entry.Entity;
            var databaseEntry = entry.GetDatabaseValues();

            if (databaseEntry == null)
            {
                ModelState.AddModelError(string.Empty,
                    "Unable to save changes. The department was deleted by another user.");
                return;
            }

            var databaseValues = (Department)databaseEntry.ToObject();

            if (databaseValues.Name != clientValues.Name)
                ModelState.AddModelError("Name", "Current value: " + databaseValues.Name);

            if (databaseValues.Budget != clientValues.Budget)
                ModelState.AddModelError("Budget", "Current value: " + String.Format("{0:c}", databaseValues.Budget));

            if (databaseValues.StartDate != clientValues.StartDate)
                ModelState.AddModelError("StartDate", "Current value: " + String.Format("{0:d}", databaseValues.StartDate));

            if (databaseValues.InstructorID != clientValues.InstructorID)
            {
                var instructor = repo.Instructors.SingleOrDefault(i => i.ID == databaseValues.InstructorID);
                ModelState.AddModelError("InstructorID", "Current value: " + instructor?.FullName);
            }

            ModelState.AddModelError(string.Empty,
                "The record you attempted to edit was modified by another user. "
                + "The current database values have been displayed. "
                + "If you still want to edit this record, click Save again.");

            departmentToUpdate.RowVersion = databaseValues.RowVersion;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                repo.Dispose();

            base.Dispose(disposing);
        }
    }
}
