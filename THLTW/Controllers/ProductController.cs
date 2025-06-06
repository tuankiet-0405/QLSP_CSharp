using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using THLTW.Models;
using THLTW.Repositories;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting; 
using System.IO; 
using Microsoft.AspNetCore.Http; 
using System.Collections.Generic; 
using Microsoft.EntityFrameworkCore; 

namespace THLTW.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IProductRepository productRepository, ICategoryRepository categoryRepository, IWebHostEnvironment webHostEnvironment) 
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _webHostEnvironment = webHostEnvironment; 
        }        // GET: Product
        public async Task<IActionResult> Index(int? categoryId, decimal? minPrice, decimal? maxPrice, string searchTerm, int pageNumber = 1)
        {
            int pageSize = 8;
            var (products, totalCount) = await _productRepository.GetFilteredAsync(pageNumber, pageSize, categoryId, minPrice, maxPrice, searchTerm);

            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name", categoryId);
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.SelectedCategoryId = categoryId;

            // Pagination-related ViewBag properties
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;
            ViewBag.TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            return View(products.ToList());
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            // ViewBag.Categories = await _categoryRepository.GetAllAsync(); // For dropdown
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name");
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,Description,CategoryId")] Product product, IFormFile mainImage, List<IFormFile> additionalImages) 
        {
            if (ModelState.IsValid)
            {
                if (mainImage != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "Products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + mainImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await mainImage.CopyToAsync(fileStream);
                    }
                    product.ImageUrl = "/images/Products/" + uniqueFileName; 
                }

                if (additionalImages != null && additionalImages.Count > 0)
                {
                    product.AdditionalImages = new List<ProductAdditionalImage>(); 
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "Products");
                    if (!Directory.Exists(uploadsFolder)) 
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    foreach (var imageFile in additionalImages)
                    {
                        if (imageFile != null && imageFile.Length > 0)
                        {
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(fileStream);
                            }
                            product.AdditionalImages.Add(new ProductAdditionalImage { ImageUrl = "/images/Products/" + uniqueFileName }); 
                        }
                    }
                }
                
                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            // ViewBag.Categories = await _categoryRepository.GetAllAsync(); // For dropdown
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,CategoryId")] Product productViewModel, IFormFile? mainImage, List<IFormFile>? additionalImages) // Added nullable type for mainImage and additionalImages
        {
            Console.WriteLine($"Edit POST called for ID: {id}");
            if (id != productViewModel.Id)
            {
                Console.WriteLine("ID mismatch.");
                return NotFound();
            }

            // Log ModelState errors for debugging
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is INVALID.");
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    if (modelStateVal != null) // Add null check here
                    {
                        foreach (var error in modelStateVal.Errors)
                        {
                            Console.WriteLine($"Key: {modelStateKey}, Error: {error.ErrorMessage}");
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                Console.WriteLine("ModelState is VALID.");
                try
                {
                    var existingProduct = await _productRepository.GetByIdAsync(id); 
                    if (existingProduct == null)
                    {
                        Console.WriteLine($"Existing product with ID {id} not found during update attempt.");
                        return NotFound("Existing product not found.");
                    }
                    Console.WriteLine($"Fetched existing product: {existingProduct.Name}");

                    existingProduct.Name = productViewModel.Name;
                    existingProduct.Price = productViewModel.Price;
                    existingProduct.Description = productViewModel.Description;
                    existingProduct.CategoryId = productViewModel.CategoryId;
                    Console.WriteLine("Scalar properties updated on existingProduct.");

                    if (mainImage != null)
                    {
                        Console.WriteLine("Main image provided.");
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "Products");
                        if (!Directory.Exists(uploadsFolder)) { Directory.CreateDirectory(uploadsFolder); }

                        if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                        {
                            string oldImageFullPath = Path.Combine(_webHostEnvironment.WebRootPath, existingProduct.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImageFullPath)) { System.IO.File.Delete(oldImageFullPath); Console.WriteLine("Old main image deleted."); }
                        }
                        
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + mainImage.FileName;
                        string newFilePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(newFilePath, FileMode.Create)) { await mainImage.CopyToAsync(fileStream); }
                        existingProduct.ImageUrl = "/images/Products/" + uniqueFileName;
                        Console.WriteLine("New main image saved.");
                    }

                    if (additionalImages != null && additionalImages.Any(f => f != null && f.Length > 0))
                    {
                        Console.WriteLine("Additional images provided.");
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "Products");
                        if (!Directory.Exists(uploadsFolder)) { Directory.CreateDirectory(uploadsFolder); }

                        if (existingProduct.AdditionalImages != null)
                        {
                            Console.WriteLine("Processing existing additional images for deletion.");
                            foreach (var oldImage in existingProduct.AdditionalImages.ToList()) 
                            {
                                if (!string.IsNullOrEmpty(oldImage.ImageUrl))
                                {
                                    string oldImageFullPath = Path.Combine(_webHostEnvironment.WebRootPath, oldImage.ImageUrl.TrimStart('/'));
                                    if (System.IO.File.Exists(oldImageFullPath)) { System.IO.File.Delete(oldImageFullPath); Console.WriteLine("Old additional image file deleted."); }
                                }
                            }
                            existingProduct.AdditionalImages.Clear(); 
                            Console.WriteLine("Cleared existing AdditionalImages collection.");
                        }
                        else
                        {
                            existingProduct.AdditionalImages = new List<ProductAdditionalImage>();
                            Console.WriteLine("Initialized new AdditionalImages collection.");
                        }

                        foreach (var imageFile in additionalImages.Where(f => f != null && f.Length > 0))
                        {
                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                            string newFilePath = Path.Combine(uploadsFolder, uniqueFileName);
                            using (var fileStream = new FileStream(newFilePath, FileMode.Create)) { await imageFile.CopyToAsync(fileStream); }
                            existingProduct.AdditionalImages.Add(new ProductAdditionalImage { ImageUrl = "/images/Products/" + uniqueFileName, ProductId = existingProduct.Id });
                            Console.WriteLine("New additional image added to collection.");
                        }
                    }
                    Console.WriteLine("Attempting to update product in repository...");
                    await _productRepository.UpdateAsync(existingProduct);
                    Console.WriteLine("Product update call completed.");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine($"DbUpdateConcurrencyException: {ex.Message}");
                    if (!await ProductExists(productViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Generic Exception during Edit POST: {ex.ToString()}");
                    // Optionally, add a model error to show to the user
                    ModelState.AddModelError("", "An unexpected error occurred while saving the product. Please try again.");
                    // Repopulate necessary data for the view if returning the view
                    ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name", productViewModel.CategoryId);
                    // It's better to return the original product with attempted changes if an error occurs mid-update
                    var productForView = await _productRepository.GetByIdAsync(id) ?? productViewModel;
                    if (productForView is Product p) // if it's the actual product model
                    {
                        p.Name = productViewModel.Name; // Apply attempted changes for display
                        p.Price = productViewModel.Price;
                        p.Description = productViewModel.Description;
                        p.CategoryId = productViewModel.CategoryId;
                    }
                    return View(productForView);
                }
                return RedirectToAction(nameof(Index));
            }
            
            // ModelState is Invalid, prepare to return the view
            Console.WriteLine("ModelState is invalid, preparing to return view.");
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name", productViewModel.CategoryId);
            
            // Fetch the full existing product to display its current images and other non-bound properties,
            // then overlay the values the user attempted to submit from productViewModel.
            var modelToReturnToView = await _productRepository.GetByIdAsync(id);
            if (modelToReturnToView != null)
            {
                // Apply the values from productViewModel that the user tried to change
                modelToReturnToView.Name = productViewModel.Name;
                modelToReturnToView.Price = productViewModel.Price;
                modelToReturnToView.Description = productViewModel.Description;
                modelToReturnToView.CategoryId = productViewModel.CategoryId;
                // modelToReturnToView already has its ImageUrl and AdditionalImages loaded by GetByIdAsync
            }
            else
            {
                // Should not happen if ID was valid, but as a fallback:
                modelToReturnToView = productViewModel; 
            }
            return View(modelToReturnToView); 
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            // Delete main image
            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            // Delete additional images
            if (product.AdditionalImages != null)
            {
                foreach (var addImage in product.AdditionalImages)
                {
                    if (!string.IsNullOrEmpty(addImage.ImageUrl))
                    {
                        string addImagePath = Path.Combine(_webHostEnvironment.WebRootPath, addImage.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(addImagePath))
                        {
                            System.IO.File.Delete(addImagePath);
                        }
                    }
                }
            }

            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProductExists(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product != null;
        }
    }
}
