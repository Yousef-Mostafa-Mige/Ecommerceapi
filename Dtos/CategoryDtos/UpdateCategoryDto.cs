using FluentValidation;

namespace Ecommerceapi.Dtos.CategoryDtos;

public class UpdateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public byte[] RowVersion { get; set; } = [];
}
public class CategoryValidationUpdateCategoryDto : AbstractValidator<UpdateCategoryDto>
{
    public CategoryValidationUpdateCategoryDto()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Category name is required.");
    }
}