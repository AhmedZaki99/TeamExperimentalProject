using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace AspIdentityData
{
    public class Comment
    {

        public string Id { get; set; }

        [Required]
        public string Content { get; set; }
        [Required]
        [Precision(0)]
        public DateTime DatePosted { get; set; }
        [Precision(0)]
        public DateTime? LastEdited { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public string PostId { get; set; }
        public Post Post { get; set; }

    }
}
