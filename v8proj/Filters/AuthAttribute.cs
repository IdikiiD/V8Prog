using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using v8proj.Core.Enums.User; 

namespace v8proj.Web.Filters
{
    public class AuthAttribute : AuthorizeAttribute
    {
        public string Roles { get; set; }
        
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }
            
            HttpCookie userEmailCookie = httpContext.Request.Cookies["UserEmail"];
            if (userEmailCookie == null || string.IsNullOrEmpty(userEmailCookie.Value))
            {
                return false; 
            }

            if (!string.IsNullOrEmpty(Roles)) 
            {
                var requiredRoles = Roles.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim())
                    .ToList();
                HttpCookie userRoleCookie = httpContext.Request.Cookies["UserRole"];
                string currentUserRole = userRoleCookie?.Value; 
                
                if (string.IsNullOrEmpty(currentUserRole) || !requiredRoles.Contains(currentUserRole, StringComparer.OrdinalIgnoreCase))
                {
                    return false; 
                }
            }
            return true; 
        }
// ...

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            HttpCookie userEmailCookie = filterContext.HttpContext.Request.Cookies["UserEmail"];
            if (userEmailCookie == null || string.IsNullOrEmpty(userEmailCookie.Value))
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "Controller", "Auth" },
                        { "Action", "SignIn" } 
                    });
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "Controller", "Auth" }, 
                        { "Action", "AccessDenied" } 
                    });
            }
        }
    }
}