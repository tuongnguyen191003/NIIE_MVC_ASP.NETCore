using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_FinalTerm.Models;
using MVC_FinalTerm.Models.ViewModels;
using MVC_FinalTerm.Repository.DataContext;
using MVC_FinalTerm.Repository.Sessions;

namespace MVC_FinalTerm.Controllers
{
    //[Authorize(Roles = "Customer, Blogger")]
    public class CheckoutController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly DataContext _dataContext;
        public CheckoutController(DataContext dataContext, UserManager<AppUserModel> userManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Checkout") });
            }

            // Khôi phục giỏ hàng từ Session hoặc Cookie nếu có
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            var user = _userManager.GetUserAsync(User).Result;

            // Giả sử FullName có định dạng "FirstName LastName"
            var fullNameParts = user.FullName?.Split(' ');
            var firstName = fullNameParts?.FirstOrDefault();
            var lastName = fullNameParts?.Skip(1).FirstOrDefault();

            
            // Tạo CheckoutViewModel
            CheckoutViewModel checkoutVM = new CheckoutViewModel()
            {
                //CartItems = cartItems ?? new List<CartItemModel>(),  // Đảm bảo CartItems luôn được khởi tạ
                CartItems = cartItems,
                GrandTotal = cartItems.Sum(x => x.Quantity * x.Price),
                FirstName = firstName,
                LastName = lastName,
                Address = user.Address,
                Town = user.City,
                Email = user.Email,
                Telephone = user.PhoneNumber
            };

            return View(checkoutVM);
        }
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            // Kiểm tra nếu giỏ hàng trống
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            if (cartItems == null || !cartItems.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return View("Index", model);
            }

            // Lấy thông tin người dùng đang đăng nhập
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Checkout") });
            }

            // Tính toán TotalAmount
            decimal totalAmount = cartItems.Sum(x => x.Quantity * x.Price);

            // Tạo đơn hàng mới
            OrderModel order = new OrderModel()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Address = model.Address,
                Town = model.Town,
                Email = model.Email,
                Telephone = model.Telephone,
                PaymentMethod = model.PaymentMethod,
                OrderDate = DateTime.Now,
                UserId = user.Id ,// Liên kết đơn hàng với người dùng
                OrderDetails = new List<OrderDetails>(),
                TotalAmount = totalAmount // Gán TotalAmount
            };

            // Thêm đơn hàng vào cơ sở dữ liệu
            _dataContext.Orders.Add(order);
            await _dataContext.SaveChangesAsync();

            // Thêm chi tiết đơn hàng vào cơ sở dữ liệu
            foreach (var item in cartItems)
            {
                OrderDetails orderDetails = new OrderDetails()
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    Discount = item.Discount
                };

                order.OrderDetails.Add(orderDetails); // Thêm chi tiết vào danh sách
                _dataContext.OrderDetails.Add(orderDetails);
            }
            await _dataContext.SaveChangesAsync();

            // Xóa giỏ hàng sau khi thanh toán
            HttpContext.Session.Remove("Cart");
            // Thiết lập thông báo thành công
            TempData["success"] = "Your order has been placed successfully!";

            // Chuyển hướng đến trang xác nhận đơn hàng
            return RedirectToAction("Confirm", new { orderId = order.Id });
        }

        public IActionResult Confirm(int orderId)
        {
            // Lấy thông tin đơn hàng từ database và bao gồm cả OrderDetails
            var order = _dataContext.Orders
                                    .Include(o => o.OrderDetails) // Load OrderDetails
                                    .FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}
