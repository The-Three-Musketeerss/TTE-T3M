using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TTE.Seeding.Infrastructure;
using TTE.Seeding.Services;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddDbContext<AppDbContext>();
        services.AddHttpClient();
        services.AddTransient<FakeStoreSeeder>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .Build();

using var scope = host.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<FakeStoreSeeder>();

await seeder.SeedDataAsync();