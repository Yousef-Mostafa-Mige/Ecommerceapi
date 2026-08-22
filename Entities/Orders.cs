using System.ComponentModel.DataAnnotations;
using Ecommerceapi.Entities;

namespace Ecommerceapi.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; } = [];
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}