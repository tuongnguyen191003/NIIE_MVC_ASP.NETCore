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
        // GET: Admin/Brand
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách các đối tượng BrandModel từ cơ sở dữ liệu
            var categories = await _context.Categories.ToListAsync();

            // Truyền danh sách brands và _context sang view
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
        }

        // GET: Admin/Brand/Edit/5
        [Route("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            CategoryModel categories = await _context.Categories.FindAsync(id);
            if (categories == null) // Kiểm tra xem brand có tồn tại hay không
            {
                return NotFound();
            }
            return View(categories);
        }

        // POST: Admin/Brand/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id, CategoryModel category)
        {
            // Tìm category trong database dựa vào Id
            var existedCategory = await _context.Categories.FindAsync(id);

            if (existedCategory == null) // Kiểm tra xem brand có tồn tại hay không
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
                await  _context.SaveChangesAsync();
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
        }
        // GET: Admin/Brand/Details/5
        [Route("Details")]
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
            // Kiểm tra xem thương hiệu này có được sử dụng bởi bất kỳ sản phẩm nào không
            var productsUsingBrand = _context.Products.Where(p => p.CategoryId == id).ToList();
            if (productsUsingBrand.Any())
            {
                // Nếu có sản phẩm sử dụng thương hiệu này, hiển thị thông báo lỗi
                TempData["error"] = "This category cannot be removed because it is being used by several products.";
                return RedirectToAction("Index");
            }
            else
            {
                // Nếu không có sản phẩm sử dụng, xóa thương hiệu
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["success"] = "Remove Successfully!";
                return RedirectToAction("Index");
            }
        }

    }
}
