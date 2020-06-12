using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Swashbuckle.Issue.Example.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Serilog.Debugging.SelfLog.Enable(Console.WriteLine);
            BuildWebHost(args).Run();
        }

        private static IHost BuildWebHost(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, builder) => builder.ClearProviders())
                .ConfigureHostConfiguration(config =>
                {
                    config.AddEnvironmentVariables("ASPNETCORE_");
                    config.AddJsonFile("hosting.json");
                    config.AddCommandLine(args);
                })
                .ConfigureWebHost(config =>
                {
                    config.UseKestrel(kestrelConfig =>
                    {
                        kestrelConfig.AddServerHeader = false;
                    });
                    config.UseStartup<Startup>();
                })
                .Build();
        }
    }
}
