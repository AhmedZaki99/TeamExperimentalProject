using Microsoft.EntityFrameworkCore;

namespace AspServerData
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


            modelBuilder.Entity<User>()
                .Property(u => u.NormalizedUserName)
                .HasComputedColumnSql("UPPER([UserName])", stored: true);

            modelBuilder.Entity<User>()
                .Property(u => u.NormalizedEmail)
                .HasComputedColumnSql("UPPER([Email])", stored: true);
        }

        #endregion


    }
}
