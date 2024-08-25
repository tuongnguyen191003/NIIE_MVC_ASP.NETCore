using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVC_FinalTerm.Models;
using MVC_FinalTerm.Repository.DataContext;

namespace MVC_FinalTerm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/ProImages")]
    [Authorize(Roles = "Admin,Publisher")]
    public class ProImagesController : Controller
    {
        
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProImagesController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        // GET: Admin/ProImages
        [Route("Index")]
        public async Task<IActionResult> Index()
        {
            var productImages = await _context.ProductImages.Include(pi => pi.Product).ToListAsync();
            return View(productImages);
        }

        // GET: Admin/ProImages/Create
        [Route("Create")]
        public IActionResult Create()
        {
            ViewBag.ProductId = new SelectList(_context.Products, "Id", "Name");
            return View();
        }

        // POST: Admin/ProImages/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(int productId, List<IFormFile> ImageUploads)
        {
            if (ImageUploads == null || !ImageUploads.Any())
            {
                ModelState.AddModelError("", "Please upload at least one image.");
                ViewBag.ProductId = new SelectList(_context.Products, "Id", "Name", productId);
                return View();
            }

            foreach (var imageUpload in ImageUploads)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "frontend/images/products");
                string imageName = Guid.NewGuid().ToString() + "_" + imageUpload.FileName;
                string filePath = Path.Combine(uploadDir, imageName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await imageUpload.CopyToAsync(fs);
                }

                var productImage = new ProductImage
                {
                    ProductId = productId,
                    Name = imageName
                };
                _context.ProductImages.Add(productImage);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Images added successfully!";
            return RedirectToAction("Index");
        }

        // GET: Admin/ProImages/Edit
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }

            ViewBag.ProductId = new SelectList(_context.Products, "Id", "Name", productImage.ProductId);
            return View(productImage);
        }

        // POST: Admin/ProImages/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id, ProductImage productImage, IFormFile ImageUpload)
        {
            if (id != productImage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "frontend/images/products");
                    string imageName = Guid.NewGuid().ToString() + "_" + ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    // Delete old image
                    string oldFileImage = Path.Combine(uploadDir, productImage.Name);
                    if (System.IO.File.Exists(oldFileImage))
                    {
                        System.IO.File.Delete(oldFileImage);
                    }

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageUpload.CopyToAsync(fs);
                    }

                    productImage.Name = imageName;
                }

                _context.Update(productImage);
                await _context.SaveChangesAsync();
                TempData["success"] = "Image updated successfully!";
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(_context.Products, "Id", "Name", productImage.ProductId);
            return View(productImage);
        }

        // GET: Admin/ProImages/Details
        [Route("Details")]
        public async Task<IActionResult> Details(int id)
        {
            var productImage = await _context.ProductImages.Include(pi => pi.Product)
                                                           .FirstOrDefaultAsync(pi => pi.Id == id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // GET: Admin/ProImages/Delete
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage == null)
            {
                return NotFound();
            }

            return View(productImage);
        }

        // POST: Admin/ProImages/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Route("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productImage = await _context.ProductImages.FindAsync(id);
            if (productImage != null)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "frontend/images/products");
                string filePath = Path.Combine(uploadDir, productImage.Name);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                _context.ProductImages.Remove(productImage);
                await _context.SaveChangesAsync();
                TempData["success"] = "Image deleted successfully!";
            }

            return RedirectToAction("Index");
        }   
    }
}
