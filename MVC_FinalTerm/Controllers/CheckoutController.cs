using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_FinalTerm.Helper;
using MVC_FinalTerm.Models;
using MVC_FinalTerm.Models.ViewModels;
using MVC_FinalTerm.Repository.DataContext;
using MVC_FinalTerm.Repository.Sessions;
using MVC_FinalTerm.Service.Momo;
using MVC_FinalTerm.Services.VnPay;
using System.Security.Claims;

namespace MVC_FinalTerm.Controllers
{
    //[Authorize(Roles = "Customer, Blogger")]
    public class CheckoutController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly PaypalClient _paypalClient;
        private readonly DataContext _dataContext;
        private readonly IVnPayService _vnPayService;
        private readonly IMomoService _momoService;
        public CheckoutController(DataContext dataContext, UserManager<AppUserModel> userManager, PaypalClient paypalClient, IVnPayService vnPayService, IMomoService momoService)
        {
            _paypalClient = paypalClient;
            _dataContext = dataContext;
            _userManager = userManager;
            _vnPayService = vnPayService;
            _momoService = momoService;
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
                Email = user.Email,
                Telephone = user.PhoneNumber
            };
            ViewBag.PaypalClientId = "ARbnxJTLtxb0Wruyhkx1NxF99RproEX0R-xncYpGXNzHv9ZqNhRwxMnt-Dp7flNFOUfrU4ckDDexMV9h";

            return View(checkoutVM);

        }
        [HttpPost]
        public async Task<IActionResult> PaypalOrder(CancellationToken cancellationToken)
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            // Tạo đơn hàng (thông tin lấy từ Session???)
            var tongTien = cartItems.Sum(p => p.Total).ToString();
            var donViTienTe = "USD";
            // OrderId - mã tham chiếu (duy nhất)
            var orderIdref = "DH" + DateTime.Now.Ticks.ToString();

            try
            {
                // a. Create paypal order
                var response = await _paypalClient.CreateOrder(tongTien, donViTienTe, orderIdref);

                return Ok(response);
            }
            catch (Exception e)
            {
                var error = new
                {
                    e.GetBaseException().Message
                };

                return BadRequest(error);
            }
        }
        public async Task<IActionResult> PaypalCapture(string orderId, CancellationToken cancellationToken, EditProfileViewModel model)
        {
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            {
                try
                {
                    var response = await _paypalClient.CaptureOrder(orderId);
                    var user = await _userManager.GetUserAsync(User);
                    //nhớ kiểm tra status complete
                    if (response.status == "COMPLETED")
                    {
                        var reference = response.purchase_units[0].reference_id;//mã đơn hàng mình tạo ở trên
                        var transactionId = response.purchase_units[0]?.payments?.captures?.FirstOrDefault()?.id ?? string.Empty;
                        // Put your logic to save the transaction here
                        // You can use the "reference" variable as a transaction key
                        // 1. Tạo và Lưu đơn hàng vô database
                        // TransactionId của Seller: response.payments.captures[0].id
                        var hoaDon = new OrderModel
                        {
                            FirstName = user.Email,
                            LastName = user.Email,
                            FullName = user.FullName,
                            Address = user.Address, // Cần thay đổi nếu có thông tin address thực tế
                            Email = user.Email ?? string.Empty,
                            Telephone = user.PhoneNumber ?? string.Empty,
                            PaymentMethod = "PayPal",
                            OrderDate = DateTime.Now,
                            UserId = user.Id,
                            TotalAmount = cartItems.Sum(x => x.Quantity * x.Price),
                            Note = $"reference_id={"PayPal"}, transactionId={"PayPal"}",
                            TransactionId = "PayPal",
                            OrderDetails = new List<OrderDetails>()
                        };
                        _dataContext.Add(hoaDon);
                        _dataContext.SaveChanges();
                        foreach (var item in cartItems)
                        {
                            var orderDetails = new OrderDetails
                            {
                                OrderId = hoaDon.Id,
                                ProductId = item.ProductId,
                                ProductName = item.ProductName ?? "Unknown",
                                Price = item.Price,
                                Quantity = item.Quantity,
                                Discount = item.Discount
                            };
                            _dataContext.Add(orderDetails);
                        }
                        _dataContext.SaveChanges();
                        //2. Xóa session giỏ hàng
                        //HttpContext.Session.Set(CART_KEY, new List<>());

                        return Ok(response);
                    }
                    else
                    {
                        return BadRequest(new { Message = "Có lỗi thanh toán" });
                    }
                }
                catch (Exception e)
                {
                    var error = new
                    {
                        e.GetBaseException().Message
                    };

                    return BadRequest(error);
                }
            }
        }
        public IActionResult Success()
        {
            return View();
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
                FullName = user.FullName,
                Address = model.Address,
                Email = model.Email,
                Telephone = model.Telephone,
                PaymentMethod = model.PaymentMethod,
                OrderDate = DateTime.Now,
                UserId = user.Id,// Liên kết đơn hàng với người dùng
                OrderDetails = new List<OrderDetails>(),
                TotalAmount = totalAmount, // Gán TotalAmount
                Note = $"reference_id={"COD"}, transactionId={"COD"}",
                TransactionId = "COD",
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
        [HttpGet]
        public IActionResult PaymentCallbackVnpay()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            return Json(response);
        }
        [HttpGet]
        public async Task<IActionResult> PaymentCallBack(CheckoutViewModel model)
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            var requestQuery = HttpContext.Request.Query;
            var user = await _userManager.GetUserAsync(User);
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart");
            if (requestQuery["resultCode"] != 0)
            {
                var order = new OrderModel
                {
                    FirstName = user.Email,
                    LastName = user.Email,
                    FullName = user.FullName,
                    Address = user.Address, // Cần thay đổi nếu có thông tin address thực tế
                    Email = user.Email ?? string.Empty,
                    Telephone = user.PhoneNumber ?? string.Empty,
                    PaymentMethod = "Momo",
                    OrderDate = DateTime.Now,
                    UserId = user.Id,
                    TotalAmount = cartItems.Sum(x => x.Quantity * x.Price),
                    Note = $"reference_id={"PayPal"}, transactionId={"PayPal"}",
                    TransactionId = "PayPal",
                    OrderDetails = new List<OrderDetails>()
                };
                _dataContext.Add(order);
                await _dataContext.SaveChangesAsync();
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

            }

            else
            {
                TempData["success"] = "Da huy giao dich Momo";
                return RedirectToAction("Index", "Cart");
            }
            //var checkoutResult = await Checkout(requestQuery["orderId"]);
            return View(response);
        }



    }


}
