using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductCatalog.Models;
using ProductCatalog.Services;
using System.Security.Claims;

namespace ProductCatalog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ProductService _productService;
        private readonly categoryService _categoryService;
        private readonly IHostEnvironment _hostEnvironment;

        public AdminController(ProductService productService, categoryService categoryService, IHostEnvironment hostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _hostEnvironment = hostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            var categories = await _categoryService.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            var productImageFile = product.ImageFile;
            // Check if the model is valid
            //if (!ModelState.IsValid)
            //{
            //    ViewBag.Categories = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
            //    return View(product);
            //}

            // Handle the image upload
            if (productImageFile != null)
            {
                // Validate file type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var extension = Path.GetExtension(productImageFile.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImagePath", "Only JPG, JPEG, and PNG files are allowed.");
                    ViewBag.Categories = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
                    return View(product);
                }

                // Validate file size (1MB max)
                if (productImageFile.Length > 1 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImagePath", "The image size must not exceed 1MB.");
                    ViewBag.Categories = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
                    return View(product);
                }
                try
                {
                    // Generate a unique file name
                    var uniqueFileName = Guid.NewGuid() + extension;

                    // Build the file path to the "wwwroot/images" folder
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    Directory.CreateDirectory(uploadFolder); // Ensure the directory exists

                    var filePath = Path.Combine(uploadFolder, uniqueFileName);
                    // Save the image to the specified folder
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await productImageFile.CopyToAsync(stream);
                    }

                    // Save the relative path to the database
                    product.ImageFilePath = $"/images/{uniqueFileName}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred while uploading the image: {ex.Message}");
                    ViewBag.Categories = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
                    return View(product);
                }
            }
            else
            {
                ModelState.AddModelError("ImagePath", "Please upload an image.");
                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategories(), "Id", "Name");
                return View(product);
            }

            // Assign the current user's ID to the CreatedByUserId property
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            product.CreatedByUserId = userId;
            // Add the product to the database
            await _productService.AddProductAsync(product, userId);
            // Redirect to the AllProducts page after successful addition
            return RedirectToAction("AllProducts");
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _productService.GetAllProductsAsync()
                 .ContinueWith(t => t.Result.FirstOrDefault(p => p.Id == id));
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                // Check if there's a new image
                if (product.ImageFile != null)
                {
                    // Handle image upload
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", product.ImageFile.FileName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(stream);
                    }

                    // Save the file path to the database
                    product.ImageFilePath = "/images/" + product.ImageFile.FileName;
                }


                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _productService.EditProductAsync(product, userId);
            }
            return RedirectToAction("AllProducts");
        }

        public async Task<IActionResult> DeleteProduct(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _productService.DeleteProductAsync(id, userId);
            return RedirectToAction("index");
        }
    }
}
