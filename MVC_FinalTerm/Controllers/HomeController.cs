using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_FinalTerm.Models;
using MVC_FinalTerm.Repository.DataContext;
using System.Diagnostics;

namespace MVC_FinalTerm.Controllers
{
    //[Authorize(Roles = "Customer, Blogger")]
    public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
        private readonly DataContext _context;
        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // G?i ph??ng th?c ?? l?y d? li?u s?n ph?m m?i
            var newArrivals = await GetNewArrivals();
            return View(newArrivals);
        }

        private async Task<List<ProductModel>> GetNewArrivals()
        {
            return await _context.Products
                .Where(p => p.StatusName == "Hot" || p.StatusName == "New" || p.StatusName == "Sales")
                .Include(p => p.Brand)
                .Include(p => p.Color)
                .Include(p => p.Ram)
                .Include(p => p.Rom)
                .ToListAsync();
        }

        public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error(int statuscode)
		{
			if(statuscode == 404)
			{
				return View("NotFound");
			} else
			{
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
			
		}
	}
}
