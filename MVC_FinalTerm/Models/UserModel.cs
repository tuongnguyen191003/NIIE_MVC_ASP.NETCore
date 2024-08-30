using MVC_FinalTerm.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_FinalTerm.Models
{
    public class UserModel
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập email"), EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Yêu cầu nhập mật khẩu")]
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? FullName { get; set; }
        public string? ProfileImage { get; set; } // Thêm thuộc tính này để lưu trữ đường dẫn
        [NotMapped] // Không lưu vào database
        [FileExtension]
        public IFormFile? ProfileImageUpload { get; set; } // Thêm thuộc tính này để upload file
    }
}
