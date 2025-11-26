// <copyright file="Program.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Presentation;

using Winterhaven.Infrastructure;

internal static class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var startup = new Startup(builder.Configuration);

        startup.ConfigureServices(builder.Services);

        var application = builder.Build();
        Startup.Configure(application, builder.Environment);

        using (var scope = application.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        application.Run();
    }
}
