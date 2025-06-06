using Microsoft.AspNetCore.Identity;

namespace THLTW.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLoginAt { get; set; }
        
        // Navigation properties for AI training data
        public List<Order> Orders { get; set; } = new List<Order>();
        public List<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        public List<UserActivity> Activities { get; set; } = new List<UserActivity>();
        public List<ProductViewHistory> ViewHistory { get; set; } = new List<ProductViewHistory>();
    }
}
