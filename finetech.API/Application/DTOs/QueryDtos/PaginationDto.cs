namespace fintech.API.Application.DTOs.QueryDtos
{
    public class PaginationDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
