//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using MVC_FinalTerm.Models;
//using MVC_FinalTerm.Models.ViewModels;
//using MVC_FinalTerm.Repository.DataContext;
//using System.Linq;

//namespace MVC_FinalTerm.Areas.Admin.Controllers
//{
//    [Area("Admin")]
//    [Authorize(Roles = "Admin")]
//    [Route("Admin/UserRole")]
//    public class UserRoleController : Controller
//    {
//        private readonly UserManager<AppUserModel> _userManager;
//        private readonly RoleManager<IdentityRole> _roleManager;
//        private readonly DataContext _context;

//        public UserRoleController(UserManager<AppUserModel> userManager,
//                                   RoleManager<IdentityRole> roleManager,
//                                   DataContext context)
//        {
//            _userManager = userManager;
//            _roleManager = roleManager;
//            _context = context;
//        }

//        // GET: Admin/UserRole
//        [Route("Index")]
//        public async Task<IActionResult> Index()
//        {
//            // Lấy danh sách UserRole từ AspNetUserRoles
//            var userRoles = await _context.UserRoles
//                .ToListAsync();

//            // Tạo danh sách UserRoleViewModel từ dữ liệu AspNetUserRoles
//            var userRoleViewModels = userRoles.Select(ur => new UserRoleViewModel
//            {
//                UserId = ur.UserId,
//                // Truy vấn thông tin User dựa vào UserId
//                UserName = (await _userManager.FindByIdAsync(ur.UserId))?.UserName,
//                RoleId = ur.RoleId,
//                // Truy vấn thông tin Role dựa vào RoleId
//                RoleName = (await _roleManager.FindByIdAsync(ur.RoleId))?.Name
//            }).ToList();

//            // Truyền danh sách UserRoleViewModel cho View
//            return View(userRoleViewModels);
//        }

//        // GET: Admin/UserRole/Create
//        [Route("Create")]
//        public async Task<IActionResult> Create()
//        {
//            ViewData["UserId"] = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName");
//            ViewData["RoleId"] = new SelectList(await _roleManager.Roles.ToListAsync(), "Id", "Name");
//            return View();
//        }

//        // POST: Admin/UserRole/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [Route("Create")]
//        public async Task<IActionResult> Create(string UserId, string RoleId)
//        {
//            var user = await _userManager.FindByIdAsync(UserId);
//            var role = await _roleManager.FindByIdAsync(RoleId);

//            if (user != null && role != null)
//            {
//                var result = await _userManager.AddToRoleAsync(user, role.Name);
//                if (result.Succeeded)
//                {
//                    TempData["success"] = "Role assigned to user successfully.";
//                    return RedirectToAction("Index");
//                }
//                foreach (var error in result.Errors)
//                {
//                    ModelState.AddModelError("", error.Description);
//                }
//            }
//            else
//            {
//                ModelState.AddModelError("", "Invalid user or role.");
//            }

//            ViewData["UserId"] = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName", UserId);
//            ViewData["RoleId"] = new SelectList(await _roleManager.Roles.ToListAsync(), "Id", "Name", RoleId);
//            return View();
//        }

//        // GET: Admin/UserRole/Edit/5
//        [Route("Edit/{userId}/{roleId}")]
//        public async Task<IActionResult> Edit(string userId, string roleId)
//        {
//            var user = await _userManager.FindByIdAsync(userId);
//            var role = await _roleManager.FindByIdAsync(roleId);

//            if (user == null || role == null)
//            {
//                return NotFound();
//            }

//            // Tạo một ViewModel để truyền thông tin User và Role
//            var viewModel = new UserRoleViewModel
//            {
//                UserId = userId,
//                RoleId = roleId,
//                AvailableRoles = new SelectList(await _roleManager.Roles.ToListAsync(), "Id", "Name")
//            };

//            return View(viewModel);
//        }

//        // POST: Admin/UserRole/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [Route("Edit/{userId}/{roleId}")]
//        public async Task<IActionResult> Edit(string userId, string roleId, string newRoleId)
//        {
//            var user = await _userManager.FindByIdAsync(userId);
//            var oldRole = await _roleManager.FindByIdAsync(roleId);
//            var newRole = await _roleManager.FindByIdAsync(newRoleId);

//            if (user == null || oldRole == null || newRole == null)
//            {
//                return NotFound();
//            }

//            if (newRoleId != roleId)
//            {
//                // Xóa quyền cũ
//                await _userManager.RemoveFromRoleAsync(user, oldRole.Name);

//                // Gán quyền mới
//                var result = await _userManager.AddToRoleAsync(user, newRole.Name);
//                if (result.Succeeded)
//                {
//                    TempData["success"] = "User role updated successfully.";
//                    return RedirectToAction("Index");
//                }
//                foreach (var error in result.Errors)
//                {
//                    ModelState.AddModelError("", error.Description);
//                }
//            }
//            else
//            {
//                TempData["success"] = "No changes were made.";
//                return RedirectToAction("Index");
//            }

//            ViewData["UserId"] = userId;
//            ViewData["RoleId"] = newRoleId;

//            return View();
//        }

//        // GET: Admin/UserRole/Delete/5
//        [Route("Delete/{userId}/{roleId}")]
//        public async Task<IActionResult> Delete(string userId, string roleId)
//        {
//            var user = await _userManager.FindByIdAsync(userId);
//            var role = await _roleManager.FindByIdAsync(roleId);

//            if (user == null || role == null)
//            {
//                return NotFound();
//            }

//            // Xóa quyền khỏi User
//            var result = await _userManager.RemoveFromRoleAsync(user, role.Name);
//            if (result.Succeeded)
//            {
//                TempData["success"] = "User role deleted successfully.";
//                return RedirectToAction("Index");
//            }
//            foreach (var error in result.Errors)
//            {
//                ModelState.AddModelError("", error.Description);
//            }

//            return RedirectToAction("Index");
//        }
//    }
//}