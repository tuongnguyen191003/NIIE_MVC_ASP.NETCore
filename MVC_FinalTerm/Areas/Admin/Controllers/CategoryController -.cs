using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_FinalTerm.Models;
using MVC_FinalTerm.Repository.DataContext;

namespace MVC_FinalTerm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Category")]
    [Authorize(Roles = "Admin,Publisher")]
    public class CategoryController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Category/Index
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        // GET: Admin/Category/Create
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.Replace(" ", "-").ToLower();
                category.Description = category.Description.Replace("<p>", "").Replace("</p>", "").Replace("<br>", "\n");

                var slugExists = await _context.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
                if (slugExists != null)
                {
                    ModelState.AddModelError("", "Category already exists.");
                    return View(category);
                }

                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["success"] = "Category added successfully.";
                return RedirectToAction("Index");
            }

            TempData["error"] = "There are some errors in the model.";
            return View(category);
        }

        // GET: Admin/Category/Edit/5
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, CategoryModel category)
        {
            if (id != category.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existedCategory = await _context.Categories.FindAsync(id);
                if (existedCategory == null)
                {
                    return NotFound();
                }

                // Generate a slug from the name
                var newSlug = category.Name.Replace(" ", "-").ToLower();

                // Check if another category with the same slug exists
                var slugExists = await _context.Categories.FirstOrDefaultAsync(p => p.Slug == newSlug && p.CategoryId != id);
                if (slugExists != null)
                {
                    ModelState.AddModelError("", "Another category with the same slug already exists.");
                    return View(category);
                }

                // Update the category details
                existedCategory.Name = category.Name;
                existedCategory.Description = category.Description.Replace("<p>", "").Replace("</p>", "").Replace("<br>", "\n");
                existedCategory.Status = category.Status;
                existedCategory.Slug = newSlug;

                try
                {
                    _context.Update(existedCategory);
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Category updated successfully.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(existedCategory.CategoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            TempData["error"] = "There are some errors in the model.";
            return View(category);
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }

        // GET: Admin/Category/Details/5
        [Route("Details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categoryModel = await _context.Categories.FirstOrDefaultAsync(m => m.CategoryId == id);
            if (categoryModel == null)
            {
                return NotFound();
            }

            return View(categoryModel);
        }

        // GET: Admin/Category/Delete/5
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var productsUsingCategory = _context.Products.Where(p => p.CategoryId == id).ToList();
            if (productsUsingCategory.Any())
            {
                TempData["error"] = "This category cannot be removed because it is being used by several products.";
                return RedirectToAction("Index");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            TempData["success"] = "Category removed successfully.";
            return RedirectToAction("Index");
        }
    }
}
