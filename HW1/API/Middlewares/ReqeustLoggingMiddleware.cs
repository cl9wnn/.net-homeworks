using System.Diagnostics;
using UAParser;

namespace API.Middlewares;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await next(context);

        stopwatch.Stop();

        var uaParser = Parser.GetDefault();
        var clientInfo = uaParser.Parse(context.Request.Headers.UserAgent);
        var deviceType = clientInfo.Device.IsSpider
            ? "Bot"
            : string.IsNullOrEmpty(clientInfo.Device.Family)
                ? "Unknown"
                : clientInfo.OS.Family;

        var log = new
        {
            timestamp = DateTime.UtcNow,
            method = context.Request.Method,
            path = context.Request.Path.ToString(),
            statusCode = context.Response.StatusCode,
            duration = stopwatch.ElapsedMilliseconds,
            ip = context.Connection.RemoteIpAddress?.ToString(),
            device = deviceType
        };

        logger.LogInformation("{@RequestLog}", log);
    }
}