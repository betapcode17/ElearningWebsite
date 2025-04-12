using Microsoft.AspNetCore.Mvc;

namespace ElearningWebsite.Areas.Admin.Controllers
{
    public class EnrollmentManager : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
