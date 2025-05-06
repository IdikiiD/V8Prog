// Файл: v8proj.DAL/Repositories/User/UsersRepository.cs
using System.Collections.Generic;
using System.Data.Entity; // Для ToListAsync(), SingleOrDefaultAsync() и т.д. если это EF6, или Microsoft.EntityFrameworkCore для EF Core
using System.Linq;
using System.Threading.Tasks;
using ExpressMapper; // Этот using здесь не нужен, если маппинг происходит только в UpdateAsync
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

        // --- ИЗМЕНЕНИЕ НАЧАЛО ---
        public async Task<IEnumerable<UserEf>> GetPaginatedUsersBySearchTermAndTypeAsync(string searchTerm, UserType userType, int page, int pageSize)
        {
            var query = _context.Users.AsQueryable(); // Начинаем с IQueryable

            // Фильтрация по типу пользователя
            if (userType != UserType.None) // Предполагаем, что UserType.None означает "не фильтровать по типу"
            {
                query = query.Where(x => x.UserType == userType);
            }

            // Фильтрация по поисковой строке (имя ИЛИ email)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                string searchTermLower = searchTerm.ToLower(); // Для регистронезависимого поиска
                query = query.Where(x => x.FullName.ToLower().Contains(searchTermLower) ||
                                         x.Email.ToLower().Contains(searchTermLower));
            }

            // Сортировка и пагинация
            return await query.OrderByDescending(x => x.UserId) // Или OrderBy(x => x.FullName)
                              .Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .ToListAsync();
        }
        // --- ИЗМЕНЕНИЕ КОНЕЦ ---

        public async Task<UserEf> UpdateAsync(UserEf entity)
        {
            // Важно: Этот метод мапит entity на existingUser.
            // Убедитесь, что entity содержит только те поля, которые должны быть обновлены,
            // или настройте ExpressMapper для игнорирования определенных полей при обновлении.
            var existingUser = await _context.Users.FindAsync(entity.UserId);
            if (existingUser == null) return null;

            // Если вы используете EF Core, отслеживание изменений происходит автоматически,
            // и этот маппинг может быть заменен на прямое присвоение свойств:
            // existingUser.FullName = entity.FullName;
            // existingUser.Email = entity.Email; // и т.д. для обновляемых полей
            // Если вы используете ExpressMapper для обновления, убедитесь, что он настроен правильно
            // и не перезаписывает, например, PasswordHash или DateRegistered ненужными значениями из entity.
            Mapper.Map(entity, existingUser); // Если entity - это UserEf, полученный из UserDto, то здесь может быть проблема с PasswordHash

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