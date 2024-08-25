using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

    namespace MVC_FinalTerm.Areas.Admin.Controllers
    {
        [Area("Admin")]
        [Route("Admin/Dashboard")]
        [Authorize(Roles = "Admin")]
        public class DashboardController : Controller
        {
        [Route("Index")]
        public IActionResult Index()
            {
                return View();
            }
        //// Action này chỉ dành cho Admin
        //public IActionResult RestrictedAction()
        //{
        //    // Kiểm tra nếu người dùng không phải là Admin, chuyển hướng đến trang NotFound của Home
        //    if (!User.IsInRole("Admin"))
        //    {
        //        return RedirectToAction("NotFound", "Home");
        //    }

        //    return View();
        //}
    }
    }
