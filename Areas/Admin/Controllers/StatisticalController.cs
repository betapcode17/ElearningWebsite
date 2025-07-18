using System.Text.Json;
using ElearningWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace ElearningWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StatisticalController : Controller
    {

        private readonly LearningManagementSystemContext _db;

        public StatisticalController(LearningManagementSystemContext db)
        {
            _db = db;
        }

        #region Thống kê tổng quát
        [Authorize(Roles = "1")]
        public IActionResult Statistical()
        {
           
            ViewBag.TotalStudent = _db.Students.Count();
            ViewBag.TotalCourse = _db.Courses.Count();
            ViewBag.TotalEnrollment = _db.Enrollments.Count();
            return View();
        }

        #endregion

        #region Thống kê doanh thu theo thời gian
        [Authorize(Roles = "1")]
        public async Task<IActionResult> StatisticalByTime(string searchValue = "", string? startDate = null, string? endDate = null)
        {
            DateOnly? startDateOnly = null;
            DateOnly? endDateOnly = null;

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                startDateOnly = DateOnly.Parse(startDate);
                endDateOnly = DateOnly.Parse(endDate);

                
            }
            else
            {
                var endDateTime = DateTime.Now;
                //tính từ 30 ngày gần nhất
                var startDateTime = endDateTime.AddDays(-30);
                startDateOnly = DateOnly.FromDateTime(startDateTime);
                endDateOnly = DateOnly.FromDateTime(endDateTime);
            }
            var enrollmentsQuery = _db.Enrollments
                .Where(e => e.EnrollmentDate >= startDateOnly && e.EnrollmentDate <= endDateOnly)
                .Join(_db.Courses,
                      e => e.CourseId,
                      c => c.CourseId,
                      (e, c) => new { Enrollment = e, Course = c })
                .Where(e => string.IsNullOrEmpty(searchValue) || e.Course.CourseName.Contains(searchValue));

            var revenueDataQuery = enrollmentsQuery
                .GroupBy(e => new
                {
                    e.Enrollment.EnrollmentDate.Year,
                    e.Enrollment.EnrollmentDate.Month,
                    e.Enrollment.EnrollmentDate.Day,
                    e.Course.CourseName
                })
                .Select(g => new
                {
                    g.Key.Year,
                    g.Key.Month,
                    g.Key.Day,
                    g.Key.CourseName, 
                    EnrollmentCount = g.Count(),
                    Revenue = g.Sum(e => e.Course.TuitionFee)
                });

         
            var revenueData = await revenueDataQuery.AsNoTracking().ToListAsync();

          
            var formattedRevenueData = revenueData
                .Select(r => new
                {
                    TimePeriod = new DateTime(r.Year, r.Month, r.Day).ToString("dd/MM/yyyy"),
                    r.EnrollmentCount,
                    r.Revenue,
                    r.CourseName
                })
                .OrderBy(r => r.TimePeriod)
                .ToList();

         
            var totalRevenue = formattedRevenueData.Sum(r => (decimal?)r.Revenue) ?? 0;

          
            var chartData = formattedRevenueData
                .GroupBy(r => r.TimePeriod) 
                .Select(g => new
                {
                    TimePeriod = g.Key,
                    TotalRevenue = g.Sum(r => r.Revenue)
                })
                .OrderBy(r => r.TimePeriod)
                .ToList();

            
            ViewBag.ChartLabels = chartData.Select(r => r.TimePeriod).ToList();
            ViewBag.ChartData = chartData.Select(r => r.TotalRevenue).Cast<decimal>().ToList();
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.StartDate = startDateOnly;
            ViewBag.EndDate = endDateOnly;
            ViewBag.SearchValue = searchValue;
            var Data = formattedRevenueData
                .Select(r => new 
                {
                     r.CourseName,
                    r.TimePeriod,
                   r.EnrollmentCount,
                    r.Revenue
                })
                .ToList();


            if (endDateOnly < startDateOnly)
            {
                TempData["Error"] = "Lỗi startDateOnly không được lớn hơn endDateOnly ";
                return View(Data);
            }
            return View(Data);
        }
        #endregion


        #region Thống kê doanh thu theo khóa học
        [Authorize(Roles = "1")]
        public async Task<IActionResult> StatisticalByCourse(string searchValue = "", string? startDate = null, string? endDate = null)
        {
            DateOnly? startDateOnly = null;
            DateOnly? endDateOnly = null;

            
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                startDateOnly = DateOnly.Parse(startDate);
                endDateOnly = DateOnly.Parse(endDate);

               
            }
            else
            {
                var endDateTime = DateTime.Now;
                var startDateTime = endDateTime.AddDays(-30);
                startDateOnly = DateOnly.FromDateTime(startDateTime);
                endDateOnly = DateOnly.FromDateTime(endDateTime);
            }

          
            var filteredEnrollments = _db.Enrollments
                .Where(e => e.EnrollmentDate >= startDateOnly && e.EnrollmentDate <= endDateOnly);

           
            var enrollmentsWithCourses = filteredEnrollments
                .Join(_db.Courses,
                      e => e.CourseId,
                      c => c.CourseId,
                      (e, c) => new { Enrollment = e, Course = c })
                .Where(x => string.IsNullOrEmpty(searchValue) || x.Course.CourseName.Contains(searchValue));

          
            var revenueDataQuery = enrollmentsWithCourses
                .GroupBy(x => x.Course.CourseName)
                .Select(g => new
                {
                    CourseName = g.Key,
                    EnrollmentCount = g.Count(),
                    Revenue = g.Sum(x => x.Course.TuitionFee)
                });

            var revenueData = await revenueDataQuery.AsNoTracking().ToListAsync();       
            var totalRevenue = revenueData.Sum(r => r.Revenue);
            ViewBag.ChartLabels = revenueData.Select(r => r.CourseName).ToList();
            ViewBag.ChartData = revenueData.Select(r => r.Revenue).Cast<decimal>().ToList();
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.StartDate = startDateOnly;
            ViewBag.EndDate = endDateOnly;
            ViewBag.SearchValue = searchValue;          
           
            var Data = revenueData
                .OrderBy(r => r.CourseName)
                .ToList();
            if (endDateOnly < startDateOnly)
            {
                TempData["Error"] = "Lỗi startDateOnly không được lớn hơn endDateOnly ";
                return View(Data);
            }
            return View(Data);
        }
        #endregion

        #region Thống kê số lượt đăng ký theo khóa học
        [Authorize(Roles = "1")]
        public async Task<IActionResult> EnrollmentStatsByCourse(string searchValue = "", string? startDate = null, string? endDate = null)
        {
           
            DateOnly? startDateOnly = null;
            DateOnly? endDateOnly = null;

            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                startDateOnly = DateOnly.Parse(startDate);
                endDateOnly = DateOnly.Parse(endDate);
            }
            else
            {
               
                var endDateTime = DateTime.Now;
                var startDateTime = endDateTime.AddDays(-30);
                startDateOnly = DateOnly.FromDateTime(startDateTime);
                endDateOnly = DateOnly.FromDateTime(endDateTime);
            }

          
            var filteredEnrollments = _db.Enrollments
                .Where(e => e.EnrollmentDate >= startDateOnly && e.EnrollmentDate <= endDateOnly);

          
            var enrollmentsWithCourses = filteredEnrollments
                .Join(_db.Courses,
                      e => e.CourseId,
                      c => c.CourseId,
                      (e, c) => new { Enrollment = e, Course = c })
                .Where(x => string.IsNullOrEmpty(searchValue) || x.Course.CourseName.Contains(searchValue));

         
            var enrollmentStatsQuery = enrollmentsWithCourses
                .GroupBy(x => x.Course.CourseName)
                .Select(g => new
                {
                    CourseName = g.Key,
                    EnrollmentCount = g.Count()
                });

          
            var enrollmentStats = await enrollmentStatsQuery.AsNoTracking().ToListAsync();
            var totalEnrollments = enrollmentStats.Sum(r => r.EnrollmentCount);

        
            ViewBag.ChartLabels = enrollmentStats.Select(r => r.CourseName).ToList();
            ViewBag.ChartData = enrollmentStats.Select(r => r.EnrollmentCount).Cast<int>().ToList();
            ViewBag.TotalEnrollments = totalEnrollments;
            ViewBag.StartDate = startDateOnly;
            ViewBag.EndDate = endDateOnly;
            ViewBag.SearchValue = searchValue;

          
          
            var Data = enrollmentStats
                .OrderBy(r => r.CourseName)
                .ToList();
            if (endDateOnly < startDateOnly)
            {
                TempData["Error"] = "Lỗi startDateOnly không được lớn hơn endDateOnly ";
                return View(Data);
            }
            return View(Data);
        }
        #endregion

    }
}


//Vì r.Revenue có thể là kiểu double, int, float... nên
//.Cast<decimal>() sẽ ép kiểu từng phần tử trong danh sách về kiểu decimal để đảm bảo thống nhất khi truyền sang biểu đồ (Chart.js cần kiểu số chính xác).