// <copyright file="DatabaseContext.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Infrastructure;

using Mantanimus.Infrastructure.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

internal sealed class DatabaseContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    public DatabaseContext(
        DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        ArgumentNullException.ThrowIfNull(optionsBuilder);

        optionsBuilder.AddInterceptors(new AuditableEntitySaveChangesInterceptor());

        // TODO: Gateway rpc-websockets server - (authRpcServer.js - auth.register)
        //              - Ping
        //              - Heartbeat
        //              - POST UserAccount/Register w/ email, password, username.
        //              - Forward the error (if any) from the rpc-server to the client with message: error.detail;
        //              - On success, I guess just do nothing as it should be handled by the rpc-websockets library???

        optionsBuilder.UseLazyLoadingProxies();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        base.OnModelCreating(builder);
    }
}
