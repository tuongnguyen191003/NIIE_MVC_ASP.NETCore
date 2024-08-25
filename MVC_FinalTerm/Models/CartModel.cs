namespace MVC_FinalTerm.Models
{
    public class CartModel
    {
        public List<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();
        public decimal CartSubtotal { get; set; }
        public decimal ShippingAndHandling { get; set; }
        public decimal OrderTotal { get; set; }
        public string CouponCode { get; set; }
        public decimal CouponDiscount { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
    }
}
