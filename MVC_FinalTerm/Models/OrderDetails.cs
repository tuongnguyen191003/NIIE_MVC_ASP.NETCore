using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MVC_FinalTerm.Models
{
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Order")]
        public int OrderId { get; set; }
        public OrderModel Order { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal? Discount { get; set; }
        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }

    }
}
