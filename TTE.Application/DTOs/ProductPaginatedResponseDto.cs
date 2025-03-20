namespace TTE.Application.DTOs
{
    public class ProductPaginatedResponseDto : GenericResponseDto<IEnumerable<ProductResponseDto>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public ProductPaginatedResponseDto(bool success, string message, IEnumerable<ProductResponseDto> data, int page, int pageSize, int totalCount)
            : base(success, message, data)
        {
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        }
    }
}
