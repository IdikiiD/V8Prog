using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExpressMapper;
using v8proj.BissnessLogic.Infrastructure.Abstractions;
using v8proj.BissnessLogic.Interfaces.JWT;
using v8proj.BissnessLogic.Interfaces.User;
using v8proj.Core.Entities.User;
using v8proj.Core.Enums;
using v8proj.Core.Interface.User;
using v8proj.Core.Model.DTO.User;
using v8proj.Web.Model.DTO;

namespace v8proj.BissnessLogic.Services.AuthentificationService
{
    public class AuthenticationService : IAuthentificationSrevice
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        private readonly ICookiesService _cookiesService;

        public AuthenticationService(IUsersRepository usersRepository, IUserService userService,  IJwtService jwtService, ICookiesService cookiesService)
        {
            _usersRepository = usersRepository;
            _userService = userService;
            _jwtService = jwtService;
            _cookiesService = cookiesService;
        }

        public async Task<BaseResponse<bool>> SignUp(SignUpDto registrationDto)
        {
            var response = await _userService.CreateUserAsync(registrationDto);

            if (response.Data == null || response.Status != OperationStatus.Success)
                return new BaseResponse<bool>(false, response.Status, response.Message);

            return new BaseResponse<bool>(true, response.Status, "User signed up successfully");
        }

        public async Task<BaseResponse<bool>> SignIn(SignInDto loginDto)
        {
            // Получаем пользователя по email
            var user = await _usersRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
            {
                // Если пользователь не найден
                return new BaseResponse<bool>(false, OperationStatus.Error, "Email or password is incorrect.");
            }

            // Здесь предполагается, что есть проверка пароля (это не реализовано в примере, добавьте по мере необходимости)
            // Например: var isPasswordValid = CheckPassword(loginDto.Password, user.PasswordHash);

            // Если логин успешен
            var mappedUser = Mapper.Map<UserEf, UserDto>(user);
            return new BaseResponse<bool>(true, OperationStatus.Success, "User signed in successfully");
        }

        public async Task<BaseResponse<bool>> SendEmailConfirmation(string email)
        {
            // Реализуйте логику отправки подтверждения по email
            throw new NotImplementedException();
        }
        
        public async Task<BaseResponse<bool>> IsTokenValid()
        {
            return await _jwtService.IsTokenValid() ? 
                new BaseResponse<bool>(true, OperationStatus.Success, "Token is valid") : 
                new BaseResponse<bool>(false, OperationStatus.Error, "Token is invalid");
        }
        public async Task<BaseResponse<bool>> Logout()
        {
            _cookiesService.deleteCookie("jwt");
            return  new BaseResponse<bool>(true, OperationStatus.Success, "User signed out successfully");
        }
    }
}
