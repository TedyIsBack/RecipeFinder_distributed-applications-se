using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
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
            var context = _httpContextAccessor.HttpContext;
            var token = context?.Session.GetString("JWT");

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();

                if (handler.CanReadToken(token))
                {
                    var jwt = handler.ReadJwtToken(token);
                    var expiration = jwt.ValidTo;

                    if (expiration < DateTime.UtcNow)
                    {
                        context.Session.Remove("JWT");
                        await context.SignOutAsync("CookieLogin");

                        return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                        {
                            RequestMessage = request,
                            ReasonPhrase = "JWT expired"
                        };
                    }
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
