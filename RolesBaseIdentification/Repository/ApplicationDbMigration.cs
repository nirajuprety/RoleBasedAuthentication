using Microsoft.EntityFrameworkCore;

namespace RolesBaseIdentification.Repository
{
    public class ApplicationDbMigration
    {
        public static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = new ApplicationDbContext(serviceScope.ServiceProvider.GetService<
                           DbContextOptions<ApplicationDbContext>>()))
                {
                    context.Database.Migrate();
                }


            }
        }
    }
}
