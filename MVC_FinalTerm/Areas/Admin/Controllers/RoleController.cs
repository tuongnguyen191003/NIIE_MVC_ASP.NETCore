using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MVC_FinalTerm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [Route("Admin/Role")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: Admin/Role
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        // GET: Admin/Role/Details/5
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var identityRole = await _roleManager.FindByIdAsync(id);
            if (identityRole == null)
            {
                return NotFound();
            }

            return View(identityRole);
        }

        // GET: Admin/Role/Create
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(IdentityRole identityRole)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    TempData["success"] = "Role created successfully.";
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(identityRole);
        }

        // GET: Admin/Role/Edit/5
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var identityRole = await _roleManager.FindByIdAsync(id);
            if (identityRole == null)
            {
                return NotFound();
            }
            return View(identityRole);
        }

        // POST: Admin/Role/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id, IdentityRole identityRole)
        {
            if (id != identityRole.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(id);
                if (role == null)
                {
                    return NotFound();
                }

                role.Name = identityRole.Name;

                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    TempData["success"] = "Role updated successfully.";
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(identityRole);
        }

        // GET: Admin/Role/Delete/5
        [HttpGet]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                TempData["success"] = "Role deleted successfully.";
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
