using System.ComponentModel.DataAnnotations;

namespace MVC_FinalTerm.Models
{
    public class CouponModel
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public decimal Discount { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } // Trạng thái hoạt động
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
