using System.Collections.Generic;
using System.Linq;
using System.Data.Entity; 
using v8proj.BissnessLogic.Interfaces.Posts; 
using v8proj.Core.Entities;
using v8proj.Core.Entities.User; 
using v8proj.DAL; 

namespace v8proj.BissnessLogic.Services.Posts 
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context) 
        {
            _context = context;
        }

        public List<Post> GetPostsFilteredAndSorted(string category)
        {
            var postsQuery = _context.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                postsQuery = postsQuery.Where(p => p.Category == category);
            }

            return postsQuery
                .Where(p => !string.IsNullOrEmpty(p.ImagePath1))
                .OrderByDescending(p => p.CreatedAt)
                .ToList();
        }

        public Post GetPostById(int id)
        {
            return _context.Posts.Find(id);
        }

        public void AddPost(Post post)
        {
            _context.Posts.Add(post);
        }

        public Post GetPostWithFavorites(int postId)
        {
            return _context.Posts.Include(p => p.FavoritedBy).FirstOrDefault(p => p.Id == postId);
        }

        public UserEf GetUserWithFavorites(string userEmail)
        {
            return _context.Users
                .Include(u => u.FavoritePosts) 
                .FirstOrDefault(u => u.Email == userEmail);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}