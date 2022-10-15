using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace AspIdentityData
{
    public class Post
    {

        public string Id { get; set; }

        [MaxLength(256)]
        public string Caption { get; set; }
        [Required]
        public string Content { get; set; }

        [Required]
        [Precision(0)]
        public DateTime DatePosted { get; set; }
        [Precision(0)]
        public DateTime? LastEdited { get; set; }


        public string UserId { get; set; }
        public AppUser User { get; set; }

        public List<Comment> Comments { get; set; } = new();

    }
}
