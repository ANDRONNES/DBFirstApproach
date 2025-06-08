using System.Net.Http.Headers;
using System.Text;
using DBFirst.Models;

namespace DBFirst.Middlewares;

public class BasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    private const string Realm = "My Realm";
    private readonly AgencyContext _context;
    public BasicAuthMiddleware(RequestDelegate next)
    {
        _next = next;
        _context = new AgencyContext();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            var authHeader = AuthenticationHeaderValue.Parse(context.Request.Headers["Authorization"]);

            if (authHeader.Scheme.Equals("basic", StringComparison.OrdinalIgnoreCase) &&
                authHeader.Parameter != null)
            {
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');
                var username = credentials[0];
                var password = credentials[1];

                if (IsAuthorized(username,password)) // Example validation
                {
                    await _next(context);
                    return;
                }
            }
        }
        context.Response.Headers["WWW-Authenticate"] = $"Basic realm=\"{Realm}\"";
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    }

    private bool IsAuthorized(string username, string password)
    {
        return username == "admin" && password == "password";
            
    }
}