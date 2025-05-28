using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using v8proj.BissnessLogic.Interfaces.Posts;
using v8proj.Core.Entities;
using v8proj.Core.Entities.User;
using v8proj.Core.Model.DTO;
using v8proj.Core.Enums;
using v8proj.DAL;
using v8proj.Web.Model.DTO;

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
            _context.SaveChanges();
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

        public async Task<BaseResponse<object>> DeletePost(int postId)
        {
            try
            {
                var post = await _context.Posts.FindAsync(postId);

                if (post == null)
                {
                    System.Diagnostics.Debug.WriteLine($"DeletePost: Post with ID {postId} not found.");
                    return new BaseResponse<object>(null, OperationStatus.Error, "Post not found.");
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
                System.Diagnostics.Debug.WriteLine($"DeletePost: Post with ID {postId} successfully deleted.");

                return new BaseResponse<object>(null, OperationStatus.Success, "Post successfully deleted.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"CRITICAL ERROR in PostService.DeletePost for PostId {postId}:");
                System.Diagnostics.Debug.WriteLine($"Message: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Source: {ex.Source}");
                System.Diagnostics.Debug.WriteLine($"Method: {ex.TargetSite?.Name}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"INNER EXCEPTION:");
                    System.Diagnostics.Debug.WriteLine($"  Message: {ex.InnerException.Message}");
                    System.Diagnostics.Debug.WriteLine($"  Source: {ex.InnerException.Source}");
                    System.Diagnostics.Debug.WriteLine($"  Method: {ex.InnerException.TargetSite?.Name}");
                    System.Diagnostics.Debug.WriteLine($"  Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
                }

                return new BaseResponse<object>(null, OperationStatus.Error, $"An internal error occurred while trying to delete the post: {ex.Message}");
            }
        }
    }
}