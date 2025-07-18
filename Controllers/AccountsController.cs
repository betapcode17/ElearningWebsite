using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ElearningWebsite.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using ElearningWebsite.ViewModel;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using ElearningWebsite.Helpers;

namespace ElearningWebsite.Controllers
{
    public class AccountsController : Controller
    {
        private readonly LearningManagementSystemContext _db;
        private readonly ILogger<AccountsController> _logger;
      

        public AccountsController(LearningManagementSystemContext db, ILogger<AccountsController> logger)
        {
            _db = db;
            _logger = logger;
          
        }

        #region Đăng nhập
        [HttpGet]
        public IActionResult Login()
        {
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var account = await _db.Accounts
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Username == model.Username);

                if (account == null || account.Password != PasswordUtil.HashPassword(model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản hoặc mật khẩu không đúng");
                    return View(model);
                }
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString()),
            new Claim(ClaimTypes.Name, account.Username),
            new Claim(ClaimTypes.Role, account.Role.ToString())
        };

                if (account.Role == 0) 
                {
                    var student = await _db.Students
                        .AsNoTracking()
                        .FirstOrDefaultAsync(s => s.StudentId == account.UserId);

                    if (student != null)
                    {
                        claims.Add(new Claim("FullName", student.FullName));
                        claims.Add(new Claim("DateOfBirth", student.DateOfBirth.ToString("yyyy-MM-dd")));
                        claims.Add(new Claim("PhoneNumber", student.PhoneNumber));
                        claims.Add(new Claim("Email", student.Email));
                        if (!string.IsNullOrEmpty(student.ImagePath))
                        {
                            claims.Add(new Claim("ImagePath", student.ImagePath));
                        }
                        if (student.Gender.HasValue)
                        {
                            claims.Add(new Claim("Gender", student.Gender.Value.ToString()));
                        }
                    }
                }

