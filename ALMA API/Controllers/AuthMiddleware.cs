using System.Security.Claims;

namespace ALMA_API.Controllers;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public AuthMiddleware(IConfiguration appConfiguration, RequestDelegate next)
    {
        _configuration = appConfiguration;
        _next = next;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        if (!Authorize(httpContext))
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            HttpResponseWritingExtensions.WriteAsync(httpContext.Response, "{\"message\": \"Unauthorized\"}").Wait();
            return;
        }
        await _next(httpContext);
    }

    private bool Authorize(HttpContext httpContext)
    {
        var header = httpContext.Request.Headers["Authorization"];
        if (header.Count == 0) return true;
        var tokenValue = Convert.ToString(header).Trim().Split(" "); 
        if (tokenValue.Length < 1)
            return true; // Authorize Without Token -- Is AllowAnonymous
        var token = tokenValue[0];
        if (int.TryParse(httpContext.User.FindFirstValue(ClaimTypes.UserData), out var id))
            httpContext.Items["id"] = id;
        //TODO make database validation token
        return true;
    }
}