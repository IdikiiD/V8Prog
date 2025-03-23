using System;
using System.Web.ApplicationServices;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;
using v8proj.BissnessLogic.Infrastructure.Abstractions;
using v8proj.BissnessLogic.Infrastructure.Services;
using v8proj.BissnessLogic.Interfaces.JWT;
using v8proj.BissnessLogic.Interfaces.User;
using v8proj.BissnessLogic.Services.JWTService;
using v8proj.BissnessLogic.Services.User;
using v8proj.Core.Interface.User;

using v8proj.DAL;
using v8proj.DAL.Repositories.User;
using AuthenticationService = v8proj.BissnessLogic.Services.AuthentificationService.AuthenticationService;

namespace v8proj
{
    public static class UnityConfig
    {
        #region Unity Container

        private static Lazy<IUnityContainer> _container =
            new Lazy<IUnityContainer>(() =>
            {
                var container = new UnityContainer();
                RegisterTypes(container);
                return container;
            });

        public static IUnityContainer Container => _container.Value;

        #endregion


        public static void RegisterTypes(IUnityContainer container)
        {
            /*
             * NOTE: To load from web.config uncomment the line below.
             * Make sure to add a Unity.Configuration to the using statements.
             *container.LoadConfiguration();
             */

            RegisterDbContext(container);
            RegisterRepositories(container);
            RegisterServices(container);
        }

        private static void RegisterServices(IUnityContainer container)
        {
            //bll
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IJwtService, JwtService>();

            //infrastructure
            container.RegisterType<ICookiesService, CookiesServices>();

        }

        private static void RegisterRepositories(IUnityContainer container)
        {
            container.RegisterType<IUsersRepository, UsersRepository>();
            container.RegisterType<IAuthentificationSrevice, AuthenticationService>();

        }

        private static void RegisterDbContext(IUnityContainer container)
        {
            container.RegisterType<ApplicationDbContext>();
        }
    }
}
