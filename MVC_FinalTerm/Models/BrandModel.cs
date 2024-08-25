using MVC_FinalTerm.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_FinalTerm.Models
{
    public class BrandModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public string Status { get; set; }
        public string? Image {  get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? CreateUpdate {  get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}
