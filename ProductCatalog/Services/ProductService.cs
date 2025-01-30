using ProductCatalog.Models;
using ProductCatalog.Repositories;

namespace ProductCatalog.Services
{
    public class ProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        // For anonymous users: Get only active products that are currently availabl, optionally filtered by category

        public async Task<IEnumerable<Product>> GetActiveProductsAsync(int? categoryId = null)
        {
            return await _productRepository.GetActiveProductsAsync(categoryId);
        }
        // For admin users: [admin can view all products, not just active ones] Get all products, optionally filtered by category

        public async Task<IEnumerable<Product>> GetAllProductsAsync(int? categoryId = null)
        {
            return await _productRepository.GetAllProductsAsync(categoryId);
        }

        public async Task<IEnumerable<Product>> GetProductsWithCategory(int? categoryId)
        {
            return await _productRepository.GetProductsByCategoryAsync(categoryId);
        }
        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetProductByIdAsync(id);
        }


        public async Task AddProductAsync(Product product, string userId)
        {
            product.CreatedByUserId = userId;
            await _productRepository.AddProductAsync(product);

            // Log the addition
            await _productRepository.LogProductUpdateAsync(new ProductLog
            {
                ProductId = product.Id,
                UpdatedByUserId = userId,
                Action = "Added"
            });
        }

        public async Task EditProductAsync(Product product, string userId)
        {
            // Handle image upload if a new image is provided
            if (product.ImageFile != null)
            {
                // Save the uploaded image
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", product.ImageFile.FileName);

                // Save the image to the file system
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(stream);
                }

                // Store the image path in the product model
                product.ImageFilePath = "/images/" + product.ImageFile.FileName;
            }
            product.CreatedByUserId = userId;
            // Update the product in the database
            await _productRepository.UpdateProductAsync(product);

            // Log the edit
            await _productRepository.LogProductUpdateAsync(new ProductLog
            {
                ProductId = product.Id,
                UpdatedByUserId = userId,
                Action = "Edited"
            });
        }


        public async Task DeleteProductAsync(int productId, string userId)
        {
            await _productRepository.DeleteProductAsync(productId);

            // Log the deletion
            await _productRepository.LogProductUpdateAsync(new ProductLog
            {
                ProductId = productId,
                UpdatedByUserId = userId,
                Action = "Deleted"
            });
        }
    }
}
