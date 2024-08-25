namespace MVC_FinalTerm.Models.ViewModels
{
    public class ProductDetailsViewModel
    {
        public ProductModel Product { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public List<ProductModel> RelatedProducts { get; set; } // Danh sách sản phẩm cùng series
    }
}
