using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_FinalTerm.Models;
using MVC_FinalTerm.Models.ViewModels;
using MVC_FinalTerm.Repository.DataContext;
using MVC_FinalTerm.Repository.Sessions;

namespace MVC_FinalTerm.Controllers
{
    //[Authorize(Roles = "Customer, Blogger")]
    public class CartController : Controller
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IActionResult Index()
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemViewModel cartVM = new()
            {
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price)
            };
            return View(cartVM);
        }
        
        public async Task<IActionResult> Add(int productId)
        {
            var product = await _dataContext.Products.FindAsync(productId);
            if (product == null)
            {
                TempData["error"] = "Product not found.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (cartItem == null)
            {
                cart.Add(new CartItemModel(product));
            }
            else
            {
                cartItem.Quantity += 1;
            }
            HttpContext.Session.SetJson("Cart", cart);

            TempData["success"] = "Product added to cart successfully";

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Decrease(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(p => p.ProductId == Id);
            }

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            TempData["success"] = "Decrease product to cart Successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Increase(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            CartItemModel cartItem = cart.Where(c => c.ProductId == Id).FirstOrDefault();
            if (cartItem.Quantity >= 1)
            {
                ++cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(p => p.ProductId == Id);
            }

            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }
            TempData["success"] = "Increase product to cart Successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Remove(int Id)
        {
            List<CartItemModel> cart = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            cart.RemoveAll((p) => p.ProductId == Id);
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            TempData["success"] = "Remove your cart Successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Clear(int Id)
        {
            HttpContext.Session.Remove("Cart");
            TempData["success"] = "Clear your cart Successfully";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(string coupon)
        {
            // Lấy danh sách sản phẩm trong giỏ hàng từ Session
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();

            // Tìm mã giảm giá trong cơ sở dữ liệu
            CouponModel couponModel = await _dataContext.Coupons
                .FirstOrDefaultAsync(c => c.Code == coupon && c.IsActive && c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now);

            // Kiểm tra tính hợp lệ của mã giảm giá
            if (couponModel == null)
            {
                // Nếu mã giảm giá không hợp lệ hoặc hết hạn
                TempData["error"] = "Mã giảm giá không hợp lệ hoặc đã hết hạn!";
                TempData["coupon"] = coupon; // Để giữ lại mã coupon đã nhập

                // Tính toán lại tổng giá trị giỏ hàng
                decimal grandTotal = cartItems.Sum(x => x.Quantity * x.Price);

                // Trả về View với thông tin giỏ hàng hiện tại
                CartItemViewModel cartVM = new()
                {
                    CartItems = cartItems,
                    GrandTotal = grandTotal,
                    DiscountAmount = 0 // Không có giảm giá
                };
                return View("Index", cartVM);
            }
            else
            {
                // Áp dụng mã giảm giá
                decimal discountAmount = 0;
                foreach (var item in cartItems)
                {
                    decimal itemDiscount = item.Price * item.Quantity * (couponModel.Discount / 100);
                    item.Discount = itemDiscount; // Cập nhật giá trị giảm giá cho từng sản phẩm
                    discountAmount += itemDiscount;
                }

                // Tính toán lại tổng giá trị giỏ hàng sau khi áp dụng mã giảm giá
                decimal grandTotal = cartItems.Sum(x => x.Quantity * x.Price) - discountAmount;

                // Lưu thông tin giỏ hàng đã áp dụng mã giảm giá vào Session
                HttpContext.Session.SetJson("Cart", cartItems);

                // Trả về View với thông tin giỏ hàng đã cập nhật
                CartItemViewModel cartVM = new()
                {
                    CartItems = cartItems,
                    GrandTotal = grandTotal,
                    DiscountAmount = discountAmount // Giá trị giảm giá đã áp dụng
                };
                TempData["success"] = "Áp dụng mã giảm giá thành công!";
                TempData["coupon"] = coupon; // Để giữ lại mã coupon đã nhập
                return View("Index", cartVM);
            }
        }
        public IActionResult ProceedToCheckout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Lưu giỏ hàng vào Session trước khi chuyển hướng
                HttpContext.Session.SetJson("TempCart", HttpContext.Session.GetJson<List<CartItemModel>>("Cart"));

                // Chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Checkout") });
            }

            return RedirectToAction("Index", "Checkout");
        }


    }
}
