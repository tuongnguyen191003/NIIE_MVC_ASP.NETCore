    using System.ComponentModel.DataAnnotations;

    namespace MVC_FinalTerm.Models
    {
        public class CategoryModel
        {
            [Key]
            public int CategoryId { get; set; }
            [Required]
            public string Name { get; set; }
            public string? Description { get; set; }
            public string? Slug { get; set; }
            public string Status {  get; set; }
        //public ICollection<ChildCategory> ChildCategories { get; set; }
            //public ICollection<ProductModel>? Products { get; set; } // Thêm thuộc tính này
    }
    }
