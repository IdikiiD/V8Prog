using System.Threading.Tasks;
using v8proj.Core.Model.DTO.User;
using v8proj.Web.Model.DTO;

namespace v8proj.BissnessLogic.Interfaces.User
{
    public interface IAuthentificationSrevice
    {
         
        Task<BaseResponse<bool>> SignUp(SignUpDto registrationDto);
        Task<BaseResponse<bool>> SignIn(SignInDto loginDto);
        Task<BaseResponse<bool>> IsTokenValid();
        Task<BaseResponse<bool>> Logout();
     

    }
}