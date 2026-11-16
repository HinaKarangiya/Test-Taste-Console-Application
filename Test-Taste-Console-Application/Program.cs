using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.Services;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            //The ConfigureServices function configures the services.
            ConfigureServices(serviceCollection);

            //The RunServiceOperationsAsync function executes the code that can create the outputs.
            await RunServiceOperationsAsync(serviceCollection);
        }

        private static async Task RunServiceOperationsAsync(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();

            IOutputService screenOutputService;
            try
            {
                //The service provider gets the services. Creating them can fail, e.g. when the API key is missing.
                screenOutputService = serviceProvider.GetRequiredService<IOutputService>();
            }
            catch (Exception exception)
            {
                ReportFailure(exception);
                serviceProvider.Dispose();
                return;
            }

            //Every output is a separate step. When one step fails, the user sees why and the next step still runs.
            await RunStepAsync(screenOutputService.OutputAllPlanetsWithMoonsAndTheirAverageMoonTemperatureToConsoleAsync);
            await RunStepAsync(screenOutputService.OutputAllMoonsAndTheirMassToConsoleAsync);
            await RunStepAsync(screenOutputService.OutputAllPlanetsAndTheirMoonsToConsoleAsync);

            serviceProvider.Dispose();
        }

        private static async Task RunStepAsync(Func<Task> step)
        {
            try
            {
                await step();
            }
            catch (Exception exception)
            {
                ReportFailure(exception);
                ConsoleWriter.CreateEmptyLines(1);
            }
        }

        private static void ReportFailure(Exception exception)
        {
            //The users and developers can see the thrown exceptions.
            Logger.Instance.Error($"{LoggerMessage.ScreenOutputOperationFailed}{exception.Message}");
            Console.WriteLine($"{ExceptionMessage.ScreenOutputOperationFailed}{exception.Message}");
            System.Diagnostics.Debug.WriteLine($"{ExceptionMessage.ScreenOutputOperationFailed}{exception.Message}");
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            //The function configures all the services.
            XmlConfigurator.Configure(LogManager.GetRepository(Assembly.GetEntryAssembly()),
                new FileInfo(ConfigurationFileName.Logger));

            //The settings (including the API key) come from appsettings.json & An environment variable (e.g. Api__Key) overrides the file.
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(ConfigurationFileName.AppSettings, optional: true)
                .AddEnvironmentVariables()
                .Build();
            serviceCollection.AddSingleton(configuration);

            serviceCollection.AddHttpClient<BaseHttpClient>();

            serviceCollection.AddSingleton<ISolarSystemService, SolarSystemService>();
            serviceCollection.AddSingleton<IPlanetService, PlanetService>();
            serviceCollection.AddSingleton<IOutputService, ScreenOutputService>();
            serviceCollection.AddSingleton<IMoonService, MoonService>();
        }
    }
}
