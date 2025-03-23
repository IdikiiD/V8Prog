using System.Collections.Generic;
using System.Threading.Tasks;
using v8proj.Core.Enums.User;
using v8proj.Core.Model.DTO.User;
using v8proj.Web.Model.DTO;

namespace v8proj.BissnessLogic.Interfaces.User
{
    public interface IUserService
    {
        Task<BaseResponse<UserDto>> CreateUserAsync(SignUpDto signUpDto);
        Task<BaseResponse<UserDto>> UpdateUserAsync(UserDto userDto);
        Task<BaseResponse<bool>> DeleteUserByIdAsync(int id);
        Task<BaseResponse<UserDto>> GetUserByIdAsync(int id);
        Task<BaseResponse<UserDto>> GetUserByEmailAsync(string email);
        Task<BaseResponse<List<UserDto>>> GetUsersAsync(string email, UserType userType , int currentPage , int amountOfUsers); 
    }
}