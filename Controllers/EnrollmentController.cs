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
        public IActionResult Index(int? page, string search_box)
        {
            var enrollement = _db.Enrollments.AsNoTracking();

            if (!string.IsNullOrEmpty(search_box))
            {
               //var matchingCourse =  
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);
            return View(enrollement.ToPagedList(pageNumber, pageSize));
        }
        #endregion

        #region Đăng kí khóa học
        #endregion
    }
}
