using System.Collections.Generic;
using v8proj.Core.Entities;
using v8proj.Core.Entities.User; 

namespace v8proj.BissnessLogic.Interfaces.Posts 
{
    public interface IPostService
    {
        IEnumerable<Post> SearchPosts(string query);

        List<Post> GetPostsFilteredAndSorted(string category); 
        Post GetPostById(int id); 
        void AddPost(Post post); 
        Post GetPostWithFavorites(int postId); 
        UserEf GetUserWithFavorites(string userEmail); 
        void SaveChanges();
    }
}