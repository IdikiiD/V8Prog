using System;
using System.Collections.Generic;
using v8proj.Core.Entities.User;

namespace v8proj.Core.Entities
{

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string ImagePath1 { get; set; }
        public string ImagePath2 { get; set; }
        public string ImagePath3 { get; set; }

        public DateTime CreatedAt { get; set; } // <-- это поле должно быть


        public virtual ICollection<UserEf> FavoritedBy { get; set; } = new List<UserEf>();

    }
}