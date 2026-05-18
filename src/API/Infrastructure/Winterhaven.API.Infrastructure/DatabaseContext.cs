namespace Winterhaven.API.Infrastructure;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Core.Domain.Entities.Users;

[ExcludeFromCodeCoverage]
internal sealed class DatabaseContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    public DatabaseContext(
        DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        builder.Entity<Actor>().HasData(Actor.GetSystemActor());

        base.OnModelCreating(builder);
    }
}