namespace Sage.Active.Cli
{
    /// <summary>
    /// The programming model FrameworkDetector found - decides which package(s) and usage snippet to offer.
    /// </summary>
    public enum ProjectFrameworkKind
    {
        /// <summary>An ASP.NET Core web app (Minimal API or MVC) - has access to IServiceCollection DI.</summary>
        AspNetCoreWeb,

        /// <summary>A .NET Generic Host worker/background service - also has DI via Host.CreateApplicationBuilder.</summary>
        WorkerService,

        /// <summary>A console application (or file-based app with no DI host) with no built-in DI container.</summary>
        Console,

        /// <summary>A class library - the caller decides how it's hosted.</summary>
        ClassLibrary,

        /// <summary>Couldn't confidently classify the project.</summary>
        Unknown,
    }

    /// <summary>
    /// The result of scanning a directory for a .NET project or file-based app.
    /// </summary>
    public class DetectedProject
    {
        /// <summary>Path to the .csproj, or - when IsFileBasedApp is true - the single entry .cs file.</summary>
        public string ProjectPath { get; init; } = string.Empty;

        public ProjectFrameworkKind Kind { get; init; }
        public string TargetFramework { get; init; } = "net8.0";

        /// <summary>
        /// True when there's no .csproj at all and this is a .NET 10+ file-based app (dotnet run app.cs).
        /// </summary>
        public bool IsFileBasedApp { get; init; }

        /// <summary>
        /// True for AspNetCoreWeb and WorkerService - projects with an IServiceCollection to register into.
        /// </summary>
        public bool HasDependencyInjection => Kind is ProjectFrameworkKind.AspNetCoreWeb or ProjectFrameworkKind.WorkerService;
    }
}
