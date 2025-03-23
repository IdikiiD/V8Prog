using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ExpressMapper;
using v8proj.Core.Entities.User;
using v8proj.Core.Enums.User;
using v8proj.Core.Interface.User;

namespace v8proj.DAL.Repositories.User

{
    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext _context;

        public UsersRepository(ApplicationDbContext context) =>
            _context = context;

        public async Task<UserEf> CreateAsync(UserEf entity)
        {
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<UserEf> GetByIdAsync(int id) =>
            await _context.Users.FindAsync(id);

        public async Task<UserEf> GetByEmailAsync(string email) => 
            await _context.Users.SingleOrDefaultAsync(x => x.Email == email);

        public async Task<IEnumerable<UserEf>> GetPaginatedUsersByEmailAndTypeAsync(string email, UserType userType, int page, int pageSize)
        {
            var request = _context.Users.AsQueryable();
            if (!string.IsNullOrEmpty(email)) request = request.Where(x => x.Email.Contains(email));
            if (userType != UserType.None) request = request.Where(x => x.UserType == userType);

            return await request.OrderByDescending(x => x.UserId).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<UserEf> UpdateAsync(UserEf entity)
        {
            var existingUser = await _context.Users.FindAsync(entity.UserId);
            if (existingUser == null) return null;

            Mapper.Map(entity, existingUser);

            await _context.SaveChangesAsync();
            return existingUser;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}