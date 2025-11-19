using Core.CrossCuttingConcerns.Exceptions.HttpProblemDetails;
using Core.CrossCuttingConcerns.Exceptions.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.CrossCuttingConcerns.Exceptions.Handlers;

public class HttpExceptionHandler : ExceptionHandler
{
    private readonly HttpResponse _response;

    public HttpExceptionHandler(HttpResponse response)
    {
        _response = response;
    }

    protected override Task HandleException(BusinessException businessException)
    {
        _response.StatusCode = StatusCodes.Status400BadRequest;
        string details = new BusinessProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "-",
            Title = "Business exception",
            Detail = businessException.Message,
            Instance = ""
        }.ToString();
        return _response.WriteAsync(details);
    }

    protected override Task HandleException(ValidationException validationException)
    {
        _response.StatusCode = StatusCodes.Status400BadRequest;
        string details = new Core.CrossCuttingConcerns.Exceptions.HttpProblemDetails.ValidationProblemDetails(validationException.Errors).ToString();
        return _response.WriteAsync(details);
    }

    protected override Task HandleException(AuthorizationException authorizationException)
    {
        _response.StatusCode = StatusCodes.Status401Unauthorized;
        string details = new AuthorizationProblemDetails(authorizationException.Message).ToString();
        return _response.WriteAsync(details);
    }

    protected override Task HandleException(NotFoundException notFoundException)
    {
        _response.StatusCode = StatusCodes.Status404NotFound;
        string details = new NotFoundProblemDetails(notFoundException.Message).ToString();
        return _response.WriteAsync(details);
    }

    protected override Task HandleException(Exception exception)
    {
        _response.StatusCode = StatusCodes.Status500InternalServerError;
        string details = new InternalServerErrorProblemDetails(exception.Message).ToString();
        return _response.WriteAsync(details);
    }
}

