using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Rooms;
using Winterhaven.API.Infrastructure;
using Winterhaven.API.Infrastructure.Services.Seeds.Rooms;

namespace Winterhaven.API.Presentation;

internal static class Program
{
    internal static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var startup = new Startup(builder.Configuration);

        startup.ConfigureServices(builder.Services);

        var application = builder.Build();
        startup.Configure(application, builder.Environment);

        using (var scope = application.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            await db.Database.EnsureDeletedAsync().ConfigureAwait(false);
            await db.Database.EnsureCreatedAsync().ConfigureAwait(false);

            var unitOfWorkFactory = scope.ServiceProvider.GetRequiredService<IUnitOfWorkFactory>();
            var roomRepository = scope.ServiceProvider.GetRequiredService<IRoomRepository>();

            await RoomSeedService.SeedAsync(unitOfWorkFactory, roomRepository).ConfigureAwait(false);
        }

        await application.RunAsync().ConfigureAwait(false);
    }
}
