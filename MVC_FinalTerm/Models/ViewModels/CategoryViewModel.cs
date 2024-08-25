namespace MVC_FinalTerm.Models.ViewModels
{
    public class CategoryViewModel
    {
        public CategoryModel Category { get; set; }
        public IEnumerable<ProductModel> Products { get; set; }
    }
}
