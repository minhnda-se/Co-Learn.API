using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using CoLearn.Services.Exceptions;
using Microsoft.EntityFrameworkCore;
using ValidationException = CoLearn.Services.Exceptions.ValidationException;

namespace CoLearn.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var status = HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred.";

            switch (ex)
            {
                case NotFoundException nf:
                    status = HttpStatusCode.NotFound;
                    message = nf.Message;
                    break;
                case ValidationException ve:
                    status = HttpStatusCode.BadRequest;
                    message = ve.Message;
                    break;
                case DbUpdateException dbEx:
                    status = HttpStatusCode.Conflict;
                    message = "Database update failed.";
                    break;
                case BusinessException be:
                    status = (HttpStatusCode)be.StatusCode;
                    message = be.Message;
                    break;
            }

            var response = new
            {
                success = false,
                error = message,
                statusCode = (int)status,
                traceId = context.TraceIdentifier
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
