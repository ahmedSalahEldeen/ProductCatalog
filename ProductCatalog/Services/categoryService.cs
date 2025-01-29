using ProductCatalog.Models;
using ProductCatalog.Repositories;
namespace ProductCatalog.Services
{
    public class categoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public categoryService(ICategoryRepository categoryRepository)
        {
         _categoryRepository = categoryRepository;
        }
        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _categoryRepository.GetAllAsync();
        }

    }
}
