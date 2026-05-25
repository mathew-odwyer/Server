namespace Winterhaven.API.Presentation;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Winterhaven.API.Infrastructure;

internal static class Program
{
    internal static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var startup = new Startup(builder.Configuration);

        startup.ConfigureServices(builder.Services);

        var application = builder.Build();
        startup.Configure(application, builder.Environment);

        using (var scope = application.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        application.Run();
    }
}