using Microsoft.EntityFrameworkCore;
using Mono.TextTemplating;
using ProductCatalog.Data;
using ProductCatalog.Models;
using ProductCatalog.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting.ProductCatalogProject
{
    public class ProductRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test
                .Options;

            var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync();
            return dbContext;
        }

        [Fact]
        public async Task GetActiveProductsAsync_ShouldReturnActiveProducts()
        {
            var dbContext = await GetDbContext();
            var repository = new ProductRepository(dbContext);

            // Arrange
            var activeProduct = new Product
            {
                Id = 1,
                Name = "Active Product",
                StartDate = DateTime.UtcNow.AddMinutes(-5), // started from 5 Minutes ago
                DurationInMinutes = 10, // Still active
                CategoryId = 1,
                CreatedByUserId = "02fe3eea-a150-4ba4-b777-2288d3b01134",
                ImageFilePath = "/images/047bc3af-318f-4c2b-a1c2-d9e853d1de9c.png"
            };

            await dbContext.products.AddAsync(activeProduct);
            await dbContext.SaveChangesAsync(); 

            // Act
            var result = await repository.GetActiveProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Active Product", result.First().Name);
        }
    }
}
