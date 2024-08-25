using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_FinalTerm.Repository.DataContext;

namespace MVC_FinalTerm.Controllers
{
    //[Authorize(Roles = "Customer, Blogger")]
    public class BannerController : Controller
    {
        private readonly DataContext _context;
        public BannerController(DataContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var banner = _context.Banners.ToList();
            return View(banner);
        }
    }
}
