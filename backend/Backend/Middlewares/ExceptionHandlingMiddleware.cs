using Backend.Common;
using System.Net;

namespace Backend.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (Exception e)
            {

                if (e is BadRequestException ||
                    e is NotFoundException ||
                    e is UnauthorizedException ||
                    e is ForbiddenException ||
                    e is ConflictException)
                {
                    _logger.LogWarning(
                        "Handled exception: {ExceptionType} {Method} {Path}. Message: {Message}",
                        e.GetType().Name,
                        context.Request.Method,
                        context.Request.Path,
                        e.Message
                    );
                }
                else
                {
                    _logger.LogError(
                        e,
                        "Unhandled exception occurred while processing request: {Method} {Path}. Exception: {ExceptionType}",
                        context.Request.Method,
                        context.Request.Path,
                        e.GetType().Name
                    );
                }

                await HandleExceptionAsync(context, e);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            HttpStatusCode httpStatusCode;
            string message;
            IEnumerable<string>? errors = null;

            switch (exception)
            {
                //404
                case NotFoundException:
                    httpStatusCode = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;
                //401
                case UnauthorizedException:
                    httpStatusCode = HttpStatusCode.Unauthorized;
                    message = exception.Message;
                    break;
                //400
                case InvalidGoogleTokenException:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case BadRequestException badRequest:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    errors = badRequest.Errors;
                    break;
                //403
                case ForbiddenException:
                    httpStatusCode = HttpStatusCode.Forbidden;
                    message = exception.Message;
                    break;
                //409
                case ConflictException:
                    httpStatusCode = HttpStatusCode.Conflict;
                    message = exception.Message;
                    break;
                case InvalidOperationException:
                    httpStatusCode = HttpStatusCode.InternalServerError;
                    message = exception.Message;
                    break;
                //500
                default:
                    httpStatusCode = HttpStatusCode.InternalServerError;
                    message = "An unexpected error occurred.";
                    break;
            }

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)httpStatusCode;

            var response = errors != null ? ApiResponse<object>.FailureResponse(message, errors) : ApiResponse<object>.FailureResponse(message);

            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}
