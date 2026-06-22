using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserPanelMvcAuth.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
