using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;

namespace Winterhaven.Gateway.Presentation;

[ExcludeFromCodeCoverage]
internal static class Program
{
    internal static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var startup = new Startup(builder.Configuration);

        startup.ConfigureServices(builder.Services);

        var application = builder.Build();
        startup.Configure(application, builder.Environment);

        application.Run();
    }
}
