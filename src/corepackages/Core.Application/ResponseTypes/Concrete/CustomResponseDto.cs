namespace Core.Application.ResponseTypes.Concrete;

public class CustomResponseDto<T>
{
    public T? Data { get; set; }
    public int StatusCode { get; set; }
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }

    public CustomResponseDto()
    {
    }

    public CustomResponseDto(T data, int statusCode, bool isSuccess)
    {
        Data = data;
        StatusCode = statusCode;
        IsSuccess = isSuccess;
    }

    public static CustomResponseDto<T> Success(int statusCode, T data, bool isSuccess)
    {
        return new CustomResponseDto<T>(data, statusCode, isSuccess);
    }

    public static CustomResponseDto<T> Success(int statusCode, T data, bool isSuccess, string message)
    {
        return new CustomResponseDto<T>(data, statusCode, isSuccess) { Message = message };
    }

    public static CustomResponseDto<T> Fail(int statusCode, string message)
    {
        return new CustomResponseDto<T>
        {
            StatusCode = statusCode,
            IsSuccess = false,
            Message = message
        };
    }
}

