using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_FinalTerm.Models;
using MVC_FinalTerm.Models.ViewModels;
using MVC_FinalTerm.Repository.DataContext;
using MVC_FinalTerm.Repository.Sessions;
using System.Net.WebSockets;


namespace MVC_FinalTerm.Controllers
{
    //[Authorize(Roles = "Customer, Blogger")]
    public class ProductController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _context;

        public ProductController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var products = _context.Products.Include(p => p.Reviews).ToList();
            var productCount = products.Count; // Lấy số lượng sản phẩm

            // Truyền số lượng sản phẩm vào view
            ViewData["ProductCount"] = productCount;
            return View(products);
        }
        public async Task<IActionResult> Details(int Id)
        {
            if (Id == null) return NotFound();
            var product = await _context.Products
                .Include(p => p.Reviews)
                .Include(p => p.Ram)
                .Include(p => p.Rom)
                .Include(p => p.Color)
                .Include(p => p.ProductImages)
                .Include(p => p.DetailDescriptions)
                .Include(p => p.ProductSpecs)
                .FirstOrDefaultAsync(p => p.Id == Id);
            if (product == null) return NotFound();
            // Tạo ProductDetailsViewModel
            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                Quantity = 1,
                TotalPrice = product.Price
            };

            // Lấy danh sách sản phẩm cùng Series
            viewModel.RelatedProducts = await _context.Products
                //.Where(p => p.SeriesId == product.SeriesId && p.Id != product.Id)
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id)// Lọc sản phẩm cùng Category và khác ID hiện tại
                .Take(4) // Lấy tối đa 4 sản phẩm liên quan
                .ToListAsync();

            return View(viewModel);
        }

        //[HttpPost]
        //public async Task<IActionResult> AddReview(int productId, string name, string email, int rating, string content)
        //{
        //    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(content))
        //    {
        //        return BadRequest("Vui lòng điền đầy đủ thông tin.");
        //    }

        //    var review = new ReviewModel
        //    {
        //        ProductId = productId,
        //        ReviewerName = name,
        //        Email = email,
        //        Rating = rating,
        //        Content = content
        //    };

        //    _context.Reviews.Add(review);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("Details", new { Id = productId });
        //}


        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, string content, int rating)
        {
            // Kiểm tra xem người dùng đã đăng nhập chưa
            if (!User.Identity.IsAuthenticated)
            {
                // Chuyển hướng đến trang đăng nhập và lưu lại URL hiện tại để quay lại sau khi đăng nhập
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Details", "Product", new { Id = productId }) });
            }

            // Lấy thông tin người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);

            // Kiểm tra tính hợp lệ của thông tin người dùng và nội dung đánh giá
            if (user == null || string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(content) || rating <= 0)
            {
                return BadRequest("Vui lòng điền đầy đủ thông tin.");
            }

            var review = new ReviewModel
            {
                ProductId = productId,
                ReviewerName = user.UserName,
                Email = user.Email,
                Rating = rating,
                Content = content,
                AuthorImage = $"~/frontend/images/profiles/{user.UserName}.jpg"
            };

            // Thêm review vào cơ sở dữ liệu
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            // Chuyển hướng quay lại trang chi tiết sản phẩm sau khi thêm review thành công
            return RedirectToAction("Details", new { Id = productId });
        }


        // tìm kiếm --------------------------------------tìm kiếm-------------------------tìm kiếm
        public async Task<IActionResult> Search(string search)
        {
            var products = await _context.Products.Where(p => p.Name.Contains(search) || p.Description.Contains(search)).ToListAsync();

            ViewBag.Keyword = search;

            return View(products);
        }

        //-------------------------------------------

        //public async Task<IActionResult> Filter(decimal? minPrice, decimal? maxPrice, string sortOrder)
        //{
        //    // Khởi tạo query
        //    var productsQuery = _context.Products.AsQueryable();

        //    // Áp dụng các bộ lọc về giá
        //    if (minPrice.HasValue)
        //    {
        //        productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
        //    }

        //    if (maxPrice.HasValue)
        //    {
        //        productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
        //    }

        //    // Sắp xếp theo giá
        //    switch (sortOrder)
        //    {
        //        case "price_asc":
        //            productsQuery = productsQuery.OrderBy(p => p.Price);
        //            break;
        //        case "price_desc":
        //            productsQuery = productsQuery.OrderByDescending(p => p.Price);
        //            break;
        //        default:
        //            productsQuery = productsQuery.OrderBy(p => p.Name);
        //            break;
        //    }

        //    // Lấy danh sách sản phẩm sau khi lọc
        //    var products = await productsQuery.ToListAsync();

        //    return View("Search", products); // Render lại view Search với dữ liệu đã lọc
        //}

        public class ProductController : Controller
        {
            private readonly ApplicationDbContext _context;

            public ProductController(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<IActionResult> Filter(decimal? minPrice, decimal? maxPrice, string sortOrder, int? minRam, int? maxRam, int? minStorage, int? maxStorage)
            {
                // Khởi tạo query
                var productsQuery = _context.Products.Include(p => p.Ram).Include(p => p.Rom).AsQueryable();

                // Áp dụng các bộ lọc về giá
                if (minPrice.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
                }

                if (maxPrice.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
                }

                // Áp dụng bộ lọc RAM
                if (minRam.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.Ram.RAMSize >= minRam.Value);
                }

                if (maxRam.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.Ram.RAMSize <= maxRam.Value);
                }

                // Áp dụng bộ lọc dung lượng lưu trữ
                if (minStorage.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.Rom.StorageSize >= minStorage.Value);
                }

                if (maxStorage.HasValue)
                {
                    productsQuery = productsQuery.Where(p => p.Rom.StorageSize <= maxStorage.Value);
                }

                // Sắp xếp theo giá
                switch (sortOrder)
                {
                    case "price_asc":
                        productsQuery = productsQuery.OrderBy(p => p.Price);
                        break;
                    case "price_desc":
                        productsQuery = productsQuery.OrderByDescending(p => p.Price);
                        break;
                    default:
                        productsQuery = productsQuery.OrderBy(p => p.Name);
                        break;
                }

                // Lấy danh sách sản phẩm sau khi lọc
                var products = await productsQuery.ToListAsync();

                return View("Search", products); // Render lại view Search với dữ liệu đã lọc
            }
        }





    }
}
