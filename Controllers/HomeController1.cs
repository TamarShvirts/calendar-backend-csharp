using Microsoft.AspNetCore.Mvc;

namespace calendarProject.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
