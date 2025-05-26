using System.Collections.Generic;
using v8proj.Core.Entities;
using v8proj.Web.Model; 

namespace v8proj.BissnessLogic.Services.Home 
{
    public interface IHomeService
    {
        List<eUseControl> GetCars();
        List<Post> GetPosts(string category);
    }
}