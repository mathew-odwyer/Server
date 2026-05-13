namespace Winterhaven.API.Presentation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scalar.AspNetCore;
using System;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Infrastructure.Extensions;
using Winterhaven.API.Presentation.Extensions;
using Winterhaven.API.Presentation.Options.Security;
using Winterhaven.Common.Extensions;

[ExcludeFromCodeCoverage]
internal sealed class Startup
{
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IConfiguration Configuration { get; }

    public void Configure(WebApplication application, IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(environment);

        if (environment.IsDevelopment() || this.Configuration.GetValue<bool>("SCALAR_ENABLED"))
        {
            application.MapOpenApi();
            application.MapScalarApiReference(x =>
            {
                x.Title = "Winterhaven API";
                x.Layout = ScalarLayout.Classic;

                x.ExpandAllResponses = false;
                x.ExpandAllModelSections = false;
            });
        }

        if (!environment.IsDevelopment())
        {
            application.UseHsts();
        }

        application.UseRouting();
        application.UseAuthentication();
        application.UseUserSessions();
        application.UseAuthorization();

        application.UseStaticFiles(new StaticFileOptions()
        {
            HttpsCompression = HttpsCompressionMode.DoNotCompress,
            ServeUnknownFileTypes = true,
        });

        application.UseEndpoints(x => x.MapControllers());
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddValidatedOptions<ApiOptions>(this.Configuration);

        services.AddWinterhavenMappings();
        services.AddWinterhavenInfrastructureServices(this.Configuration);
        services.AddWinterhavenControllersWithFilters();
        services.AddWinterhavenAuthentication(this.Configuration);
        services.AddWinterhavenAuthorization();
        services.AddWinterhavenApiServices();
    }
}