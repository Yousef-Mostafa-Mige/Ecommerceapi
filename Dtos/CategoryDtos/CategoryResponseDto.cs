namespace Ecommerceapi.Dtos.CategoryDtos;

public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProductsCount { get; set; }
    public byte[] RowVersion { get; set; } = [];
}