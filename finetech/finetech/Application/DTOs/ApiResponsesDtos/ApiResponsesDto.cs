namespace fintech.Application.DTOs.ApiResponsesDtos
{
    public class ApiResponsesDto<T>
    {
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public T Data { get; set; } = default!;
        public List<string> Errors { get; set; } = [];

        public ApiResponsesDto() 
        {
            IsSuccess = true;
            Errors = [];
        }

        public ApiResponsesDto(int statusCode, T data)
        {
            StatusCode = statusCode;
            IsSuccess = true;
            Data = data;
            Errors = [];
        }

        public ApiResponsesDto(int statusCode, List<string> errors)
        {
            StatusCode = statusCode;
            IsSuccess = false;
            Errors = errors;
        }

        public ApiResponsesDto(int statusCode, string error)
        {
            StatusCode = statusCode;
            IsSuccess = false;
            Errors = [error] ;
        }
    }
}
