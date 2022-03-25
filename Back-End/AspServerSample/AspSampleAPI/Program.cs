using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AspSampleAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Build the web application.
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            ConfigureServices(builder);


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            ConfigurePipeline(app);


            // Run the applicaion.
            await app.RunAsync();
        }


        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers(options =>
                options.SuppressAsyncSuffixInActionNames = false);

            builder.Services.AddDbContext<AspServerData.ApplicationDbContext>(options => 
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddScoped<IPasswordHasher<AspServerData.User>, PasswordHasher<AspServerData.User>>();

            builder.Services.AddScoped<AspServerData.IUserStore, AspServerData.UserStore>();
            builder.Services.AddScoped<AspServerData.IPostStore, AspServerData.PostStore>();
            builder.Services.AddScoped<AspServerData.ICommentStore, AspServerData.CommentStore>();
        }

        private static void ConfigurePipeline(WebApplication app)
        {
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
        }

    }
}