using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_FinalTerm.Models;
using MVC_FinalTerm.Repository.DataContext;

namespace MVC_FinalTerm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/User")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UserController(UserManager<AppUserModel> userManager,
                               RoleManager<IdentityRole> roleManager,
                               DataContext context,
                               IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: Admin/User
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }
        // GET: Admin/User/Details/5
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUserModel = await _userManager.FindByIdAsync(id);
            if (appUserModel == null)
            {
                return NotFound();
            }

            // Tạo đối tượng UserModel từ AppUserModel
            var userModel = new UserModel
            {
                Id = Guid.Parse(id), // Chuyển đổi chuỗi id sang int
                Username = appUserModel.UserName,
                Email = appUserModel.Email,
                FullName = appUserModel.FullName,
                PhoneNumber = appUserModel.PhoneNumber,
                Address = appUserModel.Address,
                City = appUserModel.City,
                DateOfBirth = appUserModel.DateOfBirth,
                ProfileImage = appUserModel.ProfileImage
            };

            return View(userModel);
        }
        // GET: Admin/User/Create
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(UserModel userModel)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUserModel
                {
                    UserName = userModel.Username,
                    Email = userModel.Email,
                    FullName = userModel.FullName,
                    PhoneNumber = userModel.PhoneNumber,
                    Address = userModel.Address,
                    City = userModel.City,
                    DateOfBirth = userModel.DateOfBirth,
                    ProfileImage = userModel.ProfileImage // Thêm thuộc tính ProfileImage vào UserModel
                };

                var result = await _userManager.CreateAsync(user, userModel.Password);
                if (result.Succeeded)
                {
                    TempData["success"] = "User created successfully.";
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(userModel);
        }
        // GET: Admin/User/Edit/5
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUserModel = await _userManager.FindByIdAsync(id);
            if (appUserModel == null)
            {
                return NotFound();
            }

            // Tạo đối tượng UserModel từ AppUserModel
            var userModel = new UserModel
            {
                // Chuyển đổi id từ chuỗi GUID sang Guid
                Id = Guid.Parse(id),
                Username = appUserModel.UserName,
                Email = appUserModel.Email,
                FullName = appUserModel.FullName,
                PhoneNumber = appUserModel.PhoneNumber,
                Address = appUserModel.Address,
                City = appUserModel.City,
                DateOfBirth = appUserModel.DateOfBirth,
                ProfileImage = appUserModel.ProfileImage
            };

            return View(userModel);
        }

        // POST: Admin/User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id, UserModel userModel)
        {
            // Sử dụng Guid.Parse() để so sánh id
            if (id != userModel.Id.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                user.FullName = userModel.FullName;
                user.PhoneNumber = userModel.PhoneNumber;
                user.Email = userModel.Email;
                user.Address = userModel.Address;
                user.City = userModel.City;
                user.DateOfBirth = userModel.DateOfBirth;

                // Xử lý cập nhật hình ảnh
                if (userModel.ProfileImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "frontend/images/profiles");
                    string imageName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(userModel.ProfileImageUpload.FileName);
                    string filePath = Path.Combine(uploadDir, imageName);

                    // Xóa hình ảnh cũ (nếu có)
                    if (!string.IsNullOrEmpty(user.ProfileImage))
                    {
                        string oldFilePath = Path.Combine(uploadDir, Path.GetFileName(user.ProfileImage));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    // Lưu hình ảnh mới
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await userModel.ProfileImageUpload.CopyToAsync(stream);
                    }

                    user.ProfileImage = "/frontend/images/profiles/" + imageName;
                }

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["success"] = "User updated successfully.";
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(userModel);
        }
        // POST: Admin/User/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var userId = Guid.Parse(id);

            var appUserModel = await _userManager.FindByIdAsync(userId.ToString());
            if (appUserModel == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(appUserModel);
            if (result.Succeeded)
            {
                TempData["success"] = "User deleted successfully.";
                return RedirectToAction("Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return RedirectToAction("Index");
        }
    }
}
