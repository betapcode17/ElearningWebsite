using System.Buffers;
using ElearningWebsite.Controllers;
using ElearningWebsite.Helpers;
using ElearningWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Extensions;

namespace ElearningWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]/[action]")]
    public class StudentsManagerController : Controller
    {
        private readonly LearningManagementSystemContext _db;

        public StudentsManagerController(LearningManagementSystemContext db)
        {
            _db = db;
        }
        #region Danh sách học sinh
        [Authorize(Roles = "1")]
        public IActionResult Index(int? page,string searchValue)
        {
            var query = _db.Students.AsNoTracking();
            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(s => s.FullName.Contains(searchValue));

            }
            int pageSize = 8;
            int pageNumber = page ?? 1;
            var pagedAccounts = query
                .OrderBy(s => s.StudentId)
                .ToPagedList(pageNumber, pageSize);
            return View(pagedAccounts);
          
        }
        #endregion
        #region Thêm học viên
        [HttpGet]
        [Authorize(Roles = "1")]
        public IActionResult Create()
        {
            var lastStudent = _db.Students
                .OrderByDescending(c => c.StudentId)
                .FirstOrDefault();
            string newStudentId;

            if (lastStudent == null)
            {
                newStudentId = "U001";
            }
            else
            {
                int lastNumber = int.Parse(lastStudent.StudentId.Substring(1));
                newStudentId = $"U{(lastNumber + 1).ToString("D3")}";
            }

            var course = new Student
            {
                StudentId = newStudentId,
            };
            return View(course);
        }
        [HttpPost]
        [Authorize(Roles = "1")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student Student, IFormFile? imageFile)
        {


            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Property: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }
                return View(Student); 
            }


            else if (ModelState.IsValid)
            {
              
                if (string.IsNullOrEmpty(Student.StudentId))
                {
                    var lastStudent = await _db.Students
                                          .OrderByDescending(c => c.StudentId)
                                          .FirstOrDefaultAsync();

                    if (lastStudent != null && lastStudent.StudentId.Length >= 4)
                    {
                        string lastId = lastStudent.StudentId.Substring(1);
                        int nextId = int.Parse(lastId) + 1;
                        Student.StudentId = "U" + nextId.ToString("D3");
                    }
                    else
                    {
                        Student.StudentId = "U001";
                    }
                }
                if (imageFile != null && imageFile.Length > 0)
                {
                    string imgPath = await MyUtil.UploadImg(imageFile, "StudentImg");
                    if (!string.IsNullOrEmpty(imgPath))
                    {
                        Student.ImagePath = imgPath;
                    }
                    else
                    {
                        ModelState.AddModelError("ImgPath", "Không thể tải lên hình ảnh.");
                        return View(Student);
                    }
                }

              
                string numberPart = Student.StudentId.StartsWith("U") ? Student.StudentId.Substring(1) : Student.StudentId; 
                string username = $"Student{numberPart}"; 
                string password = SendEmail.GenerateRandomPassword(12);
                //test email
                string email = "quocdat19991712@gmail.com";
                SendEmail.SendAccountInfoEmail(email, username, password);
                var account = new Account
                {
                    UserId = Student.StudentId,
                    Role = 0,
                    Password = PasswordUtil.HashPassword(password),
                    Username = username,
                };
                _db.Add(Student);
                _db.Add(account);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(Student);
        }
        #endregion
        #region Xóa học viên
        [Authorize(Roles = "1")]
        public IActionResult Delete(string id)
        {
            try
            {
              
                var student =  _db.Students.FirstOrDefault(s => s.StudentId == id);
                if (student == null)
                {
                    TempData["Error"] = "Không tìm thấy học viên.";
                    return RedirectToAction("Index", "StudentsManager");
                }

               
                var enrollmentCount = _db.Enrollments.Count(e => e.StudentId == id);
                if (enrollmentCount > 0)
                {
                    TempData["Error"] = "Không thể xóa học viên vì học viên đã đăng ký khóa học.";
                    return RedirectToAction("Index", "StudentsManager");
                }

              
            
                _db.Students.Remove(student);
                var account = _db.Accounts.FirstOrDefault(a => a.UserId == id);
                if (account != null)
                {
                    _db.Accounts.Remove(account);
                }

                _db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đã xóa học viên thành công.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa học viên: {ex.Message} \n {ex.InnerException?.Message}";
            }

            return RedirectToAction("Index", "StudentsManager");
        }
        #endregion

        #region Chỉnh sửa thông tin cá nhân của học viên
        [Authorize(Roles = "1")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var students = _db.Students.FirstOrDefault(s => s.StudentId == id);
            if (students == null)
            {
                TempData["Error"] = "Không tìm thấy học viên.";
                return RedirectToAction("Index", "StudentsManager");
            }
            return View(students);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Student updatedStudent, IFormFile? imageFile)
        {
            if (string.IsNullOrEmpty(updatedStudent.StudentId))
            {
                TempData["Error"] = "Thiếu mã học viên.";
                return RedirectToAction("Index", "StudentsManager");
            }

            var student = _db.Students.FirstOrDefault(s => s.StudentId == updatedStudent.StudentId);
            if (student == null)
            {
                TempData["Error"] = "Không tìm thấy học viên.";
                return RedirectToAction("Index", "StudentsManager");
            }

          
            student.FullName = updatedStudent.FullName;
            student.DateOfBirth = updatedStudent.DateOfBirth;
            student.Gender = updatedStudent.Gender;
            student.PhoneNumber = updatedStudent.PhoneNumber;
            student.Email = updatedStudent.Email;

           
            if (imageFile != null && imageFile.Length > 0)
            {
                string imgPath = await MyUtil.UploadImg(imageFile, "StudentImg");
                if (!string.IsNullOrEmpty(imgPath))
                {
                    student.ImagePath = imgPath;
                }
                else
                {
                    ModelState.AddModelError("ImagePath", "Không thể tải lên hình ảnh.");
                    return View(student); // Trả về model đang xử lý
                }
            }

            _db.Update(student);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cập nhật thành công!";
            return RedirectToAction("Index", "StudentsManager");
        }
        #endregion
    }
}
