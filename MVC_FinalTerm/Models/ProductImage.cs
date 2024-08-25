using MVC_FinalTerm.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_FinalTerm.Models
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUploads { get; set; }
        public ProductModel Product { get; set; }
    }
}
