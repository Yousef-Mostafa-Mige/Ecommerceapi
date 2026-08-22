namespace Ecommerceapi.Dtos.ProductDtos;

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int stock {get; set;}
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public byte[] RowVersion { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}