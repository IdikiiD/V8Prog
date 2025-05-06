// Файл: v8proj.BissnessLogic/Interfaces/User/IUserService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using v8proj.Core.Enums.User;
using v8proj.Core.Model.DTO.User;
using v8proj.Web.Model.DTO; // Для BaseResponse

namespace v8proj.BissnessLogic.Interfaces.User
{
    public interface IUserService
    {
        Task<BaseResponse<UserDto>> CreateUserAsync(SignUpDto signUpDto);
        Task<BaseResponse<UserDto>> UpdateUserAsync(UserDto userDto);
        Task<BaseResponse<bool>> DeleteUserByIdAsync(int id);
        Task<BaseResponse<UserDto>> GetUserByIdAsync(int id);
        Task<BaseResponse<UserDto>> GetUserByEmailAsync(string email);
        // --- ИЗМЕНЕНИЕ НАЧАЛО ---
        Task<BaseResponse<List<UserDto>>> GetUsersAsync(string searchTerm, UserType userType, int currentPage, int amountOfUsers);
        // --- ИЗМЕНЕНИЕ КОНЕЦ ---
        Task<BaseResponse<bool>> BanUserAsync(int userId);
        Task<BaseResponse<bool>> UnbanUserAsync(int userId);
    }
}