using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Web;
using RenewalReminder.Services.Abstract;

namespace RenewalReminder.Filters
{
    public class SessionFilterAttribute : TypeFilterAttribute
    {

        public SessionFilterAttribute() : base(typeof(SessionFilter))
        {
        }
        private class SessionFilter : IAuthorizationFilter
        {
            public SessionFilter()
            {
            }

            public void OnAuthorization(AuthorizationFilterContext filterContext)
            {
                if (filterContext == null)
                {
                    throw new ArgumentNullException(nameof(filterContext));
                }

                var allowAnonym = (filterContext.ActionDescriptor as ControllerActionDescriptor).MethodInfo.GetCustomAttributes(inherit: true).Any(a => a.GetType().Equals(typeof(AllowAnonymousAttribute)))
                                || (filterContext.Filters.Any(a => a.GetType().Equals(typeof(AllowAnonymousFilter))));

                var actionUrl = "/" + (filterContext.ActionDescriptor as ControllerActionDescriptor).ControllerName + "/" + (filterContext.ActionDescriptor as ControllerActionDescriptor).ActionName;
                if (!allowAnonym)
                {
                    var userAccessor = filterContext.HttpContext.RequestServices.GetService<IUserAccessor>();
                    if (!userAccessor.IsLogined)
                    {
                        var returnUrl = filterContext.HttpContext.Request.Path + filterContext.HttpContext.Request.QueryString;
                        if (returnUrl != "/")
                        {
                            returnUrl = "?returnUrl=" + HttpUtility.UrlEncode(filterContext.HttpContext.Request.Path + filterContext.HttpContext.Request.QueryString);
                        }
                        if (IsAjax(filterContext.HttpContext.Request))
                        {
                            filterContext.Result = new JsonResult(new
                            {
                                HasError = true,
                                Redirect = "/Home/Login" + returnUrl
                            });
                            return;
                        }
                        filterContext.Result = new RedirectResult("/Home/Login" + returnUrl);
                        return;
                    }
                    filterContext.HttpContext.Items["User"] = userAccessor.User;
                }
            }
            private static bool IsAjax(HttpRequest req)
            {
                return req != null && req.Headers != null && req.Headers["X-Requested-With"] == "XMLHttpRequest";
            }
        }
    }
}

