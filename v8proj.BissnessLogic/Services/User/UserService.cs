using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using ExpressMapper;
using v8proj.BissnessLogic.Interfaces.User;
using v8proj.Core.Entities.User;
using v8proj.Core.Enums;
using v8proj.Core.Enums.User;
using v8proj.Core.Interface.User;
using v8proj.Core.Model.DTO.User;
using v8proj.Web.Model.DTO; // Для BaseResponse
using v8proj.Core.Enums.Entinity;

namespace v8proj.BissnessLogic.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _usersRepository;
        public UserService(IUsersRepository usersRepository)
            => _usersRepository = usersRepository;

        public async Task<BaseResponse<UserDto>> CreateUserAsync(SignUpDto signUpDto)
        {
            var response = await GetUserByEmailAsync(signUpDto.Email);

            if (response.Data != null)
                return new BaseResponse<UserDto>(null, OperationStatus.Error, "User with this email already exists");

            var userEf = Mapper.Map<SignUpDto, UserEf>(signUpDto);

            var userEfFromDb = await _usersRepository.CreateAsync(userEf);

            return userEfFromDb == null ?
                new BaseResponse<UserDto>(null, OperationStatus.Error, "User not created") :
                new BaseResponse<UserDto>(Mapper.Map<UserEf, UserDto>(userEfFromDb), OperationStatus.Success, "User Created");
        }

        public async Task<BaseResponse<UserDto>> GetUserByIdAsync(int id)
        {
            var userEf = await _usersRepository.GetByIdAsync(id);

            return userEf == null ?
                new BaseResponse<UserDto>(null, OperationStatus.Error, "User not found") :
                new BaseResponse<UserDto>(Mapper.Map<UserEf, UserDto>(userEf), OperationStatus.Success, "User got by id");
        }

        public async Task<BaseResponse<UserDto>> GetUserByEmailAsync(string email)
        {
            var userEf = await _usersRepository.GetByEmailAsync(email);

            return userEf == null ?
                new BaseResponse<UserDto>(null, OperationStatus.Error, "User not found") :
                new BaseResponse<UserDto>(Mapper.Map<UserEf, UserDto>(userEf), OperationStatus.Success, "User got by email");
        }

        public async Task<BaseResponse<List<UserDto>>> GetUsersAsync(string searchTerm, UserType userType, int currentPage, int amountOfUsers)
        {
            // Вызываем обновленный метод репозитория
            var userEfs = (await _usersRepository.GetPaginatedUsersBySearchTermAndTypeAsync(searchTerm, userType, currentPage, amountOfUsers)).ToList();

            if (!userEfs.Any())
            {
                return new BaseResponse<List<UserDto>>(null, OperationStatus.Success, "Users not found");
            }

            // Маппинг UserEf в UserDto
            var userDtos = userEfs.Select(userEf => Mapper.Map<UserEf, UserDto>(userEf)).ToList();
            return new BaseResponse<List<UserDto>>(userDtos, OperationStatus.Success, "Users got");
        }

        public async Task<BaseResponse<UserDto>> UpdateUserAsync(UserDto userDto)
        {
            var userEf = Mapper.Map<UserDto, UserEf>(userDto);

            var userEfFromDb = await _usersRepository.UpdateAsync(userEf);

            return userEfFromDb == null ?
                new BaseResponse<UserDto>(null, OperationStatus.Error, "User not Updated") :
                new BaseResponse<UserDto>(Mapper.Map<UserEf, UserDto>(userEfFromDb), OperationStatus.Success, "User Updated");
        }

        public async Task<BaseResponse<bool>> DeleteUserByIdAsync(int id)
        {
            await _usersRepository.DeleteByIdAsync(id);

            return new BaseResponse<bool>(true, OperationStatus.Success, "User Deleted");
        }
        public async Task<BaseResponse<bool>> BanUserAsync(int userId)
        {
            try
            {
                var userEf = await _usersRepository.GetByIdAsync(userId);
                if (userEf == null)
                {
                    return new BaseResponse<bool>(false, OperationStatus.Error, "User not found.");
                }

                userEf.UserStatus = EntityStatus.Banned;

                await _usersRepository.UpdateAsync(userEf);

                return new BaseResponse<bool>(true, OperationStatus.Success, "User was banned.");
            }
            catch (System.Exception ex)
            {
                return new BaseResponse<bool>(false, OperationStatus.Error, "Error occured while banning user");
            }
        }

        public async Task<BaseResponse<bool>> UnbanUserAsync(int userId)
        {
            try
            {
                var userEf = await _usersRepository.GetByIdAsync(userId);
                if (userEf == null)
                {
                    return new BaseResponse<bool>(false, OperationStatus.Error, "User for unban was not found.");
                }

                userEf.UserStatus = EntityStatus.Active;

                await _usersRepository.UpdateAsync(userEf);

                return new BaseResponse<bool>(true, OperationStatus.Success, "User was unbanned.");
            }
            catch (System.Exception ex)
            {
                return new BaseResponse<bool>(false, OperationStatus.Error, "Error occured while unbanning user.");
            }
        }
        public async Task<BaseResponse<bool>> MakeUserAdminAsync(int userId)
        {
            try
            {
                var userEf = await _usersRepository.GetByIdAsync(userId);
                if (userEf == null)
                {
                    return new BaseResponse<bool>(false, OperationStatus.Error, "User not found.");
                }

                userEf.UserType = UserType.Admin; // Устанавливаем тип пользователя в "Admin"
                await _usersRepository.UpdateAsync(userEf);
                return new BaseResponse<bool>(true, OperationStatus.Success, "User is now an administrator.");
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>(false, OperationStatus.Error, $"Error: {ex.Message}");
            }
        }

        public async Task<BaseResponse<bool>> RevokeUserAdminAsync(int userId)
        {
            try
            {
                var userEf = await _usersRepository.GetByIdAsync(userId);
                if (userEf == null)
                {
                    return new BaseResponse<bool>(false, OperationStatus.Error, "User not found.");
                }

                userEf.UserType = UserType.User; // Снимаем права администратора
                await _usersRepository.UpdateAsync(userEf);
                return new BaseResponse<bool>(true, OperationStatus.Success, "Administrator rights revoked.");
            }
            catch (Exception ex)
            {
                return new BaseResponse<bool>(false, OperationStatus.Error, $"Error: {ex.Message}");
            }
        }
    }
    
    
}

