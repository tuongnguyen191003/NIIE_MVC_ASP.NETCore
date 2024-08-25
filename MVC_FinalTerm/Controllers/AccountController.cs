using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC_FinalTerm.Models;
using MVC_FinalTerm.Models.ViewModels;
using MVC_FinalTerm.Repository.DataContext;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MVC_FinalTerm.Repository.Sessions;
using Microsoft.EntityFrameworkCore;

namespace MVC_FinalTerm.Controllers
{
    //[Authorize(Roles = "Customer, Blogger")]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly SignInManager<AppUserModel> _signInManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly DataContext _context;

        public AccountController(UserManager<AppUserModel> userManager, SignInManager<AppUserModel> signInManager, DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Signup(SignupViewModel signupVM)
        {
            if (ModelState.IsValid)
            {
                // Nếu FullName không được cung cấp, sử dụng Username làm FullName
                string fullName = string.IsNullOrEmpty(signupVM.FullName) ? signupVM.Username : signupVM.FullName;

                var user = new AppUserModel
                {
                    UserName = signupVM.Username,
                    Email = signupVM.Email,
                    FullName = fullName
                };
                var result = await _userManager.CreateAsync(user, signupVM.Password);

                if (result.Succeeded)
                {
                    TempData["success"] = "Signup successful!";
                    return RedirectToAction("Login"); // Chuyển hướng đến trang đăng nhập sau khi đăng ký thành công
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(signupVM);
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Tìm user theo username
                var user = await _userManager.FindByNameAsync(loginVM.Username);
                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(loginVM);
                }

                // Kiểm tra role của user
                var userRoles = await _userManager.GetRolesAsync(user);
                var result = await _signInManager.PasswordSignInAsync(loginVM.Username, loginVM.Password, loginVM.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    TempData["success"] = "Login successful!";
                    
                    // Chuyển hướng theo role của user hoặc theo returnUrl nếu có
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    if (userRoles.Contains("Customer") || userRoles.Count == 0)
                    {
                        TempData["success"] = "Login successful!";
                        return RedirectToAction("Index", "Home");
                    }
                    else if (userRoles.Contains("Admin"))
                    {
                        TempData["success"] = "Login successful!";
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    else if (userRoles.Contains("Publisher"))
                    {
                        TempData["success"] = "Login successful!";
                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                    }
                    else if (userRoles.Contains("Blogger"))
                    {
                        TempData["success"] = "Login successful!";
                        return RedirectToAction("Index", "Blogger", new { area = "Admin" });
                    }
                    else
                    {
                        ModelState.AddModelError("", "You do not have access to this area.");
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }

                ModelState.AddModelError("", "Invalid login attempt.");
            }

            return View(loginVM);
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _signInManager.SignOutAsync();
                TempData["success"] = "You have been logged out.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Log the exception details here
                TempData["error"] = "An error occurred while logging out. Please try again.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new AccountViewModel
            {
                Username = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                DateOfBirth = user.DateOfBirth,
                ProfileImage = user.ProfileImage ?? "/frontend/images/default-user.png"
            };

            ViewBag.CurrentUser = await _userManager.GetUserAsync(User);
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = new EditProfileViewModel
            {
                FullName = user.FullName,
                DateOfBirth = user.DateOfBirth,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                City = user.City,
                CurrentProfileImage = user.ProfileImage ?? "/images/default-user.png"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Update user details
                user.FullName = model.FullName;
                user.DateOfBirth = model.DateOfBirth;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address;
                user.City = model.City;

                // Handle image upload
                if (model.ProfileImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "frontend/images/profiles");
                    string imageName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ProfileImageUpload.FileName);
                    string filePath = Path.Combine(uploadDir, imageName);

                    // Ensure directory exists
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    // Optional: Delete old picture if necessary
                    if (!string.IsNullOrEmpty(user.ProfileImage))
                    {
                        string oldFilePath = Path.Combine(uploadDir, Path.GetFileName(user.ProfileImage));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            try
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError("", "An error occurred while deleting the old profile image.");
                                // Optionally log the exception
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }

                    // Save new image
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfileImageUpload.CopyToAsync(stream);
                    }
                    user.ProfileImage = "/frontend/images/profiles/" + imageName;
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["success"] = "Profile updated successfully!";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            else
            {
                TempData["error"] = "There are some issues with the model.";
                // Aggregate errors
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                ModelState.AddModelError("", errorMessage);
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    await _signInManager.RefreshSignInAsync(user);
                    TempData["success"] = "Password changed successfully!";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
        
        [HttpPost]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Account");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        
        public async Task<IActionResult> GoogleResponse()
        {
            var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                // Nếu không lấy được thông tin đăng nhập từ Google, chuyển hướng đến trang Login
                return RedirectToAction(nameof(Login));
            }

            // Thực hiện đăng nhập với thông tin từ Google
            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                externalLoginInfo.LoginProvider,
                externalLoginInfo.ProviderKey,
                isPersistent: false,
                bypassTwoFactor: true);

            if (signInResult.Succeeded)
            {
                // Nếu đăng nhập thành công, chuyển hướng đến trang Home/Index
                return RedirectToAction("Index", "Home");
            }

            if (signInResult.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }

            // Nếu người dùng không có tài khoản, tạo tài khoản mới bằng thông tin từ Google
            var email = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);

            // Lấy tên đầy đủ từ Google
            var fullName = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name);

            // Nếu không có tên đầy đủ, tự động tạo từ email
            if (string.IsNullOrEmpty(fullName))
            {
                fullName = email.Substring(0, email.IndexOf('@'));
            }


            var user = new AppUserModel { UserName = email, Email = email, FullName = fullName };
            var resultCreate = await _userManager.CreateAsync(user);
            if (resultCreate.Succeeded)
            {
                await _userManager.AddLoginAsync(user, externalLoginInfo);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Nếu có lỗi, chuyển hướng đến trang Login và hiển thị thông báo lỗi
            return RedirectToAction(nameof(Login));
        }
        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            // Lấy thông tin người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Truy vấn danh sách đơn hàng của người dùng
            var orders = await _context.Orders
                .Include(o => o.OrderDetails) // Bao gồm cả thông tin chi tiết đơn hàng
                .Where(o => o.UserId == user.Id)
                .ToListAsync();

            return View(orders);
        }
        [HttpGet]
        public async Task<IActionResult> OrderDetails(int id)
        {
            // Truy vấn đơn hàng và chi tiết đơn hàng theo Id
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null || order.UserId != _userManager.GetUserId(User))
            {
                return NotFound();
            }

            return View(order);
        }

    }
}
