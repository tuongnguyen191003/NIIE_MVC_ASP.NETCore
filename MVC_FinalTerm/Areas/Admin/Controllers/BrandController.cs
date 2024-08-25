using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_FinalTerm.Models;
using MVC_FinalTerm.Repository.DataContext;

namespace MVC_FinalTerm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Brand")]
    [Authorize(Roles = "Admin,Publisher")]
    public class BrandController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BrandController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: Admin/Brand
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách các đối tượng BrandModel từ cơ sở dữ liệu
            var brands = await _context.Brands.ToListAsync();

            // Truyền danh sách brands và _context sang view
            return View(brands);
        }

        // GET: Admin/Brand/Create
        [Route("Create")]
        public IActionResult Create()
        {
            return View();
        }
        // POST: Admin/Brand/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Slug,Status,Image,ImageUpload")] BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                //code thêm dữ liệu
                brand.Slug = brand.Name.Replace(" ", "-");
                brand.Description = brand.Description.Replace("<p>", "").Replace("</p>", "").Replace("<br>", "\n");
                var slug = await _context.Brands.FirstOrDefaultAsync(p => p.Slug == brand.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Brand exists already");
                    return View(brand);
                }
                if (brand.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "frontend/images/brands");
                    string imageName = Guid.NewGuid().ToString() + "_" + brand.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await brand.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    brand.Image = imageName;
                }
                _context.Add(brand);
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
            return View(brand);
        }

        // GET: Admin/Brand/Edit/5
        [Route("Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            BrandModel brand = await _context.Brands.FindAsync(id);
            if (brand == null) // Kiểm tra xem brand có tồn tại hay không
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Admin/Brand/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Slug,Status,Image,ImageUpload")] BrandModel brand)
        {
            // Tìm brand trong database dựa vào Id
            var existedBrand = await _context.Brands.FindAsync(id);

            if (existedBrand == null) // Kiểm tra xem brand có tồn tại hay không
            {
                return NotFound(); // Trả về trang 404 nếu không tìm thấy
            }

            // Kiểm tra model state
            if (ModelState.IsValid)
            {
                // Tạo slug mới từ name
                existedBrand.Slug = existedBrand.Name.Replace(" ", "-");

                // Kiểm tra slug có trùng với slug hiện tại hay không
                var slug = await _context.Brands.FirstOrDefaultAsync(p => p.Slug == brand.Slug && p.Id != id);

                if (slug != null)
                {
                    ModelState.AddModelError("", "Category exists already");
                    return View(existedBrand);
                }

                if (existedBrand.ImageUpload != null)
                {

                    //upload new image
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "frontend/images/brands");
                    string imageName = Guid.NewGuid().ToString() + "_" + existedBrand.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    //delete old picture
                    string oldFileImage = Path.Combine(uploadDir, existedBrand.Image);

                    try
                    {
                        if (System.IO.File.Exists(oldFileImage))
                        {
                            System.IO.File.Delete(oldFileImage);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An error occured while deleting the brand image");
                    }

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await brand.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    existedBrand.Image = imageName;
                }


                // Kiểm tra xem có thay đổi nào không
                if (existedBrand.Name == brand.Name &&
                    existedBrand.Description == brand.Description &&
                    existedBrand.Status == brand.Status)
                {
                    TempData["success"] = "No changes were made.";
                    return RedirectToAction("Index");
                }

                // Cập nhật thông tin category
                existedBrand.Name = brand.Name;
                existedBrand.Description = brand.Description;
                existedBrand.Status = brand.Status;

                _context.Update(existedBrand);
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
            return View(existedBrand);
        }
        // GET: Admin/Brand/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brandModel = await _context.Brands.FirstOrDefaultAsync(m => m.Id == id);
            if (brandModel == null)
            {
                return NotFound();
            }

            return View(brandModel);
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            BrandModel brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            // Kiểm tra xem thương hiệu này có được sử dụng bởi bất kỳ sản phẩm nào không
            var productsUsingBrand = _context.Products.Where(p => p.BrandId == id).ToList();
            if (productsUsingBrand.Any())
            {
                // Nếu có sản phẩm sử dụng thương hiệu này, hiển thị thông báo lỗi
                TempData["error"] = "This brand cannot be removed because it is being used by several products.";
                return RedirectToAction("Index");
            }
            else
            {
                // Nếu không có sản phẩm sử dụng, xóa thương hiệu
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
                TempData["success"] = "Remove Successfully!";
                return RedirectToAction("Index");
            }
        }

        private bool BrandModelExists(int id)
        {
            return _context.Brands.Any(e => e.Id == id);
        }
    }
}
