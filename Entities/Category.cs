using System.ComponentModel.DataAnnotations;
using Ecommerceapi.Entities;

namespace Ecommerceapi.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
      
        public long RowVersion { get; set; } 
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
} 