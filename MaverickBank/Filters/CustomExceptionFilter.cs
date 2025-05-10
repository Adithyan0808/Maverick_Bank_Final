
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace MaverickBank.Filters
{
    public class CustomExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var statusCode = 500;
            var message = "An unexpected error occurred.";

            if (context.Exception is UnauthorizedAccessException)
            {
                statusCode = 401;
                message = "Unauthorized";
            }
            else if (context.Exception is ArgumentException || context.Exception is InvalidOperationException)
            {
                statusCode = 400;
                message = context.Exception.Message;
            }
            else if (context.Exception is KeyNotFoundException || context.Exception.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                statusCode = 404;
                message = context.Exception.Message;
            }
            // Add specific handling for InsufficientBalanceException
            else if (context.Exception is MaverickBank.Exceptions.InsufficientBalanceException)
            {
                statusCode = 400; // Bad Request is appropriate for insufficient funds
                message = context.Exception.Message;
            }

            context.Result = new ObjectResult(new
            {
                error = message
            })
            {
                StatusCode = statusCode
            };

            context.ExceptionHandled = true;
        }
    }
}

