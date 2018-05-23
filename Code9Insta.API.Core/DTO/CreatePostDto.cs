using System.ComponentModel.DataAnnotations;

namespace Code9Insta.API.Core.DTO
{
    public class CreatePostDto
    {
        [Required]
        public byte[] ImageData { get; set; }
        public string[] Tags { get; set; }
        public string Description { get; set; }
    }
}
