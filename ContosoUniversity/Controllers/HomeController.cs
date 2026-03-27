using System;
using System.Linq;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.ViewModels;

namespace ContosoUniversity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISchoolRepository repo = new InMemorySchoolRepository();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            // LINQ version (works with InMemorySchoolRepository)
            var data = repo.Students
                .GroupBy(s => s.EnrollmentDate)
                .Select(g => new EnrollmentDateGroup
                {
                    EnrollmentDate = g.Key,
                    StudentCount = g.Count()
                })
                .OrderBy(g => g.EnrollmentDate)
                .ToList();

            return View(data);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                repo.Dispose();

            base.Dispose(disposing);
        }
    }
}