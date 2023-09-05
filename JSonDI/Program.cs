using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Authentication.ExtendedProtection;

namespace JSonDI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. Create a service collection for DI
            var serviceCollection = new ServiceCollection(); // DI MS package

            // 2. Build a configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json")
                .Build();

            // 3. Add the configuration to the service collection
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            serviceCollection.AddSingleton<Test>();

            // Test class
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var testInstance = serviceProvider.GetService<Test>();
            testInstance.TestMethod();
        }
    }
}