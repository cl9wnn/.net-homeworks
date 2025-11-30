namespace API.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var requestId = Guid.NewGuid().ToString();

            logger.LogError(ex, 
                "Unhandled exception (message: {message} RequestId: {RequestId}, Path: {Path})",
                ex.Message,
                requestId,
                context.Request.Path);
            
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            
            var response = new
            {
                status = 500,
                error = "Internal Server Error",
                message = "An unexpected error has occured. We are already working to resolve it.",
                requestId
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }

}