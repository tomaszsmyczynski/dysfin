using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DysFin.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DysFin
{
    /// <summary>
    /// G��wna klasa aplikacji.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// G��wna metoda aplikacji.
        /// </summary>
        /// <param name="args">Argumenty uruchomieniowe.</param>
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            Serilog.Debugging.SelfLog.Enable(Console.Error);

            try
            {
                CreateDbIfNotExists(host);

                Log.Information("Uruchamianie...");

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Uruchamianie aplikacji nie powiod�o si�.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// Tworzy baz� danych aplikacji je�li nie zosta�a odnaleziona.
        /// </summary>
        /// <param name="host"><see cref="IHost"/></param>
        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var context = services.GetRequiredService<DysFinContext>();
                    context.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Wyst�pi� b��d tworzenia bazy danych.");
                }
            }
        }

        /// <summary>
        /// Tworzy <see cref="IHostBuilder"/>.
        /// </summary>
        /// <param name="args">Argumenty uruchomieniowe.</param>
        /// <returns><see cref="IHostBuilder"/></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseIISIntegration()
                    .UseStartup<Startup>();
                });
    }
}
