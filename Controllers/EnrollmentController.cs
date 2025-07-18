using System.Security.Claims;
using ElearningWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace ElearningWebsite.Controllers
{
    public class EnrollmentController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly LearningManagementSystemContext _db;
        public EnrollmentController(ILogger<HomeController> logger, LearningManagementSystemContext db)
        {
            _logger = logger;
            _db = db;
        }

        #region Xem danh sách các khóa học đã đăng kí
        public IActionResult Index(int? page)
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(studentId))
            {
                return RedirectToAction("Login", "Accounts");
            }

          
            var enrollments = _db.Enrollments
                                 .Where(e => e.StudentId == studentId && (e.Progress == 0 || e.Progress == null))
                                 .OrderByDescending(e => e.EnrollmentDate)
                                 .AsNoTracking()
                                 .ToList();

           
            var courses = enrollments
                          .Select(e => _db.Courses.FirstOrDefault(c => c.CourseId == e.CourseId))
                          .Where(c => c != null)
                          .ToList();

            ViewBag.Course = courses;

            int pageSize = 6;
            int pageNumber = page ?? 1;
            return View(enrollments.ToPagedList(pageNumber, pageSize));
        }
        #endregion


        #region Xem danh sách khóa học đã hoàn thành
        public IActionResult CompletedCourses(int? page)
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(studentId))
            {
                return RedirectToAction("Login", "Accounts");
            }          
            var completedEnrollments = _db.Enrollments
                                           .Where(e => e.StudentId == studentId && e.Progress == 100)
                                           .OrderByDescending(e => e.EnrollmentDate)
                                           .AsNoTracking()
                                           .ToList();

            var completedCourses = completedEnrollments
                                    .Select(e => _db.Courses.FirstOrDefault(c => c.CourseId == e.CourseId))
                                    .ToList();

            ViewBag.CourseList = completedCourses;

            int pageSize = 6;
            int pageNumber = page ?? 1;
            return View(completedEnrollments.ToPagedList(pageNumber, pageSize));
        }
        #endregion



        #region Xem danh sách khóa học đang học
        public IActionResult InProgressCourses(int? page)
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(studentId))
            {
                return RedirectToAction("Login", "Accounts");
            } 
            var inProgressEnrollments = _db.Enrollments
                                           .Where(e => e.StudentId == studentId && e.Progress < 100 && e.Progress >0)
                                           .OrderByDescending(e => e.EnrollmentDate)
                                           .AsNoTracking()
                                           .ToList();

            var inProgressCourses = inProgressEnrollments
                                    .Select(e => _db.Courses.FirstOrDefault(c => c.CourseId == e.CourseId))
                                    .ToList();

            ViewBag.CourseList = inProgressCourses;

            int pageSize = 6;
            int pageNumber = page ?? 1;
            return View(inProgressEnrollments.ToPagedList(pageNumber, pageSize));
        }
        #endregion

        #region Đăng kí khóa học
        [HttpPost]
        [Authorize]
        public IActionResult RegisterCourse(string id)
        {
           
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Invalid Course ID";
                return RedirectToAction("Index", "Courses");
            }

            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (studentId == null)
            {
                TempData["Error"] = "Bạn cần đăng nhập để đăng kí.";
                return RedirectToAction("Login", "Account");
            }
            var course = _db.Courses.Find(id);
            if (course == null)
            {
                TempData["Error"] = "Không tìm thấy khóa học.";
                return RedirectToAction("Index", "Courses");
            }

            if (_db.Enrollments.Any(e => e.CourseId == id && e.StudentId == studentId))
            {
                TempData["Error"] = "Bạn đã đăng ký khóa học này rồi.";
                return RedirectToAction("Index");
            }

          
            course.CurrentStudents++;
            if (course.CurrentStudents > course.MaxStudents)
            {
                course.CurrentStudents--;
                TempData["Error"] = "Khóa học đã đầy không thể đăng kí";
                return RedirectToAction("Index");
            }

            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            if (course.StartDate <= currentDate)
            {
                TempData["Error"] = "Lớp học đã bắt đầu, không thể đăng ký.";
                return RedirectToAction("Index", "Courses");
            }
            var lastEnrollment = _db.Enrollments
                                .OrderByDescending(e => e.EnrollmentId)
                                .FirstOrDefault();
            int newIdNumber = 1;
            if (lastEnrollment != null && lastEnrollment.EnrollmentId.Length > 1)
            {
                string numberPart = lastEnrollment.EnrollmentId.Substring(1); // Bỏ chữ 'E'
                if (int.TryParse(numberPart, out int lastNumber))
                {
                    newIdNumber = lastNumber + 1;
                }
            }
            string newEnrollmentId = $"E{newIdNumber:D3}";
            var enrollment = new Enrollment
            {
                EnrollmentId = newEnrollmentId,
                StudentId = studentId,
                CourseId = course.CourseId,
                EnrollmentDate = DateOnly.FromDateTime(DateTime.Now),
                Progress = 0
            };
            Console.WriteLine(enrollment.EnrollmentId);
           
            _db.Enrollments.Add(enrollment);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Đăng ký khóa học thành công!";
            return RedirectToAction("Index");
        }
        #endregion

        #region Hủy khóa học
        [HttpPost]
        [Authorize]
        public IActionResult CancelEnrollment(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Invalid Course ID";
                return RedirectToAction("Index", "Course");
            }

            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
            {
                TempData["Error"] = "Bạn cần đăng nhập để thực hiện thao tác này.";
                return RedirectToAction("Login", "Account");
            }
            var enrollment = _db.Enrollments
                .FirstOrDefault(e => e.CourseId == id && e.StudentId == studentId);
            if (enrollment == null)
            {
                TempData["Error"] = "Bạn chưa đăng ký khóa học này.";
                return RedirectToAction("Index");
            }
            var course = _db.Courses.FirstOrDefault(c => c.CourseId == id);
            if (course == null)
            {
                TempData["Error"] = "Không tìm thấy khóa học.";
                return RedirectToAction("Index");
            } 
            //kiểm tra khóa đã bắt đầu chưa nếu rồi kh cho hủy
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            if (course.StartDate <= currentDate)
            {
                TempData["Error"] = "Lớp học đã bắt đầu, không thể hủy đăng ký.";
                return RedirectToAction("Index");
            }          
            course.CurrentStudents--;
            _db.Courses.Update(course);          
            _db.Enrollments.Remove(enrollment);        
            _db.SaveChanges();                 
            TempData["SuccessMessage"] = "Hủy đăng ký khóa học thành công.";
            return RedirectToAction("Index");
        }
        #endregion
    }
}
