using System.Net;
using Newtonsoft.Json;
using static BookLibraryClient.Controllers.AuthController;

namespace BookLibraryClient.Middleware
{
    public class TokenRefreshMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenRefreshMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Сохраняем оригинальный запрос
            var originalRequest = context.Request;

            // Перехватываем ответ
            await _next(context);

            // Проверяем статус ответа
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                // Получаем refresh токен из куки
                var refreshToken = context.Request.Cookies["refreshToken"];
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    // Выполняем запрос на обновление токена
                    var newTokens = await RefreshAccessToken(refreshToken);

                    if (newTokens != null)
                    {
                        // Сохраняем новый access токен в контексте
                        context.Response.Cookies.Append("accessToken", newTokens.token);

                        // Повторяем оригинальный запрос с новым access токеном
                        context.Request.Headers["Authorization"] = $"Bearer {newTokens.token}";
                        context.Response.Clear();
                        context.Response.StatusCode = (int)HttpStatusCode.OK;

                        // Выполняем повторный запрос
                        await _next(context);
                    }
                }
            }
        }

        private async Task<TokenResponse> RefreshAccessToken(string refreshToken)
        {
            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync("https://yourapi.com/refresh-token", new { RefreshToken = refreshToken });

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TokenResponse>(jsonResponse);
            }

            return null;
        }
    }
}
