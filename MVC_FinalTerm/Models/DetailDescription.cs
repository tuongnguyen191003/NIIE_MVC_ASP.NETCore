using MVC_FinalTerm.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_FinalTerm.Models
{
    public class DetailDescription
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string? Content { get; set; }
        public string Image { get; set; }
        public string? VideoUrl { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile ImageUpload { get; set; }

        // Navigation Property
        public ProductModel Product { get; set; }
    }
}
