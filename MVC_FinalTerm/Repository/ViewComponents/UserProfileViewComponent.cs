using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MVC_FinalTerm.Models;

namespace MVC_FinalTerm.Repository.ViewComponents
{
    public class UserProfileViewComponent : ViewComponent
    {
        private readonly UserManager<AppUserModel> _userManager;

        public UserProfileViewComponent(UserManager<AppUserModel> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var profileImage = user?.ProfileImage ?? "/frontend/images/default-user.png";
            return View("Default", profileImage);
        }
    }
}
