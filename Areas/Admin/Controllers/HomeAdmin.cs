using Microsoft.AspNetCore.Mvc;

namespace ElearningWebsite.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeAdmin : Controller
    {
        [Route("Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
