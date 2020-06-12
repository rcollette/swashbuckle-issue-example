namespace Swashbuckle.Issue.Example.Web.Controllers.Info
{
    public class ApplicationInfo
    {
        public ApplicationInfo(string environmentName, string applicationName, string? version)
        {
            EnvironmentName = environmentName;
            ApplicationName = applicationName;
            Version = version;
        }

        public string EnvironmentName { get; }

        public string ApplicationName { get; }

        public string? Version { get; }
    }
}
