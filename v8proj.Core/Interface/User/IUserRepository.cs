
using System.Collections.Generic;
using System.Threading.Tasks;
using v8proj.Core.Entities.User;
using v8proj.Core.Enums.User; 

namespace v8proj.Core.Interface.User
{
    public interface IUsersRepository
    {
        Task<UserEf> CreateAsync(UserEf entity);
        Task<UserEf> GetByIdAsync(int id);
        Task<UserEf> GetByEmailAsync(string email);
        Task<IEnumerable<UserEf>> GetPaginatedUsersBySearchTermAndTypeAsync(string searchTerm, UserType userType, int page, int pageSize);
        Task<UserEf> UpdateAsync(UserEf entity);
        Task DeleteByIdAsync(int id);
    }
}