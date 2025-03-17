﻿namespace TTE.Application.DTOs
{
    public class GenericResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public GenericResponseDto(bool success, string message, T data)
        {
            Success = success;
            Message = message;
            Data = data;
        }
    }
}
