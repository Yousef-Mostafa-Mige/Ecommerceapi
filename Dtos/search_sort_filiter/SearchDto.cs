using Ecommerceapi.Dtos.Pagination;

namespace Ecommerceapi.Dtos.search_sort_filiter
{
    public class ProductQueryRequest
    {
        // Search
        public string? Search { get; set; }

        // Filter
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        // Sort
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }

        // Pagination
        public PaginatedRequestDto Page { get; set; } = new();
    }
}