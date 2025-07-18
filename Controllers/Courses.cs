using System.Security.Claims;
using ElearningWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Authorization;
namespace ElearningWebsite.Controllers
{
    public class Courses : Controller
    {
        private readonly LearningManagementSystemContext _db;

        public Courses(LearningManagementSystemContext db)
        {
            _db = db;
        }
        #region Hiển thị danh sách khóa học
        public IActionResult Index(int? page, string search_box, string instructorName, string startDateFrom, string startDateTo, decimal? costMin, decimal? costMax)
        {
            ViewBag.CurrentFilter = search_box;
            ViewBag.InstructorName = instructorName;
            ViewBag.StartDateFrom = startDateFrom;
            ViewBag.StartDateTo = startDateTo;
            ViewBag.CostMin = costMin;
            ViewBag.CostMax = costMax;

            //dùng userid hiển thị những khóa học sinh chưa học trong phần our Course
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var coursesQuery = _db.Courses.AsQueryable();

            if (!string.IsNullOrEmpty(studentId))
            {
               
                var enrolledCourseIDs = _db.Enrollments
                    .Where(e => e.StudentId == studentId)
                    .Select(e => e.CourseId)
                    .ToList();

                coursesQuery = coursesQuery.Where(c => !enrolledCourseIDs.Contains(c.CourseId));
            }

            if (!string.IsNullOrEmpty(search_box))
            {
                coursesQuery = coursesQuery.Where(c => c.CourseName.Contains(search_box));
            }

            if (!string.IsNullOrEmpty(instructorName))
            {
                coursesQuery = coursesQuery.Where(c => c.Instructor.Contains(instructorName));
            }

            if (!string.IsNullOrEmpty(startDateFrom) && DateOnly.TryParse(startDateFrom, out var startFrom))
            {
                coursesQuery = coursesQuery.Where(c => c.StartDate >= startFrom);
            }

            if (!string.IsNullOrEmpty(startDateTo) && DateOnly.TryParse(startDateTo, out var startTo))
            {
                coursesQuery = coursesQuery.Where(c => c.StartDate <= startTo);
            }

            if (costMin.HasValue)
            {
                coursesQuery = coursesQuery.Where(c => c.TuitionFee >= costMin.Value);
            }

            if (costMax.HasValue)
            {
                coursesQuery = coursesQuery.Where(c => c.TuitionFee <= costMax.Value);
            }

            int pageSize = 6;
            int pageNumber = page ?? 1;


            var pagedCourses = coursesQuery
                .OrderBy(c => c.CourseName)
                .ToPagedList(pageNumber, pageSize);

            return View(pagedCourses);
        }
        #endregion

        #region Trang chi tiết khóa học  
        public IActionResult Details(string id)
        {
            var course = _db.Courses.FirstOrDefault(c => c.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isEnrolled = false;
            int progress = 0;
            if (!string.IsNullOrEmpty(studentId))
            {
                var enrollment = _db.Enrollments
                    .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == id);
                if (enrollment != null)
                {
                    isEnrolled = true;
                    progress = enrollment.Progress.HasValue ? enrollment.Progress.Value : 0; // Kiểm tra null
                }
            }
            ViewBag.IsEnrolled = isEnrolled;
            ViewBag.StudentId = studentId;
            ViewBag.Progress = progress;

            return View(course);
        }
        [HttpGet]
        public IActionResult WatchVideo(string courseId, int videoIndex)
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(studentId))
            {
                TempData["Error"] = "Vui lòng đăng nhập để xem video.";
                return RedirectToAction("Details", new { id = courseId });
            }

            var enrollment = _db.Enrollments
                .FirstOrDefault(e => e.StudentId == studentId && e.CourseId == courseId);

            if (enrollment == null)
            {
                TempData["Error"] = "Bạn phải đăng ký khóa học để xem video.";
                return RedirectToAction("Details", new { id = courseId });
            }

         
            int currentProgress = enrollment.Progress ?? 0;

           
            if (currentProgress < 100)
            {
                currentProgress += 20;
              
                enrollment.Progress = Math.Min(currentProgress, 100);
                _db.SaveChanges();
            }

           
            if (enrollment.Progress >= 100)
            {
                TempData["SuccessMessage"] = "Bạn đã hoàn thành khóa học!";
            }
            else
            {
                TempData["SuccessMessage"] = "Video đã được mở! Tiến độ của bạn đã được cập nhật.";
            }

            return RedirectToAction("Details", new { id = courseId });
        }
        #endregion





    }
}
