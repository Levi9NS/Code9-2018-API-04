using System;
using System.Collections.Generic;
using System.Text;

namespace Code9Insta.API.Infrastructure.Entities
{
    public class Tag
    {
        public Guid Id { get; set; }      
        public string Text { get; set; }

        public ICollection<PostTag> PostTags { get; set; }
    }
}
