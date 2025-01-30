using ProductCatalog.Models;

namespace ProductCatalog.Repositories
{
    public interface IProductRepository
    {
        // For anonymous users: Get active products, optionally filtered by category
        Task<IEnumerable<Product>> GetActiveProductsAsync(int? categoryId = null);
        // For admin users: Get all products(active or inactive), optionally filtered by category
        Task< IEnumerable<Product>> GetAllProductsAsync(int? categoryId = null);
        Task<Product> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task DeleteProductAsync(int id);
        Task LogProductUpdateAsync(ProductLog  log);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int? categoryId);

    }
}
