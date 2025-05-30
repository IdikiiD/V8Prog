using System;
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
using v8proj.BissnessLogic.Interfaces.Home;
using v8proj.BissnessLogic.Services.Home;
using v8proj.BissnessLogic.Interfaces.Posts;
using v8proj.BissnessLogic.Interfaces.Reports;
using v8proj.BissnessLogic.Services.Posts;
using v8proj.BissnessLogic.Services.Reports;
using v8proj.Core.Interface.Report;
using v8proj.DAL.Repositories.Report;

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
            RegisterDbContext(container);
            RegisterRepositories(container);
            RegisterServices(container);
        }

        private static void RegisterServices(IUnityContainer container)
        {
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IJwtService, JwtService>();
            container.RegisterType<ICookiesService, CookiesServices>();
            container.RegisterType<IEUseControlService, EUseControlService>();
            container.RegisterType<IPostService, PostService>(); 
            container.RegisterType<IHomeService, HomeService>();
            container.RegisterType<IReportRepository, ReportRepository>();
            container.RegisterType<IReportService, ReportService>();
            container.RegisterType<IAuthentificationSrevice, AuthenticationService>(); 
        }

        private static void RegisterRepositories(IUnityContainer container)
        {
            container.RegisterType<IUsersRepository, UsersRepository>();
            container.RegisterType<v8proj.Core.Interface.Support.ISupportRepository, v8proj.DAL.Repositories.SupportRepository>();
        }

        private static void RegisterDbContext(IUnityContainer container)
        {
            container.RegisterType<ApplicationDbContext>();
        }
    }
}