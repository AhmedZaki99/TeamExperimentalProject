using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataProcessingContext
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {

        #region Entity Sets

        

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


        }

        #endregion


        #region Helper Methods



        #endregion

    }
}
