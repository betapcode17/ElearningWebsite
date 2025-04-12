using System.Diagnostics;
using ElearningWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElearningWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LearningManagementSystemContext _db;
        public HomeController(ILogger<HomeController> logger, LearningManagementSystemContext db)
        {
            _logger = logger;
            _db = db; // Khởi tạo _db
        }

        public IActionResult Index()
        {
            var latestCourses = _db.Courses
                         .OrderByDescending(c => c.CourseId)
                         .Take(6)
                         .ToList();
            return View(latestCourses);
          
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
