using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MVC_FinalTerm.Models;
using MVC_FinalTerm.Repository.DataContext;

namespace MVC_FinalTerm.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/ProDetail")]
    [Authorize(Roles = "Admin,Publisher")]
    public class ProDetailController : Controller
    {
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProDetailController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        //// GET: Admin/ProDetail
        //[Route("Index")]
        //public async Task<IActionResult> Index()
        //{
        //    var details = await _context.DetailDescriptions.Include(d => d.Product).ToListAsync();
        //    return View(details);
        //}
        // GET: Admin/ProDetail
        [Route("Index")]
        public async Task<IActionResult> Index(int productId)
        {
            // Lấy danh sách các mô tả chi tiết dựa trên ProductId
            var details = await _context.DetailDescriptions
            .Include(d => d.Product)
                .Where(d => d.ProductId == productId)
                .ToListAsync();

            // Nếu không có mô tả chi tiết nào, trả về một danh sách trống
            if (details == null || !details.Any())
            {
                details = new List<DetailDescription>();
            }

            ViewBag.ProductId = productId;
            return View(details);
        }

        // GET: Admin/ProDetail/Create
        [Route("Create")]
        public IActionResult Create(int productId)
        {
            ViewBag.ProductId = productId;
            return View();
        }

        // POST: Admin/ProDetail/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(DetailDescription model)
        {
            if (ModelState.IsValid)
            {
                if (model.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "frontend/images/descriptions");
                    string imageName = Guid.NewGuid().ToString() + "_" + model.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageUpload.CopyToAsync(fs);
                    }
                    model.Image = imageName;
                }

                _context.DetailDescriptions.Add(model);
                await _context.SaveChangesAsync();
                TempData["success"] = "Detail added successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "There was an issue adding the detail";
            return View(model);
        }

        // GET: Admin/ProDetail/Edit
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var detail = await _context.DetailDescriptions.FindAsync(id);
            if (detail == null)
            {
                return NotFound();
            }
            return View(detail);
        }

        // POST: Admin/ProDetail/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit")]
        public async Task<IActionResult> Edit(int id, DetailDescription model)
        {
            if (ModelState.IsValid)
            {
                var detail = await _context.DetailDescriptions.FindAsync(id);
                if (detail == null)
                {
                    return NotFound();
                }

                if (model.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "frontend/images/descriptions");
                    string imageName = Guid.NewGuid().ToString() + "_" + model.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);

                    if (System.IO.File.Exists(Path.Combine(uploadDir, detail.Image)))
                    {
                        System.IO.File.Delete(Path.Combine(uploadDir, detail.Image));
                    }

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageUpload.CopyToAsync(fs);
                    }
                    detail.Image = imageName;
                }

                detail.Content = model.Content;
                detail.VideoUrl = model.VideoUrl;

                _context.Update(detail);
                await _context.SaveChangesAsync();
                TempData["success"] = "Detail updated successfully";
                return RedirectToAction("Index");
            }
            TempData["error"] = "There was an issue updating the detail";
            return View(model);
        }

        // GET: Admin/ProDetail/Details
        [Route("Details")]
        public async Task<IActionResult> Details(int id)
        {
            var detail = await _context.DetailDescriptions.Include(d => d.Product).FirstOrDefaultAsync(d => d.Id == id);
            if (detail == null)
            {
                return NotFound();
            }
            return View(detail);
        }

        // GET: Admin/ProDetail/Delete
        [Route("Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var detail = await _context.DetailDescriptions.FindAsync(id);
            if (detail == null)
            {
                return NotFound();
            }
            return View(detail);
        }

        // POST: Admin/ProDetail/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detail = await _context.DetailDescriptions.FindAsync(id);
            if (detail != null)
            {
                string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "frontend/images/descriptions");
                if (System.IO.File.Exists(Path.Combine(uploadDir, detail.Image)))
                {
                    System.IO.File.Delete(Path.Combine(uploadDir, detail.Image));
                }

                _context.DetailDescriptions.Remove(detail);
                await _context.SaveChangesAsync();
                TempData["success"] = "Detail removed successfully";
            }
            return RedirectToAction("Index");
        }
    }
}
