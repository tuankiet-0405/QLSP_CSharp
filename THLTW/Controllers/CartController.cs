using Microsoft.AspNetCore.Mvc;
using THLTW.Models;
using THLTW.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // Required for session
using Newtonsoft.Json; // Required for JSON serialization

namespace THLTW.Controllers
{
    public class CartController : Controller
    {
        private readonly IProductRepository _productRepository;
        private const string CartSessionKey = "Cart";

        public CartController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (cartJson == null)
            {
                return new List<CartItem>();
            }
            return JsonConvert.DeserializeObject<List<CartItem>>(cartJson) ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cart)
        {
            var cartJson = JsonConvert.SerializeObject(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            // Pass the cart to a partial view or return as JSON for modal
            return PartialView("_CartModalPartial", cart); 
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return NotFound(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(item => item.ProductId == productId);

            if (cartItem == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl
                });
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            SaveCart(cart);
            return Ok(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng.", itemCount = cart.Sum(i => i.Quantity) });
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(item => item.ProductId == productId);

            if (cartItem != null)
            {
                cart.Remove(cartItem);
                SaveCart(cart);
            }
            return Ok(new { success = true, message = "Sản phẩm đã được xóa khỏi giỏ hàng.", itemCount = cart.Sum(i => i.Quantity) });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                return RemoveFromCart(productId);
            }

            var cart = GetCart();
            var cartItem = cart.FirstOrDefault(item => item.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                SaveCart(cart);
                return Ok(new { success = true, message = "Số lượng đã được cập nhật.", itemCount = cart.Sum(i => i.Quantity), itemTotal = cartItem.Total });
            }
            return NotFound(new { success = false, message = "Sản phẩm không tìm thấy trong giỏ." });
        }
        
        [HttpGet]
        public IActionResult GetCartItemCount()
        {
            var cart = GetCart();
            return Ok(new { success = true, itemCount = cart.Sum(i => i.Quantity) });
        }
    }
}
