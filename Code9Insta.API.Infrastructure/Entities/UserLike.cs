using System;


namespace Code9Insta.API.Infrastructure.Entities
{
    public class UserLike
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }

        public Post Post { get; set; }
    }
}
