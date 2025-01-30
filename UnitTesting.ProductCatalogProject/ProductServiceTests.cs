using ProductCatalog.Repositories;
using ProductCatalog.Services;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ProductCatalog.Models;


namespace UnitTesting.ProductCatalogProject
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepo;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockRepo = new Mock<IProductRepository>();
            _productService = new ProductService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetActiveProductsAsync_ShouldReturnActiveProducts()
        {
            // Arrange
            var products = new List<Product>
        {
            new Product { Id = 1, Name = "Active Product" },
            new Product { Id = 2, Name = "Another Active Product" }
        };

            _mockRepo.Setup(repo => repo.GetActiveProductsAsync(null))
                     .ReturnsAsync(products);

            // Act
            var result = await _productService.GetActiveProductsAsync();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnAllProductsInCategory()
        {
            // Arrange
            int categoryId = 1;

            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Product 1", CategoryId = 1 },
                new Product { Id = 2, Name = "Product 2", CategoryId = 1 },
                new Product { Id = 3, Name = "Product 3", CategoryId = 2 }
            };

            _mockRepo.Setup(repo => repo.GetAllProductsAsync(categoryId)).ReturnsAsync(products.Where(p => p.CategoryId == categoryId));

            // Act
            var result = await _productService.GetAllProductsAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Only products with CategoryId = 1 should be returned
        }

        [Fact]
        public async Task AddProductAsync_ShouldCallRepositoryMethod()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "New Product" };
            var userId = "user123";

            // Act
            await _productService.AddProductAsync(product, userId);

            // Assert
            _mockRepo.Verify(repo => repo.AddProductAsync(product), Times.Once);
            _mockRepo.Verify(repo => repo.LogProductUpdateAsync(It.IsAny<ProductLog>()), Times.Once);
        }


        [Fact]
        public async Task DeleteProductAsync_ShouldCallRepositoryMethod()
        {
            // Arrange
            var productId = 1;
            var userId = "admin123";

            // Act
            await _productService.DeleteProductAsync(productId, userId);

            // Assert
            _mockRepo.Verify(repo => repo.DeleteProductAsync(productId), Times.Once);
            _mockRepo.Verify(repo => repo.LogProductUpdateAsync(It.IsAny<ProductLog>()), Times.Once);
        }

    }
}


