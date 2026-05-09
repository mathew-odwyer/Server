namespace Winterhaven.Gateway.Presentation;

using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal static class Program
{
    internal static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Startup.ConfigureServices(builder.Services);

        var application = builder.Build();
        Startup.Configure(application, builder.Environment);

        application.Run();
    }
}