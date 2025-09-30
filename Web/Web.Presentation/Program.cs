namespace Web.Presentation;

using Web.Infrastructure;

////        - Push to master and create issues

//// ISSUES:
////    - Set the created by and modified by to the user account that actually made the changes and use a system admin GUID if it wasn't a user.
////    - IUserContext - string UserAccountId { get; } - set up in Presentation or Infrastructure and use IHttpContextAccessor to fetch the user ID.
////        - This can replace all Request classes where there is a UserAccountId.
////    - Allow users to have player names that other users have - this is okay, just figure out how to manage it in game/server-side.
////        - This involves updating the DB mapping, too and MAYBE the CreatePlayerRequestHandler, not sure yet.
////        - Also involves updating the way we fetch players at the moment in regards to IPlayerRepository.
////        - Also consider DatabaseUpdateException where soft deletion is important - if a player has been soft deleted, they should be able to re-create a new player with the same name, hopefully - if that's too difficult then just tell them no.
////    - User Email Confirmation
////    - Reset Password Flow
////    - Forgot Password Flow
////    - Change Password Flow
////    - Do I need to hash refresh token before sending it to the user?
////    - Remove dependency of Identity from Domain and Application
////    - Membership - Consider "Roles" with ASP.NET Identity? or?
////    - Cap player limit per user account (consider "membership" flag) - ensure capped in DB and CreatePlayerRequestHandler, too.
////        - Documentation
////        - Unit Tests

/// <summary>
/// The Web API application.
/// </summary>
internal static class Program
{
    /// <summary>
    /// Defines the entry point of the application.
    /// </summary>
    /// <param name="args">
    /// The arguments.
    /// </param>
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var startup = new Startup(builder.Configuration);

        startup.ConfigureServices(builder.Services);

        var application = builder.Build();
        startup.Configure(application, builder.Environment);

        using (var scope = application.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            db.Database.EnsureCreated();
        }

        application.Run();
    }
}
