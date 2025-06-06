using Microsoft.AspNetCore.Mvc;
using THLTW.Services;
using THLTW.Models;
using Microsoft.AspNetCore.Identity;

namespace THLTW.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AIController(IRecommendationService recommendationService, UserManager<ApplicationUser> userManager)
        {
            _recommendationService = recommendationService;
            _userManager = userManager;
        }

        [HttpGet("recommendations/similar/{productId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetSimilarProducts(int productId, int count = 5)
        {
            try
            {
                var recommendations = await _recommendationService.GetSimilarProductsAsync(productId, count);
                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting similar products: {ex.Message}");
            }
        }

        [HttpGet("recommendations/trending")]
        public async Task<ActionResult<IEnumerable<Product>>> GetTrendingProducts(int count = 10)
        {
            try
            {
                var trending = await _recommendationService.GetTrendingProductsAsync(count);
                return Ok(trending);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting trending products: {ex.Message}");
            }
        }        [HttpGet("recommendations/personalized")]
        public async Task<ActionResult<IEnumerable<Product>>> GetPersonalizedRecommendations(int count = 10)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized("User not authenticated");
                }

                var recommendations = await _recommendationService.GetPersonalizedRecommendationsAsync(user.Id, count);
                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error getting personalized recommendations: {ex.Message}");
            }
        }
    }
}