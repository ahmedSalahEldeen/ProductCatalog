using Microsoft.EntityFrameworkCore;
using ProductCatalog.Data;
using ProductCatalog.Models;

namespace ProductCatalog.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;

        }

        public async Task<IEnumerable<Product>> GetActiveProductsAsync(int? categoryId = null)
        {
            var currentDateTime = DateTime.UtcNow;
            //using flitration filter by category
            var query =  _context.products
                .Include(p => p.Category)
                .Where(p => p.StartDate <= currentDateTime &&
                              currentDateTime <= p.StartDate.AddMinutes(p.DurationInMinutes))
                .AsQueryable();

            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }
            return await query.ToListAsync();

        }
        public async Task<IEnumerable<Product>> GetAllProductsAsync(int? categoryId = null)
        {
            var query = _context.products
            .Include(p => p.Category)
            .AsQueryable();
            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }
            return await query.ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.products
               .Include(p => p.Category)
               .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int? categoryId)
        {
            var query = _context.products.Include(p => p.Category).AsQueryable();

            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            return await query.ToListAsync();
        }

        public async Task AddProductAsync(Product product)
        {
            _context.products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            // If there's a new image, handle the upload
            if (product.ImageFile != null)
            {
                // Generate a unique file name (optional: add a timestamp to prevent name collisions)
                var fileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName)
                               + "_" + Guid.NewGuid() + Path.GetExtension(product.ImageFile.FileName);

                // Define the path to save the image
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                // Save the image to the file system
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(stream);
                }

                // Update the product's ImageFilePath
                product.ImageFilePath = "/images/" + fileName;
            }

            // Update the product entity
            _context.products.Update(product);
            await _context.SaveChangesAsync();
        }


        public async Task LogProductUpdateAsync(ProductLog log)
        {
            _context.productLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int id)
        {
            var product = await _context.products.FindAsync(id);
            if (product != null)
            {
                _context.products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }


    }
}
