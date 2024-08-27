﻿using System.ComponentModel.DataAnnotations;

namespace MVC_FinalTerm.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập email"), EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Yêu cầu nhập mật khẩu")]
        public string Password { get; set; }
    }
}