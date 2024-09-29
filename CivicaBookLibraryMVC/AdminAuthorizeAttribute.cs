using CivicaBookLibraryMVC.Implementation;
using CivicaBookLibraryMVC.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CivicaBookLibraryMVC
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AdminAuthorizeAttribute : Attribute, IAuthorizationFilter 
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            HttpContext context1 = context.HttpContext;
            IJwtTokenHandler tokenHandler = new JwtTokenHandler();

            string token = context1.Request.Cookies["jwtToken"];

            if(token != null)
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var isAdmin = jwtToken.Claims.First(claim => claim.Type == "Admin").Value;
                if (isAdmin == "True")
                {
                    return;
                }
                else
                {
                    context1.Response.Cookies.Delete("jwtToken");
                    context1.Response.Cookies.Delete("userId");
                }
            }
            else
            {
                context1.Response.Cookies.Delete("jwtToken");
                context1.Response.Cookies.Delete("userId");
            }

            // Unauthorized access, redirect or return unauthorized result
            context.Result = new UnauthorizedResult();
        }
    }
}