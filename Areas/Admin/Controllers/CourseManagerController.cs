using ElearningWebsite.Controllers;
using ElearningWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElearningWebsite.Areas.Admin.Controllers
{
    public class CourseManagerController : Controller
    {
        private readonly LearningManagementSystemContext _db;
        private readonly ILogger<AccountsController> _logger;


        public CourseManagerController(LearningManagementSystemContext db, ILogger<AccountsController> logger)
        {
            _db = db;
            _logger = logger;

        }

        public IActionResult index()
        {
            var courses = _db.Courses.AsNoTracking().ToList();
            return View(courses);
        }

        #region Thêm khóa học
        #endregion

        #region Sửa khóa học
        #endregion

        #region Xóa khóa học
        #endregion
    }
}
