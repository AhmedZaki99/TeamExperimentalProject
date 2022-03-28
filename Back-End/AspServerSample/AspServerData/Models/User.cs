using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace AspServerData
{
    [Index(nameof(NormalizedUserName), IsUnique = true)]
    [Index(nameof(NormalizedEmail), IsUnique = true)]
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }
        [Required]
        public string NormalizedUserName { get; set; }

        public string PasswordHash { get; set; }


        [Required]
        [MaxLength(256)]
        public string Email { get; set; }
        [Required]
        public string NormalizedEmail { get; set; }

        public bool EmailConfirmed { get; set; }


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


        public List<Role> Roles { get; set; } = new();
        public List<Post> Posts { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();

    }
}
