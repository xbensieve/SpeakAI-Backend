    using Common.Enum;
    using System.Text.Json;

    namespace DTO.DTO
    {
        public class ResponseDTO
        {
            public StatusCodeEnum  StatusCode { get; set; }
            public string? Message { get; set; } = string.Empty;
            public bool IsSuccess { get; set; }
            public object? Result { get; set; }
            public override string ToString() => JsonSerializer.Serialize(this);
            public ResponseDTO(string message, StatusCodeEnum statusCode, bool success = false, object? result = null)
            {
                Message = message;
                StatusCode = statusCode;
                IsSuccess = success;
                Result = result;
            }
        }
    }
