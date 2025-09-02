using Microsoft.AspNetCore.Mvc;

namespace calendarProject.Controllers
{
    public class HomeController2 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
