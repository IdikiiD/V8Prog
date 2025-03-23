using System;

namespace v8proj.BissnessLogic.Infrastructure.Abstractions
{
    public interface ICookiesService
    {
        void setCookie(string key, string value, DateTime expires);
        string getCookie(string key);
        void deleteCookie(string key);
    }
}