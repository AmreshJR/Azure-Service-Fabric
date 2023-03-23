using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationDAL.Utilities
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {

            httpContext.User.Identities.FirstOrDefault().AddClaim(new Claim("Custom", "your field"));
            await _next(httpContext);
        }
    }
}
