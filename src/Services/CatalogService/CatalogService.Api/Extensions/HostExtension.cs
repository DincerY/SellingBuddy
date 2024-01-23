using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace CatalogService.Api.Extensions;

public static class HostExtension
{

    public static IWebHost MigrationDbContext<TContext>(this IWebHost host, Action<TContext, IServiceProvider> seeder)
        where TContext : DbContext
    {
        //Oluşturulan Service Scope(hizmet kapsamı) dışında bu service kullanımı sona ermiş olur. 
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;

            var logger = services.GetRequiredService<ILogger<TContext>>();

            var context = services.GetService<TContext>();

            try
            {

                logger.LogInformation("Migrating database associated with context {DbContextName}", typeof(TContext).Name);

                var retry = Policy.Handle<SqlException>()
                    .WaitAndRetry(new TimeSpan[]
                    {
                        TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(8)
                    });

                retry.Execute(() => InvokeSeeder(seeder, context, services));

                logger.LogInformation("Migrated database associated with context {DbContextName}",typeof(TContext).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"An error occured while migrating the database used on context {DbContextName}",typeof(TContext).Name);
            }
        }

        return host;
    }

    private static void InvokeSeeder<TContext>(Action<TContext, IServiceProvider> seeder, TContext context, IServiceProvider services) where TContext : DbContext
    {
        context.Database.EnsureCreated();
        context.Database.Migrate();
        seeder(context, services);
    }
}