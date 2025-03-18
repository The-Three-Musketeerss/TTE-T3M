namespace TTE.Application.DTOs
{
    public class ProductPaginatedDto : GenericResponseDto<IEnumerable<ProductDto>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public ProductPaginatedDto(bool success, string message, IEnumerable<ProductDto> data, int page, int pageSize, int totalCount)
            : base(success, message, data)
        {
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        }
    }
}
