using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace TTE.Infrastructure.Data
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Get connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (environment == "Production")
            {
                string secretName = "prod/db/connstring";
                string region = "us-east-1";

                IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

                var request = new GetSecretValueRequest
                {
                    SecretId = secretName,
                    VersionStage = "AWSCURRENT"
                };

                try
                {
                    // Synchronous wait for the secret
                    var response = client.GetSecretValueAsync(request).GetAwaiter().GetResult();

                    // If your secret is stored as a plain connection string:
                    connectionString = response.SecretString;
                }
                catch (Exception ex)
                {
                    // Decide how to handle or rethrow
                    throw new Exception($"Error retrieving secret '{secretName}'", ex);
                }
            }


            // Configure DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
