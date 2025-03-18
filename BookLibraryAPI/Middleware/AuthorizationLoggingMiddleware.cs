using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

public class AuthorizationLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthorizationLoggingMiddleware> _logger;

    public AuthorizationLoggingMiddleware(RequestDelegate next, ILogger<AuthorizationLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        var token = authHeader?.StartsWith("Bearer ") == true ? authHeader.Substring("Bearer ".Length).Trim() : null;

        // Проверяем, аутентифицирован ли пользователь
        if (context.User.Identity.IsAuthenticated)
        {
            // Получаем роли пользователя
            var roles = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            // Логируем информацию о пользователе, его ролях и токене
            _logger.LogInformation("User {User} is authenticated with roles: {Roles}. JWT Token: {Token}",
                context.User.Identity.Name, string.Join(", ", roles), token);
        }
        else
        {
            _logger.LogInformation("User is not authenticated. JWT Token: {Token}", token);
        }

        // Вызываем следующий middleware в конвейере
        await _next(context);
    }
}