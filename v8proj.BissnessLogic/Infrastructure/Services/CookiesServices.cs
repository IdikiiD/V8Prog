using System;
using System.Web;
using v8proj.BissnessLogic.Infrastructure.Abstractions;

namespace v8proj.BissnessLogic.Infrastructure.Services
{
    public class CookiesServices : ICookiesService
    {
        public void setCookie(string key, string value, DateTime expires)
        {
            var cookie = new HttpCookie(key, value)
            {

                Expires = expires,
                HttpOnly = true,
                Secure = true
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public string getCookie(string key)
        {
            var cookie = HttpContext.Current.Request.Cookies[key];
            return cookie?.Value ?? string.Empty;
        }

        public void deleteCookie(string key)
        {
            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                var cookie = new HttpCookie(key)
                {
                    Expires = DateTime.Now.AddYears(-1)
                };
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
            
        }
    }
}