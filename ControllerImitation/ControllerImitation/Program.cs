using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ControllerImitation
{
    public class Program
    {
        public static void Main(string[] args)
        {
#if TRUE

            var settingsBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");


            var configuration = settingsBuilder.Build();


            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(configuration)
                .UseStartup<Startup>()
                .Build();

            host.Run();
#else
            // получаем путь к файлу 
            var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            // путь к каталогу проекта
            var pathToContentRoot = Path.GetDirectoryName(pathToExe);

            var settingsBuilder = new ConfigurationBuilder()
                .SetBasePath(pathToContentRoot)
                .AddJsonFile("appsettings.json");


            var configuration = settingsBuilder.Build();
            var urls = new List<string>()
            {
                $"http://*:{configuration["PortNumber"]}"
            };
            // создаем хост
            var host = WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(pathToContentRoot)
                .UseStartup<Startup>()
                .UseUrls(urls.ToArray())
                .Build();
            // запускаем в виде службы
            host.RunAsService();
#endif
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
