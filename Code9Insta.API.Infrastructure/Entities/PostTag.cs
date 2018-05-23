using System;
using System.Collections.Generic;
using System.Text;

namespace Code9Insta.API.Infrastructure.Entities
{
    public class PostTag
    {
        public Guid PostId { get; set; }
        public Post Post { get; set; }

        public Guid TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
