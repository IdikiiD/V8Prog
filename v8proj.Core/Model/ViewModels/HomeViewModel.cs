using System.Collections.Generic;
using v8proj.Core.Entities;

namespace v8proj.Web.Model.ViewModels
{
    public class HomeViewModel
    {
        public List<eUseControl> Cars { get; set; }
        public List<Post> Posts { get; set; }
    }

}