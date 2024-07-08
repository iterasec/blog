using RedirectTest.Filters;
using Microsoft.AspNetCore.Mvc;

namespace RedirectTest.Controllers
{
    [Authentication]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
