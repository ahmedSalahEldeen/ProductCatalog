using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Models;
using System.Reflection.Emit;

namespace ProductCatalog.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> products { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<ProductLog> productLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
               .HasOne(p => p.Category)
               .WithMany()
               .HasForeignKey(p => p.CategoryId)
       .OnDelete(DeleteBehavior.Restrict); // Restrict delete behavior

            //Categories are created as a seed to the db and  will not be added using the application interface. 

            builder.Entity<Category>().HasData(
              new Category { Id = 1, Name = "Computers & Laptops" },
              new Category { Id = 2, Name = "Electronics" },
              new Category { Id = 3, Name = "Mobile Phones & Accessories" }
          );
        }

    }
}
