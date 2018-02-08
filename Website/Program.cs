using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Website
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseKestrel(options =>
            {
                options.Limits.MaxConcurrentConnections = 1000;
                //options.Limits.MaxConcurrentUpgradedConnections = 100;
                //options.Limits.MaxRequestBodySize = 10 * 1024;
                //options.Limits.MinRequestBodyDataRate =
                //    new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
                //options.Limits.MinResponseDataRate =
                //    new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));

                options.Listen(IPAddress.Loopback, 5001);
                //options.Listen(IPAddress.Loopback, 5001, listenOptions =>
                //{
                //    listenOptions.UseHttps("testCert.pfx", "testPassword");
                //});
            })
                .UseStartup<Startup>()
            //.ConfigureAppConfiguration((hostContext, config) =>
            //{
            //    // delete all default configuration providers
            //    config.Sources.Clear();
            //    config.AddJsonFile("myconfig.json", optional: true);
            //})
                .Build();
    }
}