                else if (account.Role == 1) 
                {
                    var admin = await _db.Admins
                        .AsNoTracking()
                        .FirstOrDefaultAsync(a => a.AdminId == account.UserId);

                    if (admin != null)
                    {
                        claims.Add(new Claim("FullName", admin.FullName));
                        claims.Add(new Claim("Email", admin.Email));

                        if (!string.IsNullOrEmpty(admin.ImagePath))
                        {
                            claims.Add(new Claim("ImagePath", admin.ImagePath));
                        }
                        
                    }
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(15)
                    });



                return account.Role == 0
                    ? RedirectToAction("Index", "Home")
                    : RedirectToAction("Statistical", "Statistical", new { area = "Admin" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi đăng nhập");
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đăng nhập");
                return View(model);
            }
        }
        #endregion

        #region Trang cá nhân
        [Authorize]
        public IActionResult Profile()
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(studentId))
            {
                return RedirectToAction("Login", "Account");
            }
            int enrollmentCount = _db.Enrollments
                                     .Where(e => e.Progress == 0 || e.Progress == null)
                                     .Count(e => e.StudentId == studentId); 
                                 
            ViewBag.EnrollmentCount = enrollmentCount;

            int CompletedCourse = _db.Enrollments
                                     .Where(e=>e.Progress == 100)
                                     .Count(e => e.StudentId == studentId);
            ViewBag.CompletedCourse = CompletedCourse;


            int InProgessCourse = _db.Enrollments
                                      .Where (e=>e.Progress <100 && e.Progress >0)
                                      .Count(e=>e.StudentId == studentId);
            ViewBag.InProgessCourse = InProgessCourse;

            return View();
        }
        #endregion
        #region Đăng kí
        [HttpGet]
        public IActionResult Register()
        {
           
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }

            if (_db.Accounts.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("UserName", "Tên đăng nhập đã tồn tại");
                return View(model);
            }

            try
            {

                var lastAccount = _db.Accounts
                                 .OrderByDescending(a => a.UserId)
                                  .FirstOrDefault();
                int newIdNumber = 1;
                if (lastAccount != null && lastAccount.UserId.Length > 1)
                {
                    string numberPart = lastAccount.UserId.Substring(1);
                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        newIdNumber = lastNumber + 1;
                    }
                }
                string UserId = $"U{newIdNumber:D3}";
                model.UserId = string.IsNullOrEmpty(UserId)
                    ? "U001"
                    : UserId;

                model.Role = 0;

                var account = new Account
                {
                    UserId = model.UserId,
                    Username = model.Username,
                    Password = PasswordUtil.HashPassword(model.Password),
                    Role = 0,
                };

                var student = new Student
                {
                    StudentId = account.UserId,
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber ?? string.Empty,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                };

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
                        return View(model);
                    }
                }

                _db.Accounts.Add(account);
                _db.Students.Add(student);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi đăng ký: {Message} | StackTrace: {StackTrace}", ex.Message, ex.StackTrace);
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đăng ký: " + ex.Message);
                return View(model);
            }
        }
        #endregion

        #region Đăng xuất
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        #endregion
        #region Quên mật khẩu
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Vui lòng nhập email.";
                return RedirectToAction("ForgotPassword");
            }
            var student = _db.Students.FirstOrDefault(s => s.Email == email);
            if (student == null)
            {
                TempData["Error"] = "Email không tồn tại trong hệ thống.";
                return RedirectToAction("ForgotPassword");
            }
            var account = _db.Accounts.FirstOrDefault(a => a.UserId == student.StudentId);
            if (account == null)
            {
                TempData["Error"] = "Không tìm thấy tài khoản tương ứng với sinh viên.";
                return RedirectToAction("ForgotPassword");
            }
            string newPassword = SendEmail.GenerateRandomPassword();
            account.Password = PasswordUtil.HashPassword(newPassword);
            _db.Accounts.Update(account);
            _db.SaveChanges();
            bool emailSent = SendEmail.SendNewPasswordEmail(email, newPassword);
            if (emailSent)
            {
                TempData["Success"] = "Mật khẩu mới đã được gửi tới email của bạn.";
            }
            else
            {
                TempData["Error"] = "Không thể gửi email. Vui lòng thử lại sau.";
            }
            return RedirectToAction("Login");
        }
        #endregion

        #region Thay đổi thông tin
        [HttpGet]
        public IActionResult UpdateProfile()
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
            {
                return RedirectToAction("Login", "Account");
            }

            var student = _db.Students.FirstOrDefault(s => s.StudentId == studentId);
            var account = _db.Accounts.FirstOrDefault(a => a.UserId == studentId);

            if (student == null || account == null)
            {
                return NotFound("Không tìm thấy thông tin người dùng.");
            }

            var model = new RegisterVM
            {
                UserId = student.StudentId,
                Username = account.Username,
                FullName = student.FullName,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                DateOfBirth = student.DateOfBirth,
                Gender = student.Gender,
                ImagePath = student.ImagePath
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateProfile(RegisterVM model, IFormFile? imageFile)
        {
            if (string.IsNullOrEmpty(model.Password))
                ModelState.Remove("Password");

            if (string.IsNullOrEmpty(model.ConfirmPassword))
                ModelState.Remove("ConfirmPassword");

            if (string.IsNullOrEmpty(model.Username))
                ModelState.Remove("Username");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }

            try
            {
                var student = _db.Students.FirstOrDefault(s => s.StudentId == model.UserId);
                var account = _db.Accounts.FirstOrDefault(a => a.UserId == model.UserId);

                if (student == null || account == null)
                {
                    ModelState.AddModelError("", "Không tìm thấy người dùng.");
                    return View(model);
                }

                if (!string.IsNullOrEmpty(model.Username) && _db.Accounts.Any(a => a.Username == model.Username && a.UserId != model.UserId))
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại.");
                    return View(model);
                }

                if (!string.IsNullOrEmpty(model.Username))
                {
                    account.Username = model.Username;
                }

                if (!string.IsNullOrEmpty(model.Password))
                {
                    account.Password = PasswordUtil.HashPassword(model.Password);
                }

              
                if (!string.IsNullOrEmpty(model.FullName))
                {
                    student.FullName = model.FullName;
                }

                if (!string.IsNullOrEmpty(model.Email))
                {
                    student.Email = model.Email;
                }

                if (!string.IsNullOrEmpty(model.PhoneNumber))
                {
                    student.PhoneNumber = model.PhoneNumber;
                }

                if (model.DateOfBirth != default(DateOnly))
                {
                    student.DateOfBirth = model.DateOfBirth;
                }

                if (model.Gender != default(int))
                {
                    student.Gender = model.Gender;
                }

               
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
                        return View(model);
                    }
                }

                _db.Update(account);
                _db.Update(student);
                await _db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Profile", "Accounts");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật thông tin");
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi cập nhật thông tin.");
                return View(model);
            }
        }
        #endregion
    }
}

