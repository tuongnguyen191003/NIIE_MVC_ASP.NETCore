namespace MVC_FinalTerm.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int ProductCount { get; set; }
        public int UserCount { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<MonthlyRevenue> MonthlyRevenue { get; set; }
        public List<CategoryProductCount> CategoryProductCounts { get; set; }
        public List<UserViewModel> Users { get; set; }
    }

    public class UserViewModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string? PhoneNumber { get; set; }
    }
    public class MonthlyRevenue
    {
        public int Month { get; set; }
        public decimal Revenue { get; set; }
    }

    public class CategoryProductCount
    {
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
    }

}
