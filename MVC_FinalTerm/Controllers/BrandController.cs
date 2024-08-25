using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_FinalTerm.Models;
using MVC_FinalTerm.Repository.DataContext;

namespace MVC_FinalTerm.Controllers
{
    //[Authorize(Roles = "Customer, Blogger")]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string Slug = "")
        {
            BrandModel  brand = _dataContext.Brands.Where(c => c.Slug == Slug).FirstOrDefault();
            if (brand == null) return RedirectToAction("Index");
            var productByBrands = _dataContext.Products.Include(p => p.Reviews).Where(p => p.BrandId == brand.Id);
            return View(await productByBrands.OrderByDescending(c => c.Id).ToListAsync());
        }
    }
}
