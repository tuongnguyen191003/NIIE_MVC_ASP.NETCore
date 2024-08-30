using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_FinalTerm.Models;
using MVC_FinalTerm.Repository.DataContext;
using MVC_FinalTerm.Repository.Sessions;

namespace MVC_FinalTerm.Controllers
{
    //[Authorize(Roles = "Customer, Blogger")]
    public class WishListController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly DataContext _context;

        public WishListController(UserManager<AppUserModel> userManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Add(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(); // Người dùng chưa đăng nhập
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound(); // Sản phẩm không tồn tại
            }

            // Kiểm tra xem sản phẩm đã có trong danh sách yêu thích chưa
            var existingItem = _context.WishListItems
                .FirstOrDefault(w => w.ProductId == productId && w.UserId == user.Id);

            if (existingItem != null)
            {
                TempData["Message"] = "Product is already in your wishlist.";
                return RedirectToAction("Index", "WishList"); // Hoặc trở về trang hiện tại nếu cần
            }

            string stockStatus = product.StockQuantity > 0 ? "In Stock" : "Out of Stock";

            var wishlistItem = new WishListItems
            {
                UserId = user.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Image = product.Image,
                StockStatus = stockStatus
            };

            _context.WishListItems.Add(wishlistItem);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Product added to your wishlist!";
            return RedirectToAction("Index", "Product"); // Hoặc trở về trang hiện tại nếu cần
        }


        //[HttpPost]
        //public async Task<IActionResult> Delete(int productId)
        //{
        //    var user = await _userManager.GetUserAsync(User);
        //    if (user == null)
        //    {
        //        return Unauthorized(); // Người dùng chưa đăng nhập
        //    }

        //    var wishlistItem = await _context.WishListItems
        //        .FirstOrDefaultAsync(w => w.ProductId == productId && w.UserId == user.Id);

        //    if (wishlistItem == null)
        //    {
        //        return NotFound("Product not found in your wishlist."); // Trả về thông báo nếu không tìm thấy sản phẩm trong wishlist
        //    }

        //    _context.WishListItems.Remove(wishlistItem);
        //    await _context.SaveChangesAsync();

        //    TempData["Success"] = "Product removed from your wishlist.";
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public async Task<IActionResult> Remove(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(); // Người dùng chưa đăng nhập
            }

            // Tìm sản phẩm trong wishlist dựa trên ProductId và UserId
            var wishlistItem = await _context.WishListItems
                .FirstOrDefaultAsync(w => w.ProductId == productId && w.UserId == user.Id);

            if (wishlistItem == null)
            {
                TempData["error"] = "Product not found in your wishlist."; // Thông báo lỗi nếu không tìm thấy sản phẩm
                return RedirectToAction("Index");
            }

            // Xóa sản phẩm khỏi wishlist
            _context.WishListItems.Remove(wishlistItem);
            await _context.SaveChangesAsync();

            TempData["success"] = "Product removed from your wishlist.";
            return RedirectToAction("Index");
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var wishlistItems = _context.WishListItems.Where(w => w.UserId == user.Id).ToList();
            ViewBag.WishlistCount = wishlistItems.Count;
            return View(wishlistItems);
        }

        public async Task<int> GetWishListCount()   
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                return await _context.WishListItems.CountAsync(w => w.UserId == user.Id);
            }
            return 0;
        }

    }
}
