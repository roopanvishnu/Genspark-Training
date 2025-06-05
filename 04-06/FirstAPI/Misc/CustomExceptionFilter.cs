using FirstAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FirstAPI.Misc;

public class CustomExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        context.Result = new BadRequestObjectResult(new ErrorObjectDTO
        {
            ErrorNumber = 500,
            ErrorMessage = context.Exception.Message,
            ErrorType = context.Exception.GetType().ToString()
        }
        );
    }
}