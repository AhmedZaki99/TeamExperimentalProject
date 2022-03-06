using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace ASPNetCoreData
{
    public class Post
    {

        public int PostId { get; set; }

        [MaxLength(256)]
        public string Caption { get; set; }
        [Required]
        public string Content { get; set; }

        [Required]
        [Precision(0)]
        public DateTime DatePosted { get; set; }


        public int UserId { get; set; }
        public User User { get; set; }

        public List<Comment> Comments { get; set; } = new();

    }
}
