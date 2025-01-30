using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Models;
using ProductCatalog.Models.ViewModels;
using ProductCatalog.Services;

namespace ProductCatalog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductService _productService;
        private readonly categoryService _categoryService;

        public HomeController(ILogger<HomeController> logger, ProductService productService, categoryService categoryService)
        {
            _logger = logger;
            _productService = productService;
            _categoryService = categoryService;
        }


        public async Task<IActionResult> Index(int? categoryId)
        {
            try
            {
                IEnumerable<Product> products;

                if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
                {
                    products = await _productService.GetAllProductsAsync(categoryId);
                }
                else
                {
                    products = await _productService.GetActiveProductsAsync(categoryId);
                }

                //var activeProducts = await _productService.GetAllProductsAsync();
                var categories = await _categoryService.GetAllCategories();
                //var products =   await _productService.GetProductsWithCategory(categoryId);
                var viewModel = new ProductListViewModel
                {
                    Products = products,
                    Categories = categories,
                    SelectedCategoryId = categoryId
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching  products.");
                return View("Error");
            }
           
        }

        [HttpGet]
        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();

            }
            return View(product);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
