using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ElearningWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using X.PagedList.Extensions;

namespace ElearningWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("[area]/[controller]/[action]")]
    public class AccountsManagerController : Controller
    {
        private readonly LearningManagementSystemContext _db;

        public AccountsManagerController(LearningManagementSystemContext db)
        {
            _db = db;
        }
        [Authorize(Roles = "1")]
        public IActionResult Index(int? page, string searchValue)
        {
          
            var query = _db.Accounts.AsNoTracking();
            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(s => s.Username.Contains(searchValue));
                                    
            }
            int pageSize = 8;
            int pageNumber = page ?? 1;
            var pagedAccounts = query
                .OrderBy(s => s.UserId) 
                .ToPagedList(pageNumber, pageSize);
            return View(pagedAccounts);
        }
        #region Đăng xuất
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
          
            return RedirectToAction("Index", "Home", new { area = "" });
        }
        #endregion


    }
}
