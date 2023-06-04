using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Courrier.Filters
{
    public class AuthorizeSessionFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var isLoggedIn = context.HttpContext.Session.GetString("UserId");
            if (isLoggedIn == null)
            {
                var returnUrl = context.HttpContext.Request.Path.ToString();

                // L'utilisateur n'est pas connecté, redirigez-le vers la page de connexion
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "area", "" },
                { "page", "/Login" },
                { "returnUrl", returnUrl }
            });
            }
        }
    }
}
