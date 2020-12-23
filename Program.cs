using CreditOne.Core.Logger.Factories;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CreditOne.P360FormSubmissionService
{
    public class Program
    {
        public static void Main(string[] args)
        {
           CreateWebHostBuilder(args).Build().Run();
           
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging((context, builder) =>
                {
                    builder.AddFile(opts =>
                    {
                        context.Configuration.GetSection("FileLoggingOptions").Bind(opts);
                    });
                })
                .UseStartup<Startup>();
    }
}
