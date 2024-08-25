namespace MVC_FinalTerm.Models.ViewModels
{
    public class CheckoutViewModel
    {
        public List<CartItemModel> CartItems { get; set; }
        public decimal GrandTotal { get; set; }

        // Thông tin người dùng
        public string FirstName { get; set; }   
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Town { get; set; }
        public string Country { get; set; } // Bỏ country
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string CompanyName { get; set; } // Bỏ company name
        public string Postcode { get; set; } // Postcode không bắt buộc
        public string OtherNote { get; set; } // Other note không bắt buộc

        // Phương thức thanh toán
        public string PaymentMethod { get; set; }
    }
}
