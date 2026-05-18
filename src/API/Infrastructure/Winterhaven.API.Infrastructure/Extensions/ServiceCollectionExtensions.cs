namespace Winterhaven.API.Infrastructure.Extensions;

using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using Winterhaven.API.Core.Application.Behaviours;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Application.Requests.Users.LoginUser;
using Winterhaven.API.Core.Application.Requests.Users.RegisterUser;
using Winterhaven.API.Core.Application.Services.Maps;
using Winterhaven.API.Core.Application.Services.Security;
using Winterhaven.API.Core.Application.Services.Users;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Players;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Infrastructure.Contexts;
using Winterhaven.API.Infrastructure.Interceptors;
using Winterhaven.API.Infrastructure.Options.Maps;
using Winterhaven.API.Infrastructure.Options.Security;
using Winterhaven.API.Infrastructure.Services.Maps;
using Winterhaven.API.Infrastructure.Services.Security;
using Winterhaven.API.Infrastructure.Services.Users;
using Winterhaven.API.Infrastructure.Work;
using Winterhaven.API.Infrastructure.Work.Players;
using Winterhaven.API.Infrastructure.Work.Users;
using Winterhaven.Common.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        string connectionName = "Docker";

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            connectionName = "Local";
        }

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<DbContext, DatabaseContext>((provider, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString(connectionName), o =>
            {
                o.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });

            options.UseLazyLoadingProxies();
            options.AddInterceptors(provider.GetRequiredService<AuditableEntitySaveChangesInterceptor>());
        }, ServiceLifetime.Scoped);

        services
            .AddIdentityCore<IdentityUser<Guid>>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddSignInManager<SignInManager<IdentityUser<Guid>>>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(x =>
        {
            x.User.RequireUniqueEmail = true;
            x.SignIn.RequireConfirmedEmail = false;
        });

        services.AddSingleton<IFileSystem, FileSystem>();

        services.AddValidatorsFromAssemblyContaining<LoginUserRequestValidator>();
        services.AddMediatR(x =>
        {
            x.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehahviour<,>));
            x.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            x.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

            x.RegisterServicesFromAssemblyContaining<RegisterUserRequestHandler>();
        });

        services.AddValidatedOptions<JwtOptions>(configuration);
        services.AddValidatedOptions<MapStorageOptions>(configuration);

        services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

        services.AddScoped<IUserAccountRepository, UserAccountRepository>();
        services.AddScoped<IUserSessionTokenRepository, UserSessionTokenRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IActorRepository, ActorRepository>();

        services.AddScoped<IUserRegistrar, UserRegistrar>();
        services.AddScoped<IUserAuthenticator, UserAuthenticator>();
        services.AddScoped<IActorContext, ActorContext>();

        services.AddScoped<ISecureTokenFactory, SecureTokenFactory>();
        services.AddScoped<ISecureTokenHasher, SecureTokenHasher>();

        services.AddSingleton<IMapLocator, MapLocator>();

        return services;
    }
}