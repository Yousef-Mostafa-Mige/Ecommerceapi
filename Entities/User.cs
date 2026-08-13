using Ecommerceapi.Entities;

namespace Ecommerceapi.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string PasswordHash { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    
    
}