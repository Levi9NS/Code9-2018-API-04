using System;
using System.Collections.Generic;


namespace Code9Insta.API.Infrastructure.Entities
{
    public class Post
    {
        public Guid Id { get; set; }
        public Guid ImageId { get; set; }
        public Guid ProfileId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Description { get; set; }

        public ICollection<Comment> Comments { get; set; }
        public ICollection<PostTag> PostTags { get; set; }
        public ICollection<UserLike> UserLikes { get; set; }

        public Profile Profile { get; set; }
        public Image Image { get; set; }
    }

}
