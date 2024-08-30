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
            var products = await _context.Products
         .Include(p => p.Reviews)
         .ToListAsync();

            // Đảm bảo rằng Reviews không phải là null
            foreach (var product in products)
            {
                if (product.Reviews == null)
                {
                    product.Reviews = new List<ReviewModel>();
                }
            }

            return View(products);
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

        // Phương thức này để xử lý khi người dùng nhấn Enter hoặc nút Search
        [HttpGet]
        public async Task<IActionResult> SearchResults(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return View(new List<ProductModel>());
            }

            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Ram)
                .Include(p => p.Rom)
                .Include(p => p.Color)
                .Include(p => p.Reviews)
                .Where(p => p.Name.Contains(keyword) || p.Brand.Name.Contains(keyword) || p.CategoryId != null && p.Category.Name.Contains(keyword))
                .ToListAsync();

            return View("ProductResults", products);
        }
        [HttpPost]
        public async Task<IActionResult> QuickSearch(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return PartialView("_SearchResults", new List<ProductModel>());
            }

            var products = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Where(p => p.Name.Contains(keyword) || p.Brand.Name.Contains(keyword) || p.CategoryId != null && p.Category.Name.Contains(keyword))
                .Take(5)
                .ToListAsync();

            return PartialView("_SearchResults", products);
        }
    }
}
