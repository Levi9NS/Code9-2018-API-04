using System;
using System.ComponentModel.DataAnnotations;

namespace Code9Insta.API.Infrastructure.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid ProfileId { get; set; }
        public Guid PostId { get; set; }
        public DateTime CreatedOn { get; set; }
        [Required]
        public string Text { get; set; }

        public Profile Profile { get; set; }
    }
}
