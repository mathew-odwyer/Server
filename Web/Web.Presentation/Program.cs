namespace Web.Presentation;

using Web.Infrastructure;

////        - Player/Get(name)
////        - Player/Update(name, x, y)
////        - Player/Delete(name)
////        - Push to master and create issues

//// ISSUES:
////    - User Email Confirmation
////    - Reset Password Flow
////    - Forgot Password Flow
////    - Change Password Flow
////    - Do I need to hash refresh token before sending it to the user?
////    - Remove dependency of Identity from Domain and Application
////    - Set the created by and modified by to the user account that actually made the changes and use a system admin GUID if it wasn't a user.
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
