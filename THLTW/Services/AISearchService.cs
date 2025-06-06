using THLTW.Models;
using THLTW.Data;
using Microsoft.EntityFrameworkCore;

namespace THLTW.Services
{
    public interface IAISearchService
    {
        Task<List<Product>> SmartSearchAsync(string query, int maxResults = 10);
        Task<List<Product>> SearchByImageAsync(string imageUrl, int maxResults = 10);
        Task<AISearchSuggestion> GetSearchSuggestionsAsync(string partialQuery);
        Task<string> GenerateChatResponseAsync(string userMessage, string? userId = null);
        Task<AIAnalytics> GetAIAnalyticsAsync();
        Task LogSearchQueryAsync(string query, string? userId, int resultCount);
    }

    public class AISearchService : IAISearchService
    {
        private readonly ApplicationDbContext _context;
        private readonly IRecommendationService _recommendationService;

        public AISearchService(ApplicationDbContext context, IRecommendationService recommendationService)
        {
            _context = context;
            _recommendationService = recommendationService;
        }

        public async Task<List<Product>> SmartSearchAsync(string query, int maxResults = 10)
        {
            try
            {
                // Log the search query
                await LogSearchQueryAsync(query, null, 0);

                // Extract keywords and intent from the query
                var searchTerms = ExtractSearchTerms(query);
                var priceRange = ExtractPriceRange(query);
                var categoryHints = ExtractCategoryHints(query);

                // Build the search query
                var searchQuery = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Reviews)
                    .AsQueryable();                // Apply text search
                if (searchTerms.Any())
                {
                    searchQuery = searchQuery.Where(p => 
                        searchTerms.Any(term => 
                            p.Name.Contains(term) || 
                            (p.Description != null && p.Description.Contains(term)) ||
                            (p.Category != null && p.Category.Name.Contains(term))
                        )
                    );
                }

                // Apply price filter
                if (priceRange.HasValue)
                {
                    var (minPrice, maxPrice) = priceRange.Value;
                    searchQuery = searchQuery.Where(p => p.Price >= minPrice && p.Price <= maxPrice);
                }                // Apply category filter
                if (categoryHints.Any())
                {
                    searchQuery = searchQuery.Where(p => 
                        p.Category != null && categoryHints.Contains(p.Category.Name.ToLower())
                    );
                }

                // Order by relevance (simplified scoring)
                var results = await searchQuery
                    .OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0)
                    .ThenByDescending(p => p.Reviews.Count)
                    .Take(maxResults)
                    .ToListAsync();

                // Update log with result count
                await LogSearchQueryAsync(query, null, results.Count);

