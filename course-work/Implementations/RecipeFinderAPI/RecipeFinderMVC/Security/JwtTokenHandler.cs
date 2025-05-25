
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;

namespace RecipeFinderMVC.Security
{
    public class JwtTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public JwtTokenHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var jwt = session.GetString("JWT");

            if (!string.IsNullOrWhiteSpace(jwt))
            {
                // 🔒 Проверка дали токенът е изтекъл
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadToken(jwt) as JwtSecurityToken;
                var expiration = jwtToken?.ValidTo;

                if (expiration != null && expiration < DateTime.UtcNow)
                {
                    // Токенът е изтекъл – изтриваме сесията и cookie-то
                    session.Remove("JWT");
                    await _httpContextAccessor.HttpContext.SignOutAsync("CookieLogin");

                    // Прекъсваме заявката и пренасочваме
                    var response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized)
                    {
                        RequestMessage = request
                    };
                    return response;
                }

                // Ако токенът е валиден – добавяме го към Authorization header
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            }

            return await base.SendAsync(request, cancellationToken);
        }
        /* protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
         {
             var token = _httpContextAccessor.HttpContext.Session.GetString("JWT");
             if (!string.IsNullOrEmpty(token))
             {
                 request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
             }
             Console.WriteLine("JWT in handler: " + token);

             return await base.SendAsync(request, cancellationToken);
         }*/
    }
    
}
