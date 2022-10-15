using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace AspIdentityData
{
    public class AppUser : IdentityUser
    {

        [MaxLength(256)]
        public string FirstName { get; set; }
        [MaxLength(256)]
        public string LastName { get; set; }


        [Required]
        [Precision(0)]
        public DateTime BirthDate { get; set; }

        [Precision(0)]
        public DateTime? LastSignedIn { get; set; }

        [Required]
        [Precision(3)]
        public DateTime DateCreated { get; set; }


        public List<Post> Posts { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();

    }
}