                return results;
            }
            catch (Exception ex)
            {
                // Log error and return empty results
                Console.WriteLine($"Smart search error: {ex.Message}");
                return new List<Product>();
            }
        }

        public async Task<List<Product>> SearchByImageAsync(string imageUrl, int maxResults = 10)
        {
            // Placeholder for image search functionality
            // In a real implementation, this would use computer vision services
            // like Azure Cognitive Services or Google Vision API
            
            await Task.Delay(100); // Simulate processing time
            
            // For now, return trending products as fallback
            return await _recommendationService.GetTrendingProductsAsync(maxResults);
        }

        public async Task<AISearchSuggestion> GetSearchSuggestionsAsync(string partialQuery)
        {
            var suggestions = new AISearchSuggestion();

            try
            {                // Get popular search terms
                var popularTerms = await _context.Products
                    .Where(p => p.Name.Contains(partialQuery) || (p.Description != null && p.Description.Contains(partialQuery)))
                    .Select(p => p.Name)
                    .Take(5)
                    .ToListAsync();

                suggestions.PopularTerms = popularTerms;

                // Get category suggestions
                var categoryNames = await _context.Categories
                    .Where(c => c.Name.Contains(partialQuery))
                    .Select(c => c.Name)
                    .Take(3)
                    .ToListAsync();

                suggestions.Categories = categoryNames;

                // Generate smart completions
                suggestions.SmartCompletions = GenerateSmartCompletions(partialQuery);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search suggestions error: {ex.Message}");
            }

            return suggestions;
        }

        public async Task<string> GenerateChatResponseAsync(string userMessage, string? userId = null)
        {
            try
            {
                var message = userMessage.ToLower().Trim();

                // Intent detection for product search
                if (ContainsKeywords(message, new[] { "đồng hồ", "dong ho", "watch" }))
                {
                    return await HandleProductSearchInquiry(message, "đồng hồ");
                }

                if (ContainsKeywords(message, new[] { "áo", "shirt", "quần", "pants", "clothing" }))
                {
                    return await HandleProductSearchInquiry(message, "clothing");
                }

                // Intent detection
                if (ContainsKeywords(message, new[] { "giá", "bao nhiêu", "cost", "price" }))
                {
                    return await HandlePriceInquiry(message);
                }

                if (ContainsKeywords(message, new[] { "giao hàng", "ship", "delivery", "vận chuyển" }))
                {
                    return "🚚 Chúng tôi có các hình thức giao hàng:\n" +
                           "• Giao hàng nhanh: 1-2 ngày (30.000 VNĐ)\n" +
                           "• Giao hàng tiêu chuẩn: 3-5 ngày (15.000 VNĐ)\n" +
                           "• Miễn phí giao hàng cho đơn từ 500.000 VNĐ\n" +
                           "Bạn có muốn kiểm tra thời gian giao hàng đến địa chỉ cụ thể không?";
                }

                if (ContainsKeywords(message, new[] { "bảo hành", "warranty", "guarantee" }))
                {
                    return "🛡️ Chính sách bảo hành của chúng tôi:\n" +
                           "• Điện thoại: 12-24 tháng\n" +
                           "• Laptop: 12-36 tháng\n" +
                           "• Phụ kiện: 6-12 tháng\n" +
                           "• Bảo hành 1 đổi 1 trong 7 ngày đầu\n" +
                           "Bạn quan tâm bảo hành sản phẩm nào cụ thể?";
                }

                if (ContainsKeywords(message, new[] { "điện thoại", "phone", "smartphone" }))
                {
                    return await HandleProductInquiry("điện thoại");
                }

                if (ContainsKeywords(message, new[] { "laptop", "máy tính", "computer" }))
                {
                    return await HandleProductInquiry("laptop");
                }

                if (ContainsKeywords(message, new[] { "gợi ý", "recommend", "suggest", "tư vấn" }))
                {
                    return "✨ Tôi có thể gợi ý sản phẩm phù hợp với bạn! Hãy cho tôi biết:\n" +
                           "• Loại sản phẩm bạn quan tâm?\n" +
                           "• Mức giá bạn mong muốn?\n" +
                           "• Mục đích sử dụng chính?\n" +
                           "Ví dụ: 'Tôi muốn mua điện thoại chụp ảnh đẹp, giá dưới 15 triệu'";
                }

                // Default response
                return "🤖 Cảm ơn bạn đã liên hệ! Tôi có thể hỗ trợ bạn về:\n" +
                       "• 🛍️ Tìm kiếm và tư vấn sản phẩm\n" +
                       "• 💰 Thông tin giá cả và khuyến mãi\n" +
                       "• 🚚 Giao hàng và vận chuyển\n" +
                       "• 🛡️ Bảo hành và chính sách\n" +
                       "• 📞 Hỗ trợ kỹ thuật\n\n" +
                       "Bạn cần hỗ trợ gì cụ thể nhé?";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chat response error: {ex.Message}");
                return "Xin lỗi, tôi đang gặp sự cố kỹ thuật. Vui lòng thử lại sau ít phút hoặc liên hệ hotline: 0345-840-642";
            }
        }

        public async Task<AIAnalytics> GetAIAnalyticsAsync()
        {
            try
            {
                var analytics = new AIAnalytics();

                // Calculate AI performance metrics
                var totalSearches = await _context.SearchLogs.CountAsync();
                var successfulSearches = await _context.SearchLogs
                    .Where(sl => sl.ResultCount > 0)
                    .CountAsync();

                analytics.SearchAccuracy = totalSearches > 0 ? 
                    (double)successfulSearches / totalSearches * 100 : 0;

                // Average response time (simulated)
                analytics.AverageResponseTime = 0.2;

                // Daily recommendations count
                var today = DateTime.Today;
                analytics.DailyRecommendations = await _context.SearchLogs
                    .Where(sl => sl.Timestamp >= today)
                    .CountAsync();

                // User satisfaction (simulated - would come from feedback)
                analytics.UserSatisfaction = 94.5;

                // Popular search terms
                analytics.PopularSearchTerms = await _context.SearchLogs
                    .Where(sl => sl.Timestamp >= DateTime.Now.AddDays(-7))
                    .GroupBy(sl => sl.Query.ToLower())
                    .OrderByDescending(g => g.Count())
                    .Take(10)
                    .Select(g => new SearchTerm { Term = g.Key, Count = g.Count() })
                    .ToListAsync();

                return analytics;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI Analytics error: {ex.Message}");
                return new AIAnalytics();
            }
        }

        public async Task LogSearchQueryAsync(string query, string? userId, int resultCount)
        {
            try
            {
                var searchLog = new SearchLog
                {
                    Query = query,
                    UserId = userId,
                    ResultCount = resultCount,
                    Timestamp = DateTime.Now
                };

                _context.SearchLogs.Add(searchLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Search logging error: {ex.Message}");
            }
        }

        #region Private Helper Methods

        private List<string> ExtractSearchTerms(string query)
        {
            // Remove common Vietnamese stop words and extract meaningful terms
            var stopWords = new[] { "tôi", "muốn", "cần", "tìm", "mua", "có", "là", "một", "của", "cho", "với", "được" };
            
            return query.ToLower()
                .Split(new[] { ' ', ',', '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(word => !stopWords.Contains(word) && word.Length > 2)
                .ToList();
        }

        private (decimal min, decimal max)? ExtractPriceRange(string query)
        {
            // Extract price information from natural language
            query = query.ToLower();

            if (query.Contains("dưới") || query.Contains("under"))
            {
                var maxPrice = ExtractPriceFromText(query);
                if (maxPrice.HasValue)
                    return (0, maxPrice.Value);
            }

            if (query.Contains("trên") || query.Contains("over"))
            {
                var minPrice = ExtractPriceFromText(query);
                if (minPrice.HasValue)
                    return (minPrice.Value, decimal.MaxValue);
            }

            // Default price ranges based on keywords
            if (query.Contains("rẻ") || query.Contains("cheap"))
                return (0, 5000000); // Under 5M

            if (query.Contains("tầm trung") || query.Contains("mid-range"))
                return (5000000, 15000000); // 5M-15M

            if (query.Contains("cao cấp") || query.Contains("premium") || query.Contains("đắt"))
                return (15000000, decimal.MaxValue); // Over 15M

            return null;
        }

        private decimal? ExtractPriceFromText(string query)
        {
            // Simple price extraction (triệu, nghìn, etc.)
            var pricePatterns = new[]
            {
                @"(\d+)\s*triệu",
                @"(\d+)\s*tr",
                @"(\d+)\s*nghìn",
                @"(\d+)\s*k"
            };

            foreach (var pattern in pricePatterns)
            {
                var match = System.Text.RegularExpressions.Regex.Match(query, pattern);
                if (match.Success && decimal.TryParse(match.Groups[1].Value, out var amount))
                {
                    if (pattern.Contains("triệu") || pattern.Contains("tr"))
                        return amount * 1000000;
                    if (pattern.Contains("nghìn") || pattern.Contains("k"))
                        return amount * 1000;
                }
            }

            return null;
        }

        private List<string> ExtractCategoryHints(string query)
        {
            var categoryMappings = new Dictionary<string, string[]>
            {
                ["điện thoại"] = new[] { "phone", "smartphone", "di động", "mobile" },
                ["laptop"] = new[] { "máy tính", "computer", "notebook" },
                ["tai nghe"] = new[] { "headphone", "earphone", "âm thanh" },
                ["đồng hồ"] = new[] { "watch", "smartwatch", "thông minh" },
                ["phụ kiện"] = new[] { "accessory", "case", "ốp", "cáp", "sạc" }
            };

            var hints = new List<string>();
            query = query.ToLower();

            foreach (var mapping in categoryMappings)
            {
                if (mapping.Value.Any(keyword => query.Contains(keyword)))
                {
                    hints.Add(mapping.Key);
                }
            }

            return hints;
        }

        private bool ContainsKeywords(string message, string[] keywords)
        {
            return keywords.Any(keyword => message.Contains(keyword));
        }

        private async Task<string> HandlePriceInquiry(string message)
        {
            // Try to extract product name from message and provide price info
            var products = await _context.Products
                .OrderByDescending(p => p.Reviews.Count())
                .Take(3)
                .ToListAsync();

            var response = "💰 Một số sản phẩm phổ biến và giá cả:\n\n";
            foreach (var product in products)
            {
                response += $"• {product.Name}: {product.Price:N0} VNĐ\n";
            }

            response += "\nBạn quan tâm sản phẩm nào cụ thể? Tôi có thể tìm giá tốt nhất cho bạn!";
            return response;
        }        private async Task<string> HandleProductSearchInquiry(string message, string productType)
        {
            try
            {
                // Use the smart search to find relevant products
                var products = await SmartSearchAsync(message, 5);

                if (!products.Any())
                {
                    return $"🔍 Xin lỗi, tôi không tìm thấy {productType} phù hợp với yêu cầu '{message}' của bạn.\n" +
                           $"Bạn có thể thử:\n" +
                           $"• Tìm kiếm với từ khóa khác\n" +
                           $"• Duyệt qua danh mục sản phẩm\n" +
                           $"• Liên hệ với tư vấn viên để được hỗ trợ";
                }

                var response = $"🎯 Tôi tìm thấy {products.Count} {productType} phù hợp với yêu cầu của bạn:\n\n";

                foreach (var product in products)
                {
                    var rating = product.Reviews?.Any() == true ? 
                        $" (⭐ {product.Reviews.Average(r => r.Rating):F1})" : "";
                    response += $"💰 **{product.Name}**{rating}\n";
                    response += $"   Giá: {product.Price:N0} VNĐ\n";
                    if (!string.IsNullOrEmpty(product.Description))
                    {
                        var shortDesc = product.Description.Length > 100 ? 
                            product.Description.Substring(0, 100) + "..." : 
                            product.Description;
                        response += $"   {shortDesc}\n";
                    }
                    response += "\n";
                }

                response += "📞 Bạn có muốn tôi tư vấn thêm về sản phẩm nào cụ thể không?";
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Product search inquiry error: {ex.Message}");
                return $"Xin lỗi, có lỗi xảy ra khi tìm kiếm {productType}. Vui lòng thử lại sau.";
            }
        }

        private async Task<string> HandleProductInquiry(string productType)
        {
            var count = await _context.Products
                .Where(p => p.Name.ToLower().Contains(productType) || 
                           (p.Description != null && p.Description.ToLower().Contains(productType)))
                .CountAsync();

            return $"📱 Chúng tôi có {count} {productType} đang bán.\n" +
                   $"Bạn có thể cho tôi biết thêm về:\n" +
                   $"• Mức giá mong muốn?\n" +
                   $"• Thương hiệu ưa thích?\n" +
                   $"• Tính năng quan trọng?\n" +
                   $"Tôi sẽ gợi ý {productType} phù hợp nhất!";
        }

        private List<string> GenerateSmartCompletions(string partialQuery)
        {
            var completions = new List<string>();

            // Common completion patterns
            var patterns = new[]
            {
                partialQuery + " giá rẻ",
                partialQuery + " chính hãng",
                partialQuery + " mới nhất",
                partialQuery + " tốt nhất",
                partialQuery + " khuyến mãi"
            };

            return patterns.Take(3).ToList();
        }

        #endregion
    }

    // Supporting classes
    public class AISearchSuggestion
    {
        public List<string> PopularTerms { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public List<string> SmartCompletions { get; set; } = new();
    }

    public class AIAnalytics
    {
        public double SearchAccuracy { get; set; }
        public double AverageResponseTime { get; set; }
        public int DailyRecommendations { get; set; }
        public double UserSatisfaction { get; set; }
        public List<SearchTerm> PopularSearchTerms { get; set; } = new();
    }

    public class SearchTerm
    {
        public string Term { get; set; } = "";
        public int Count { get; set; }
    }

    public class SearchLog
    {
        public int Id { get; set; }
        public string Query { get; set; } = "";
        public string? UserId { get; set; }
        public int ResultCount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
