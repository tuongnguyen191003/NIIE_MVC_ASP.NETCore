using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_FinalTerm.Models;
using MVC_FinalTerm.Repository.DataContext;

namespace MVC_FinalTerm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Product")]
    [Authorize(Roles = "Admin,Publisher")]
    public class ProductController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: Admin/Product
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Color)
                .Include(p => p.Ram)
                .Include(p => p.Rom)
                .ToListAsync());
        }

        // GET: Admin/Product/Create
        [Route("Create")]
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "Name");
            ViewData["RamId"] = new SelectList(_context.Rams, "RamId", "Value");
            ViewData["RomId"] = new SelectList(_context.Roms, "RomId", "Value");
            return View();
        }

        // POST: Admin/Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(
            [Bind("Id,Name,Description,Slug,Price,OldPrice,ColorId,StockQuantity,StatusName,IsOnStatus,BrandId,Image,ImageUpload,RamId,RomId,CategoryId")]
            ProductModel product)
        {

            ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            ViewBag.ColorId = new SelectList(_context.Colors, "ColorId", "Name", product.ColorId);
            ViewBag.RomId = new SelectList(_context.Roms, "RomId", "Value", product.RomId);
            ViewBag.RamId = new SelectList(_context.Rams, "RamId", "Value", product.RamId);
            if (ModelState.IsValid)
            {
                product.Slug = product.Name.ToLower().Replace(" ", "-");
                product.Description = product.Description.Replace("<p>", "").Replace("</p>", "").Replace("<br>", "\n");
                //product.Price = decimal.Parse(product.Price.ToString().Replace(",", "."));
                var slug = await _context.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "Product exists already");
                    return View(product);
                }
                if (product.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "frontend/images/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.Image = imageName;
                }
                _context.Add(product);
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

        [HttpGet]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int Id)
        {
            ProductModel product = await _context.Products.FindAsync(Id);
            ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            ViewBag.ColorId = new SelectList(_context.Colors, "ColorId", "Name", product.ColorId);
            ViewBag.RomId = new SelectList(_context.Roms, "RomId", "Value", product.RomId);
            ViewBag.RamId = new SelectList(_context.Rams, "RamId", "Value", product.RamId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int Id, ProductModel product)
        {

            ViewBag.BrandId = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            ViewBag.ColorId = new SelectList(_context.Colors, "ColorId", "Name", product.ColorId);
            ViewBag.RomId = new SelectList(_context.Roms, "RomId", "Value", product.RomId);
            ViewBag.RamId = new SelectList(_context.Rams, "RamId", "Value", product.RamId);
            var existed_product = await _context.Products.FindAsync(Id);
            // Kiểm tra sản phẩm có tồn tại không
            if (existed_product == null)
            {
                // Nếu không tồn tại, trả về lỗi hoặc thông báo thích hợp
                TempData["error"] = "Product not found.";
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                existed_product.Slug = existed_product.Name.Replace(" ", "-");

                if (existed_product.ImageUpload != null)
                {

                    //upload new image
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "frontend/images/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + existed_product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    //delete old picture
                    string oldFileImage = Path.Combine(uploadDir, existed_product.Image);

                    try
                    {
                        if (System.IO.File.Exists(oldFileImage))
                        {
                            System.IO.File.Delete(oldFileImage);
                        }
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An error occured while deleting the product image");
                    }

                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    existed_product.Image = imageName;
                }

                existed_product.Name = product.Name;
                existed_product.Description = product.Description;
                existed_product.Price = product.Price;
                existed_product.OldPrice = product.OldPrice;
                existed_product.ColorId = product.ColorId;
                existed_product.StockQuantity = product.StockQuantity;
                existed_product.StatusName = product.StatusName;
                existed_product.IsOnStatus = product.IsOnStatus;
                existed_product.CategoryId = product.CategoryId;
                existed_product.RamId = product.RamId;
                existed_product.RomId = product.RomId;
                existed_product.BrandId = product.BrandId;

                _context.Update(existed_product);
                await _context.SaveChangesAsync();
                TempData["success"] = "Updated successfully";
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


            return View(product);
        }

        // GET: Admin/Product/Details/5
        [Route("Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productModel = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Color)
                .Include(p => p.Ram)
                .Include(p => p.Rom)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productModel == null)
            {
                return NotFound();
            }

            return View(productModel);
        }

        public async Task<IActionResult> Delete(int Id)
        {
            ProductModel product = await _context.Products.FindAsync(Id);
            if (product == null)
            {
                return NotFound();
            }

            string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "frontend/images/products");

            // Xóa hình ảnh nếu có
            if (!string.IsNullOrEmpty(product.Image))
            {
                string oldFileImage = Path.Combine(uploadDir, product.Image);

                try
                {
                    if (System.IO.File.Exists(oldFileImage))
                    {
                        System.IO.File.Delete(oldFileImage);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while deleting the product image");
                }
            }

            // Xóa sản phẩm khỏi database
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["success"] = "Deleted successfully"; // Sử dụng TempData["success"] cho thông báo thành công
            return RedirectToAction("Index");
        }
    }
}
