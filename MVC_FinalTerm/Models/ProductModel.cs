using MVC_FinalTerm.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace MVC_FinalTerm.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string? Description { get; set; }
        public string? Slug { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,0)")] // Định dạng decimal
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]  // Hiển thị  'N0' (không chữ số thập phân)
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,0)")] // Định dạng decimal
        [DisplayFormat(DataFormatString = "{0:N0}", ApplyFormatInEditMode = true)]  // Hiển thị  'N0' (không chữ số thập phân)
        public decimal OldPrice { get; set; }
        public int ColorId { get; set; } //khóa ngoại
        public int StockQuantity { get; set; }
        public string? StatusName { get; set; } //sales, hot, new
        public bool IsOnStatus { get; set; } //trạng thái ẩn hoặc hiện
        //public bool IsOnSales { get; set; } //trạng thái xem sản phẩm có phải đang sales hay không
        //public int SeriesId { get; set; } //ví dụ như iphone 15 series, khóa ngoại tới bảng danh mục con
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        //public int ChildId { get; set; } //Id của ChildCategory
        //Navigation Properties
        //public SeriesModel Series { get; set; }
        public BrandModel? Brand { get; set; }
        public CategoryModel? Category { get; set; } 
        //public ChildCategory ChildCategory { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
        public ColorModel? Color { get; set; }
        // Khóa ngoại cho RAM
        public int RamId { get; set; }
        public RamModel? Ram { get; set; }

        // Khóa ngoại cho ROM
        public int RomId { get; set; }
        public RomModel? Rom { get; set; }
        public ICollection<ProductImage>? ProductImages { get; set; }
        public ICollection<ReviewModel>? Reviews { get; set; } 
        public ICollection<DetailDescription>? DetailDescriptions { get; set; }
        public ICollection<ProductSpec>? ProductSpecs { get; set; }
    }
}
