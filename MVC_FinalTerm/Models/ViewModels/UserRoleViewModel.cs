using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC_FinalTerm.Models.ViewModels
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public SelectList AvailableRoles { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
    }
}
