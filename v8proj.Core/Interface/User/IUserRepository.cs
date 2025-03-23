using System.Collections.Generic;
using System.Threading.Tasks;
using v8proj.Core.Entities.User;
using v8proj.Core.Enums.User;
using v8proj.Core.Interface.GenericRep;

namespace v8proj.Core.Interface.User
{
    public interface IUsersRepository : IGenericRepository<UserEf>
    {
        Task<UserEf> GetByEmailAsync(string email);
        Task<IEnumerable<UserEf>> GetPaginatedUsersByEmailAndTypeAsync(string email, UserType userType, int page, int pageSize);
    }
}