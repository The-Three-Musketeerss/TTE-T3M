namespace TTE.Infrastructure.DTOs
{
    public class GenericResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        public GenericResponseDto(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
        public GenericResponseDto(bool success, string message, IEnumerable<T> data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public GenericResponseDto(bool success, string message)
        {
            Success = success;
            Message = message;
            Data = null; 
        }
    }
}
