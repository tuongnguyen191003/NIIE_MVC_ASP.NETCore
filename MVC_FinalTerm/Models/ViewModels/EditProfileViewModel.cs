using MVC_FinalTerm.Repository.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_FinalTerm.Models.ViewModels
{
    public class EditProfileViewModel
    {
        public string FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string? CurrentProfileImage { get; set; }
        [NotMapped]
        [FileExtension]
        public IFormFile? ProfileImageUpload { get; set; }
    }
}
