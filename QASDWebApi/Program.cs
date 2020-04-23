namespace QASDWebApi
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using QASDWebApi.Domain;

    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args)
                .Build();
            var deskMonitor = (DeskMonitor) host.Services.GetService(typeof(DeskMonitor));
            deskMonitor.StartMonitorLoop();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(
                    logging =>
                    {
                        logging.AddConsole();
                    })
                .ConfigureWebHostDefaults(
                    webBuilder =>
                    {
                        webBuilder.UseUrls("http://+:9999/")
                            .UseStartup<Startup>();
                    });
    }
}