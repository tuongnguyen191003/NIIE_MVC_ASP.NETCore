namespace MVC_FinalTerm.Models.ViewModels
{
    public class CartItemViewModel
    {
        public List<CartItemModel> CartItems { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal DiscountAmount { get; set; }
    }
}
