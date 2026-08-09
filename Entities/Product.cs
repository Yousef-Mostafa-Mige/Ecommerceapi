using Ecommerceapi.Entities;

namespace Ecommerceapi.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public int CategoryId { get; set; }
        public Category? Category { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}