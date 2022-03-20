using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace AspServerData
{
    public class Comment
    {

        public int CommentId { get; set; }

        [Required]
        public string Content { get; set; }
        [Required]
        [Precision(0)]
        public DateTime DatePosted { get; set; }
        [Precision(0)]
        public DateTime? LastEdited { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }

    }
}
