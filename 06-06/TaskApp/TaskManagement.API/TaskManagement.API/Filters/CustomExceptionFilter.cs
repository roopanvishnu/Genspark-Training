// Filters/CustomExceptionFilter.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskManagement.API.DTOs.Response;

namespace TaskManagement.API.Filters;

public class CustomExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        var response = new ApiResponse<string?>
        {
            Success = false,
            Message = "An error occurred while processing your request.",
            ResultsCount = 0,
            Data = null,
            Errors = new ApiErrorDto
            {
                Message = context.Exception.Message,
                Type = context.Exception.GetType().Name
            }
        };

        context.Result = new ObjectResult(response)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };

        context.ExceptionHandled = true;
    }
}