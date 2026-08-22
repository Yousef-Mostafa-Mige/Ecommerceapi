namespace Ecommerceapi.Dtos.CategoryDtos;

public class CategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProductsCount { get; set; }
    public long RowVersion { get; set; }
}