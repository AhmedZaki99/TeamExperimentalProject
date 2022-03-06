using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace ASPNetCoreData
{
    [Index(nameof(RoleName), IsUnique = true)]
    public class Role
    {

        public int RoleId { get; set; }
        [MaxLength(256)]
        [Required]
        public string RoleName { get; set; }

        public List<User> Users { get; set; } = new();

    }
}
