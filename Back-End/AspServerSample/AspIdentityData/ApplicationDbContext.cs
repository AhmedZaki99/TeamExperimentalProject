using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspIdentityData
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {

        #region Entity Sets
#nullable disable

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

#nullable enable
        #endregion


        #region Constructor

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        #endregion


        #region Configuration

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .OnDelete(DeleteBehavior.ClientCascade);
        }

        #endregion


    }
}
