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

        public CategoryController(DataContext context)
        {
            _context = context;
        }

        // GET: Admin/Category
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách các đối tượng CategoryModel từ cơ sở dữ liệu
            var categories = await _context.Categories.ToListAsync();

            // Truyền danh sách categories và _context sang view
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
        public async Task<IActionResult> Create([Bind("CategoryId,Name,Description,Slug,Status")] CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                //code thêm dữ liệu
                category.Slug = category.Name.Replace(" ", "-");
                category.Description = category.Description.Replace("<p>", "").Replace("</p>", "").Replace("<br>", "\n");
                var slug = await _context.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Category exists already");
                    return View(category);
                }
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["success"] = "Add successfully";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model đang có một vài vấn đề";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            return View(category);
        }

        // GET: Admin/Category/Edit/5
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            CategoryModel category = await _context.Categories.FindAsync(id);
            if (category == null) // Kiểm tra xem category có tồn tại hay không
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,Name,Description,Slug,Status")] CategoryModel category)
        {
            // Tìm category trong database dựa vào Id
            var existedCategory = await _context.Categories.FindAsync(id);

            if (existedCategory == null) // Kiểm tra xem category có tồn tại hay không
            {
                return NotFound(); // Trả về trang 404 nếu không tìm thấy
            }

            // Kiểm tra model state
            if (ModelState.IsValid)
            {
                // Tạo slug mới từ name
                existedCategory.Slug = existedCategory.Name.Replace(" ", "-");

                // Kiểm tra slug có trùng với slug hiện tại hay không
                var slug = await _context.Categories.FirstOrDefaultAsync(p => p.Slug == category.Slug && p.CategoryId != id);

                if (slug != null)
                {
                    ModelState.AddModelError("", "Category exists already");
                    return View(existedCategory);
                }

                // Kiểm tra xem có thay đổi nào không
                if (existedCategory.Name == category.Name &&
                    existedCategory.Description == category.Description &&
                    existedCategory.Status == category.Status)
                {
                    TempData["success"] = "No changes were made.";
                    return RedirectToAction("Index");
                }

                // Cập nhật thông tin category
                existedCategory.Name = category.Name;
                existedCategory.Description = category.Description;
                existedCategory.Status = category.Status;

                _context.Update(existedCategory);
                await _context.SaveChangesAsync();
                TempData["success"] = "Update successfully";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Model đang có một vài vấn đề";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            return View(existedCategory);
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


        public async Task<IActionResult> Delete(int id)
        {
            CategoryModel category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            // Kiểm tra xem category này có được sử dụng bởi bất kỳ sản phẩm nào không
            var productsUsingCategory = _context.Products.Where(p => p.CategoryId == id).ToList();
            if (productsUsingCategory.Any())
            {
                // Nếu có sản phẩm sử dụng category này, hiển thị thông báo lỗi
                TempData["error"] = "This category cannot be removed because it is being used by several products.";
                return RedirectToAction("Index");
            }
            else
            {
                // Nếu không có sản phẩm sử dụng, xóa category
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["success"] = "Remove Successfully!";
                return RedirectToAction("Index");
            }
        }

        private bool CategoryModelExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }
    }
}