using System.ComponentModel.DataAnnotations;

namespace MVC_FinalTerm.Models
{
    public class ProductSpec
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }

        // Navigation Property
        public ProductModel Product { get; set; }
    }
}
