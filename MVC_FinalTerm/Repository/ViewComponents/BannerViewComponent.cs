using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MVC_FinalTerm.Repository.DataContext
{
    public class BannerViewComponent : ViewComponent
    {
        private readonly DataContext _dataContext;
        public BannerViewComponent(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<IViewComponentResult> InvokeAsync() => View(await _dataContext.Banners.ToListAsync());
    }
}
