﻿using System.ComponentModel.DataAnnotations;

namespace MVC_FinalTerm.Models
{
    public class OrderModel
    {
        [Key]
        public int Id { get; set; }
        public string Note { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }

        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        // Thêm thuộc tính UserId
        public string UserId { get; set; }
        public string TransactionId { get; set; } 

        // Liên kết tới bảng người dùng nếu cần
        public AppUserModel User { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
