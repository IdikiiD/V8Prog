using System.Collections.Generic;
using System.Linq;
using v8proj.BissnessLogic.Interfaces.Home; 
using v8proj.BissnessLogic.Interfaces.Posts; 
using v8proj.Core.Entities;
using v8proj.Web.Model;
namespace v8proj.BissnessLogic.Services.Home 
{
    public class HomeService : IHomeService
    {
        private readonly IEUseControlService _eUseControlService;
        private readonly IPostService _postService;

        public HomeService(IEUseControlService eUseControlService, IPostService postService)
        {
            _eUseControlService = eUseControlService;
            _postService = postService;
        }

        public List<eUseControl> GetCars()
        {
            return _eUseControlService.GetAllCars();
        }

        public List<Post> GetPosts(string category)
        {
            return _postService.GetPostsFilteredAndSorted(category);
        }
    }
}