using System.ComponentModel.DataAnnotations;

namespace MVC_FinalTerm.Models
{
    public class OrderModel
    {
        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string Address { get; set; }
        public string? Town { get; set; }
        public string? Country { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public string? CompanyName { get; set; }
        public string? Postcode { get; set; }
        public string? OtherNote { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime OrderDate { get; set; }
        // Thêm thuộc tính UserId
        public string UserId { get; set; }

        // Liên kết tới bảng người dùng nếu cần
        public AppUserModel User { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
