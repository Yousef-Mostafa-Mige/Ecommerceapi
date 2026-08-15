using Ecommerceapi.Dtos.Pagination;
using FluentValidation;

namespace Ecommerceapi.Dtos.Pagination
{
    public class PaginatedResponseDto<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<T> Items { get; set; } = new List<T>();
    }

    public class PaginatedRequestDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class ValidationPaginatedRequestDto : AbstractValidator<PaginatedRequestDto>
    {
        public ValidationPaginatedRequestDto()
        {
            RuleFor(x => x.PageNumber).GreaterThan(0).WithMessage("Page number must be greater than zero.");
            RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("Page size must be greater than zero.");
        }
    }
}