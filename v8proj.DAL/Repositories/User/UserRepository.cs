using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<UserEf>> GetPaginatedUsersBySearchTermAndTypeAsync(string searchTerm, UserType userType, int page, int pageSize)
        {
            var query = _context.Users.AsQueryable();

            if (userType != UserType.None)
            {
                query = query.Where(x => x.UserType == userType);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                string searchTermLower = searchTerm.ToLower();
                query = query.Where(x => x.FullName.ToLower().Contains(searchTermLower) ||
                             x.Email.ToLower().Contains(searchTermLower));
            }

            // Явно проверяем, есть ли элементы, прежде чем применять Skip и Take
            if (await query.AnyAsync())
            {
                return await query.OrderByDescending(x => x.UserId)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();
            }
            else
            {
                return new List<UserEf>(); // Возвращаем пустой список.
            }
        }

        public async Task<UserEf> UpdateAsync(UserEf entity)
        {
            var existingUser = await _context.Users.FindAsync(entity.UserId);
            if (existingUser == null) return null;

            existingUser.FullName = entity.FullName;
            existingUser.Email = entity.Email; 
            existingUser.UserType = entity.UserType;
            existingUser.UserStatus = entity.UserStatus;

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
