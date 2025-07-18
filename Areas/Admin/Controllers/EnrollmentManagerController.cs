using ElearningWebsite.Controllers;
using ElearningWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace ElearningWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]/[action]")] 
    public class EnrollmentManagerController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LearningManagementSystemContext _db;
        public EnrollmentManagerController(ILogger<HomeController> logger, LearningManagementSystemContext db)
        {
            _logger = logger;
            _db = db;
        }
        #region Xem danh sách học sinh đăng kí khóa học
        [Authorize(Roles = "1")]
        public IActionResult Index(int? page, string searchValue)
        {
            int pageSize = 8;
            int pageNumber = page ?? 1;

            var query = _db.Enrollments.AsNoTracking();

            if (!string.IsNullOrEmpty(searchValue))
            {
                var matchedStudentIDs = _db.Students
                                           .Where(s => s.FullName.Contains(searchValue))
                                           .Select(s => s.StudentId)
                                           .ToList();

                var matchedCourseIDs = _db.Courses
                                          .Where(c => c.CourseName.Contains(searchValue))
                                          .Select(c => c.CourseId)
                                          .ToList();

                query = query.Where(e =>
                            matchedStudentIDs.Contains(e.StudentId) ||
                            matchedCourseIDs.Contains(e.CourseId));
            }

            var lstEnrollment = query
                                .OrderBy(e => e.EnrollmentId)
                                .ToPagedList(pageNumber, pageSize);

           
            var studentIDs = lstEnrollment.Select(e => e.StudentId).Distinct().ToList();
            var courseIDs = lstEnrollment.Select(e => e.CourseId).Distinct().ToList();

         
            var students = _db.Students
                              .Where(s => studentIDs.Contains(s.StudentId))
                              .ToDictionary(s => s.StudentId, s => s);

            var courses = _db.Courses
                             .Where(c => courseIDs.Contains(c.CourseId))
                             .ToDictionary(c => c.CourseId, c => c);

          
            ViewBag.Students = students;
            ViewBag.Courses = courses;
            return View(lstEnrollment);
        }
        #endregion

        
    }
}
