using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ElearningWebsite.Models;
using X.PagedList.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace ElearningWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoursesManagerController : Controller
    {
        private readonly LearningManagementSystemContext _context;

        public CoursesManagerController(LearningManagementSystemContext context)
        {
            _context = context;
        }


        #region Hiển thị danh sách toàn bộ khóa học
        [Authorize(Roles = "1")]
        public IActionResult Index(int? page, string search_box)
        {
            ViewBag.CurrentFilter = search_box;

            var courses = _context.Courses.AsQueryable();
            Console.WriteLine(search_box);
            if (!string.IsNullOrEmpty(search_box))
            {
                courses = courses.Where(c => (c.CourseName != null && c.CourseName.Contains(search_box))
                                             || (c.Description != null && c.Description.Contains(search_box)));
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(courses.ToPagedList(pageNumber, pageSize));
        }
        #endregion


        #region Chi tiết khóa học
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.CourseId == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        #endregion

        #region Tạo khóa học
        [Authorize(Roles = "1")]
        [HttpGet]
        public IActionResult Create()
        {
            var lastCourse = _context.Courses
                .OrderByDescending(c => c.CourseId)
                .FirstOrDefault();
            string newCourseId;

            if (lastCourse == null)
            {
                newCourseId = "C001";
            }
            else
            {
                int lastNumber = int.Parse(lastCourse.CourseId.Substring(1));
                newCourseId = $"C{(lastNumber + 1).ToString("D3")}";
            }

            var course = new Course
            {
                CourseId = newCourseId
            };
            return View(course);
        }
        [Authorize(Roles = "1")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
               
                if (string.IsNullOrEmpty(course.CourseId))
                {
                    var lastCourse = await _context.Courses
                                          .OrderByDescending(c => c.CourseId)
                                          .FirstOrDefaultAsync();

                    if (lastCourse != null && lastCourse.CourseId.Length >= 4)
                    {
                        string lastId = lastCourse.CourseId.Substring(1);
                        int nextId = int.Parse(lastId) + 1;
                        course.CourseId = "C" + nextId.ToString("D3");
                    }
                    else
                    {
                        course.CourseId = "C001";
                    }
                }

                if(course.TuitionFee < 0)
                {
                    TempData["Error"] = "Khóa học không được âm.";
                    return View(course);
                }    


              //Kiếm tra course có trùng tên ?
              if(_context.Courses.Any(c=> c.CourseName.ToLower() == course.CourseName.ToLower()))
              {
                    
                    return View(course);
              }


                if (imageFile != null && imageFile.Length > 0)
                {
                    string imgPath = await MyUtil.UploadImg(imageFile, "CourseImg");
                    if (!string.IsNullOrEmpty(imgPath))
                    {
                        course.ImgPath = imgPath;
                    }
                    else
                    {
                        
                        TempData["Error"] = "Không thể tải lên hình ảnh.";
                        return View(course);
                    }
                }
                course.CurrentStudents = 0;
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(course);
        }

        #endregion



        #region Chỉnh sửa khóa học
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }
        [Authorize(Roles = "1")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Course course, IFormFile? imageFile)
        {
            if (id != course.CourseId)
            {
                return NotFound();
            }




            if (ModelState.IsValid)
            {
                try
                {
                    var existingCourse = await _context.Courses.FindAsync(id);
                    if (existingCourse == null)
                    {
                        return NotFound();
                    }
                    if (course.TuitionFee < 0)
                    {
                        TempData["Error"] = "Khóa học không được âm.";
                        return View(course);
                    }

                    existingCourse.CourseName = course.CourseName;
                    existingCourse.Instructor = course.Instructor;
                    existingCourse.StartDate = course.StartDate;
                    existingCourse.TuitionFee = course.TuitionFee;
                    existingCourse.MaxStudents = course.MaxStudents;
                    existingCourse.VideoCount = course.VideoCount;
                    existingCourse.CurrentStudents = course.CurrentStudents;
                    existingCourse.Description = course.Description;

                   
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string imgPath = await MyUtil.UploadImg(imageFile, "CourseImg");
                        if (!string.IsNullOrEmpty(imgPath))
                        {
                            existingCourse.ImgPath = imgPath;
                        }
                        else
                        {
                            ModelState.AddModelError("ImgPath", "Không thể tải lên hình ảnh.");
                            return View(course);
                        }
                    }

                    _context.Update(existingCourse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.CourseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }
        #endregion

        #region Xóa khóa học
        [Authorize(Roles = "1")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(string id)
        {
            return _context.Courses.Any(e => e.CourseId == id);
        }
        #endregion 
    }
}
