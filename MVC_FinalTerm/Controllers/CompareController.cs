using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVC_FinalTerm.Controllers
{
    //[Authorize(Roles = "Customer, Blogger")]
    public class CompareController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
