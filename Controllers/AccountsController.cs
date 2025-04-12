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

        // GET: Accounts
        #region Đăng nhập
        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string returnUrl = null)
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

                if (account == null || account.Password != model.Password)
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

                if (account.Role == 0) // Student
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
                else if (account.Role == 1) // Admin
                {
                    var admin = await _db.Admins
                        .AsNoTracking()
                        .FirstOrDefaultAsync(a => a.AdminId == account.UserId);

                    if (admin != null)
                    {
                        claims.Add(new Claim("FullName", admin.FullName));
                        claims.Add(new Claim("Email", admin.Email));
                        claims.Add(new Claim("ImagePath", admin.ImagePath));
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
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                    });

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return account.Role == 0
                    ? RedirectToAction("Index", "Home")
                    : RedirectToAction("Index", "HomeAdmin", new { area = "Admin" });
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
        public IActionResult Register(RegisterVM model, IFormFile ProfileImg)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage); // Hoặc ghi log
                }
                return View(model);
            }
            else
            {
                if (_db.Accounts.Any(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("UserName", "Tên Đăng nhập đã tồn tại");
                    return View(model);
                }
                try
                {
                    var maxUserId = _db.Accounts
                      .OrderByDescending(a => a.UserId)
                      .Select(a => a.UserId)
                      .FirstOrDefault();

                    model.UserId = string.IsNullOrEmpty(maxUserId)
                        ? "U001"
                        : $"U{(int.Parse(maxUserId.Substring(1)) + 1).ToString("D3")}";
                    model.Role = 0;
                    var account = new Account
                    {
                        UserId =model.UserId, 
                        Username = model.Username,
                        Password = model.Password, 
                        Role = 0,                        
                    };

                    var student = new Student
                    {
                        StudentId = account.UserId, 
                        FullName = model.FullName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        DateOfBirth = model.DateOfBirth,
                        Gender = model.Gender,                         
                    };
                    if (ProfileImg != null)
                    {
                       string imgPath =  MyUtil.UploadImg(ProfileImg, "StudentImg");
                        student.ImagePath = imgPath;
                    }

                   
                  
                    _db.Add(account);
                    _db.Add(student);
                    _db.SaveChanges();

                    return RedirectToAction("Index","Home");
                }
                catch (Exception ex)
                {                
                    _logger.LogError(ex, "Lỗi đăng nhập");
                    ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi đăng nhập");
                    return View(model);
                }
            }
        }
        #endregion

        #region Đăng xuất
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home"); // Chuyển hướng về trang chủ
        }
        #endregion
    }
}
