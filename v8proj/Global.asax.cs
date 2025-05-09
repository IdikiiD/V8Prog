using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Unity;
using ExpressMapper;
using Microsoft.Ajax.Utilities;
using v8proj.Core.Entities.User;
using v8proj.Core.Model.DTO.User;

namespace v8proj
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes); // Регистрируем маршруты
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            UnityConfig.RegisterTypes(new UnityContainer());
            
            Mapper.Register<UserEf, UserDto>()
                .Member(dest => dest.Id, src => src.UserId) // <--- Главное: маппинг ID
                .Member(dest => dest.FullName, src => src.FullName)
                .Member(dest => dest.Email, src => src.Email)
                .Member(dest => dest.DateRegistered, src => src.DateRegistered)
                .Member(dest => dest.UserType, src => src.UserType)
                .Member(dest => dest.UserStatus, src => src.UserStatus)
                .Member(dest => dest.IsVerified, src => src.IsVerified)
                .Member(dest => dest.IsSignUpForLetters, src => src.IsSignUpForLetters);
             

        }
    }
}