using THLTW.Data;
using THLTW.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json;

namespace THLTW.Middleware
{
    public class UserActivityTrackingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UserActivityTrackingMiddleware> _logger;

        public UserActivityTrackingMiddleware(RequestDelegate next, ILogger<UserActivityTrackingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            // Store the original response body stream
            var originalBodyStream = context.Response.Body;

            try
            {
                await _next(context);

                // Track specific activities after request completion
                await TrackUserActivity(context, dbContext, userManager);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UserActivityTrackingMiddleware");
                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private async Task TrackUserActivity(HttpContext context, ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            try
            {
                var path = context.Request.Path.Value?.ToLower();
                var method = context.Request.Method;

                // Only track specific activities
                if (ShouldTrackActivity(path, method))
                {
                    var user = await userManager.GetUserAsync(context.User);
                    var userId = user?.Id;
                    var sessionId = context.Session.Id;
                    var ipAddress = GetClientIpAddress(context);

                    var activityType = DetermineActivityType(path, method);
                    var productId = ExtractProductId(path);
                    var metadata = CreateMetadata(context, path);

                    var userActivity = new UserActivity
                    {
                        UserId = userId ?? "anonymous",
                        ActivityType = activityType,
                        ProductId = productId,
                        ActivityData = JsonSerializer.Serialize(metadata),
                        Timestamp = DateTime.Now,
                        SessionId = sessionId,
                        IpAddress = ipAddress
                    };

                    dbContext.UserActivities.Add(userActivity);

                    // Also track product view history for detailed analytics
                    if (activityType == "View" && productId.HasValue)
                    {
                        var viewHistory = new ProductViewHistory
                        {
                            UserId = userId,
                            ProductId = productId.Value,
                            ViewedAt = DateTime.Now,
                            SessionId = sessionId,
                            IpAddress = ipAddress,
                            ViewDurationSeconds = 0 // Could be enhanced with JavaScript tracking
                        };

                        dbContext.ProductViewHistories.Add(viewHistory);
                    }

                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking user activity");
                // Don't throw - tracking shouldn't break the app
            }
        }

        private static bool ShouldTrackActivity(string? path, string method)
        {
            if (string.IsNullOrEmpty(path)) return false;

            // Track these specific patterns
            var trackablePatterns = new[]
            {
                "/product/details",
                "/cart/add",
                "/category/",
                "/home/search",
                "/product/index"
            };

            return method == "GET" && trackablePatterns.Any(pattern => path.Contains(pattern));
        }

        private static string DetermineActivityType(string? path, string method)
        {
            if (string.IsNullOrEmpty(path)) return "Unknown";

            if (path.Contains("/product/details")) return "View";
            if (path.Contains("/cart/add")) return "AddToCart";
            if (path.Contains("/category/")) return "Browse";
            if (path.Contains("/search")) return "Search";
            if (path.Contains("/product/index")) return "Browse";

            return "View";
        }

        private static int? ExtractProductId(string? path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            // Extract product ID from paths like /Product/Details/123
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 3 && segments[0].ToLower() == "product" && 
                segments[1].ToLower() == "details" && int.TryParse(segments[2], out int productId))
            {
                return productId;
            }

            return null;
        }

        private static object CreateMetadata(HttpContext context, string? path)
        {
            var metadata = new Dictionary<string, object>
            {
                { "path", path ?? "" },
                { "userAgent", context.Request.Headers.UserAgent.ToString() },
                { "referer", context.Request.Headers.Referer.ToString() },
                { "timestamp", DateTime.Now }
            };

            // Add query parameters if any
            if (context.Request.Query.Any())
            {
                metadata["queryParams"] = context.Request.Query.ToDictionary(q => q.Key, q => q.Value.ToString());
            }

            return metadata;
        }

        private static string GetClientIpAddress(HttpContext context)
        {
            var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            }
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.Connection.RemoteIpAddress?.ToString();
            }
            return ipAddress ?? "Unknown";
        }
    }
}
