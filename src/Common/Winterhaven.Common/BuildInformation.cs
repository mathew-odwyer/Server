using System.Reflection;

namespace Winterhaven.Common;

/// <summary>
///   Provides build information about Winterhaven.
/// </summary>
public static class BuildInformation
{
    /// <summary>
    ///   Gets the version of Winterhaven from the assembly's informational version.
    /// </summary>
    /// <value>
    ///   The version of Winterhaven, or "Unknown" if the informational version is not found.
    /// </value>
    public static string Version
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            string version = attribute?.InformationalVersion ?? "Unknown";

            //// The informational version may include a +gitsha suffix added by the SDK during build.
            //// We want to strip this for display purposes (instead of "1.0.0+abc123", we want "1.0.0").
            return version.Split('+')[0];
        }
    }
}
