using ElearningWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace ElearningWebsite.Controllers
{
    public class Courses : Controller
    {
        private readonly LearningManagementSystemContext _db;
      
        public Courses(LearningManagementSystemContext db)
        {
            _db = db;         
        }
        public IActionResult Index(int? page, string search_box) 
        {
            ViewBag.CurrentFilter = search_box; 

            var courses = _db.Courses.AsQueryable();
            Console.WriteLine(search_box);
            if (!string.IsNullOrEmpty(search_box))
            {
                courses = courses.Where(c => c.CourseName.Contains(search_box)
                                         || c.Description.Contains(search_box));
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(courses.ToPagedList(pageNumber, pageSize));
        }


        #region Trang chi tiết khóa học
        public IActionResult Details(string id)
        {
            Console.WriteLine(id);
            var course = _db.Courses.FirstOrDefault(c => c.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }
            else
            {
                return View(course);
            }

        }
        #endregion
    }
}
