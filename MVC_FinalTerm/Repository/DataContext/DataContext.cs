
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
    using MVC_FinalTerm.Models;

    namespace MVC_FinalTerm.Repository.DataContext
    {
        public class DataContext : IdentityDbContext<AppUserModel>
        {
            public DataContext(DbContextOptions<DataContext> options) : base(options)
            {
            }
            // DbSet for each model
            public DbSet<BrandModel> Brands { get; set; }
            public DbSet<CategoryModel> Categories { get; set; }
            //public DbSet<ChildCategory> ChildCategories { get; set; }
            public DbSet<ColorModel> Colors { get; set; }
            public DbSet<ProductImage> ProductImages { get; set; }
            public DbSet<ProductModel> Products { get; set; }
            public DbSet<RamModel> Rams { get; set; }
            public DbSet<RomModel> Roms { get; set; }
           
        //public DbSet<SeriesModel> Series { get; set; }
            public DbSet<BannerModel> Banners { get; set; }
            public DbSet<ReviewModel> Reviews { get; set; }
            public DbSet<DetailDescription> DetailDescriptions { get; set; }
            public DbSet<ProductSpec> ProductSpecs { get; set; }
            public DbSet<CouponModel> Coupons { get; set; }
            public DbSet<OrderModel> Orders { get; set; }
            public DbSet<OrderDetails> OrderDetails { get; set; }
            public DbSet<WishListItems> WishListItems { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //    {
        //    base.OnModelCreating(modelBuilder);
        //}
    }
    }
