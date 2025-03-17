namespace TTE.Application.DTOs
{
    public class GenericRequestDto<T>
    {
        public T Data { get; set; }

        public GenericRequestDto(T data)
        {
            Data = data;
        }
    }
}
