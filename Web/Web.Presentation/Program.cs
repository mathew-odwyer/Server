namespace Web.Presentation;

using Web.Infrastructure;

//// TODO: Unit Tests
//// TODO: API Documentation
//// TODO: Swagger/Swashbuckle Setup & Create a static site using Redocly
//// TODO: Create GitHub Actions that will automatically generate new documentation with Redocly when the API changes (consider per-branch - main & dev)

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
