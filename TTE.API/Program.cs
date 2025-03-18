using TTE.API.Configurations;
using TTE.Commons;
using TTE.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

var seederConfig = new SeederConfiguration();
builder.Configuration.GetSection("SeederConfiguration").Bind(seederConfig);

// Register Application and Infrastructure Services
builder.Services.AddAuthServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddSwaggerServices(builder.Configuration);

builder.Services.AddControllers();

builder.Services.AddHttpClient();
builder.Services.AddTransient<FakeStoreSeeder>(sp =>
{
    var context = sp.GetRequiredService<AppDbContext>();
    var httpClient = sp.GetRequiredService<HttpClient>();
    var logger = sp.GetRequiredService<ILogger<FakeStoreSeeder>>();
    return new FakeStoreSeeder(context, httpClient, logger, seederConfig.EnableFakeStoreSeeding);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var seeder = services.GetRequiredService<FakeStoreSeeder>();

    try
    {
        await seeder.SeedDataAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
