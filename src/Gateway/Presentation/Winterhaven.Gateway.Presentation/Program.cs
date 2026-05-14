namespace Winterhaven.Gateway.Presentation;

using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

/*
    TODO: Rename all DTOs in gateway to Parameters (UserLoginParameters)
        - This will help me to separate between API DTOs and gateway JSON-RPC params
    TODO: Remove MediatR from gateway and opt for services and just test the services
        - Fluent validation can use ValidateAndThrowAsync
    TODO: Create a Winterhaven.Common.DTOs project
    TODO: Create a Winterhaven.Common.Extensions project
*/

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