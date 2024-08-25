using Microsoft.EntityFrameworkCore;
using MVC_FinalTerm.Models;


namespace MVC_FinalTerm.Repository.DataContext
{
    public class DataSeeder
    {
        
        public static void SeedingData(DataContext _context)
        {
            _context.Database.Migrate();
            if (!_context.Brands.Any())
            {
                // Thêm Brands
                var brands = new List<BrandModel>
                {
                    new BrandModel { Name = "Apple", Description = "Apple", Slug = "apple", Status = "Active", Image = "apple.jpg" },
                    new BrandModel { Name = "Samsung", Description = "Samsung", Slug = "samsung", Status = "Active", Image = "samsung.jpg" },
                    new BrandModel { Name = "Xiaomi", Description = "Xiaomi", Slug = "xiaomi", Status = "Active", Image = "xiaomi.jpg" },
                    new BrandModel { Name = "Oppo", Description = "Oppo", Slug = "oppo", Status = "Active", Image = "oppo.jpg" },
                    new BrandModel { Name = "Asus", Description = "Asus", Slug = "asus", Status = "Active", Image = "asus.jpg" },
                    new BrandModel { Name = "Lenovo", Description = "Lenovo", Slug = "lenovo", Status = "Active", Image = "lenovo.jpg" },
                    new BrandModel { Name = "Dell", Description = "Dell", Slug = "dell", Status = "Active", Image = "dell.jpg" },
                    new BrandModel { Name = "HP", Description = "HP", Slug = "hp", Status = "Active", Image = "hp.jpg" },
                    new BrandModel { Name = "Acer", Description = "Acer", Slug = "acer", Status = "Active", Image = "acer.jpg" },
                    new BrandModel { Name = "JBL", Description = "JBL", Slug = "jbl", Status = "Active", Image = "jbl.jpg" },
                    new BrandModel { Name = "Marshall", Description = "Marshall", Slug = "marshall", Status = "Active", Image = "marshall.jpg" },
                    new BrandModel { Name = "Beats", Description = "Beats", Slug = "beats", Status = "Active", Image = "beats.jpg" },
                    new BrandModel { Name = "Huawei", Description = "Huawei", Slug = "huawei", Status = "Active", Image = "huawei.jpg" }
                };
                _context.Brands.AddRange(brands);
                // Thêm Colors
                var colors = new List<ColorModel>
                {
                    new ColorModel { Name = "Midnight", Code = "#000000" },
                    new ColorModel { Name = "Starlight", Code = "#FFFFFF" },
                    new ColorModel { Name = "Pink", Code = "#FFC0CB" },
                    new ColorModel { Name = "Red", Code = "#FF0000" },
                    new ColorModel { Name = "Blue", Code = "#0000FF" },
                    new ColorModel { Name = "Green", Code = "#008000" },
                    new ColorModel { Name = "Titan Đen", Code = "#212121" },
                    new ColorModel { Name = "Titan Trắng", Code = "#FFFFFF" },
                    new ColorModel { Name = "Titan Xanh", Code = "#00BCD4" },
                    new ColorModel { Name = "Titan Tự nhiên", Code = "#F44336" },
                    new ColorModel { Name = "Purple", Code = "#800080" },
                    new ColorModel { Name = "Yellow", Code = "#FFFF00" }
                };
                _context.Colors.AddRange(colors);

                // Thêm Rams
                var rams = new List<RamModel>
                {
                    new RamModel { Value = "4GB" },
                    new RamModel { Value = "6GB" },
                    new RamModel { Value = "8GB" },
                    new RamModel { Value = "12GB" },
                    new RamModel { Value = "16GB" }
                };
                _context.Rams.AddRange(rams);

                // Thêm Roms
                var roms = new List<RomModel>
                {
                    new RomModel { Value = "64GB" },
                    new RomModel { Value = "128GB" },
                    new RomModel { Value = "256GB" },
                    new RomModel { Value = "512GB" },
                    new RomModel { Value = "1TB" },
                    new RomModel { Value = "2TB" }
                };
                _context.Roms.AddRange(roms);
                _context.SaveChanges();
            }
        }
    }
}
