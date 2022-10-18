using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataProcessingContext
{
    public class AppUser : IdentityUser
    {
        
        [StringLength(32, MinimumLength = 3)]
        public string? FirstName { get; set; }

        [StringLength(32, MinimumLength = 3)]
        public string? LastName { get; set; }


        [Required]
        [Precision(0)]
        public DateTime BirthDate { get; set; }

        [Precision(3)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime DateCreated { get; set; }

    }
}
