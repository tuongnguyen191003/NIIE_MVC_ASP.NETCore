using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Để truy vấn database
using MVC_FinalTerm.Models.ViewModels;
using MVC_FinalTerm.Repository.DataContext; // Giả sử bạn có một DataContext để truy vấn database

namespace MVC_FinalTerm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Dashboard")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly DataContext _context;
        public DashboardController(DataContext context)
        {
            _context = context;
        }
        [Route("Index")]
        public IActionResult Index()
        {
            // Lấy dữ liệu từ database
            var productCount = _context.Products.Count();
            var userCount = _context.Users.Count();
            var orderCount = _context.Orders.Count();
            var totalRevenue = _context.Orders.Sum(o => o.TotalAmount);

            // Tính toán doanh thu theo tháng
            var monthlyRevenue = _context.Orders
                .Where(o => o.OrderDate >= DateTime.Now.AddMonths(-1))
                .GroupBy(o => o.OrderDate.Month)
                .Select(g => new MonthlyRevenue { Month = g.Key, Revenue = g.Sum(o => o.TotalAmount) })
                .OrderBy(g => g.Month)
                .ToList();

            // Lấy danh sách số lượng sản phẩm theo category
            var categoryProductCounts = _context.Categories
                  .Select(c => new CategoryProductCount { CategoryName = c.Name, ProductCount = _context.Products.Where(p => p.CategoryId == c.CategoryId).Count() })
                  .ToList();

            // Lấy danh sách user
            var users = _context.Users
                .Select(u => new UserViewModel
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber
                })
                .ToList();

            // Tạo ViewModel
            var viewModel = new DashboardViewModel
            {
                ProductCount = productCount,
                UserCount = userCount,
                OrderCount = orderCount,
                TotalRevenue = totalRevenue,
                MonthlyRevenue = monthlyRevenue,
                CategoryProductCounts = categoryProductCounts,
                Users = users
            };

            return View(viewModel);
        }
    }
}
