using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_FinalTerm.Models
{
    public class ReviewModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string ReviewerName { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Content { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }

        public string AuthorImage { get; set; }

        public string ProductImage { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? PhoneNumber { get; set; }

        public ReviewModel()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
