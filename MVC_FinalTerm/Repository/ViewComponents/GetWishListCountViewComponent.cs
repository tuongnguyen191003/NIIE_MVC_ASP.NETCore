using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_FinalTerm.Models;


namespace MVC_FinalTerm.Repository.DataContext
{
    public class GetWishListCountViewComponent : ViewComponent
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly DataContext _context;

        public GetWishListCountViewComponent(UserManager<AppUserModel> userManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            int wishlistCount = 0;

            if (user != null)
            {
                wishlistCount = await _context.WishListItems.CountAsync(w => w.UserId == user.Id);
            }

            return View(wishlistCount);
        }
    }

}
