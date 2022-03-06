using Microsoft.EntityFrameworkCore;

namespace ASPNetCoreData
{
    public class ApplicationDbContext : DbContext
    {

        #region Entity Sets
#nullable disable

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

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
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .OnDelete(DeleteBehavior.ClientCascade);
        }

        #endregion

    }
}
