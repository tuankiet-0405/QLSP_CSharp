namespace THLTW.Models
{
    public class CartItem
    {
        public int Id { get; set; } // Primary key
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Total => Price * Quantity;
    }
}
