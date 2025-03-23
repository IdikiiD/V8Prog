using System.Threading.Tasks;
using v8proj.Core.Model.DTO.User;

namespace v8proj.BissnessLogic.Interfaces.JWT
{
    public interface IJwtService
    {
        Task<string>GenerateJwtToken(UserDto user);
        Task<int> GetUserIdFromToken( );
        Task<int> GetUserRoleIdFromToken();
        Task<string> GetUserEmailFromToken();
        Task<bool> IsTokenValid();
    }
}