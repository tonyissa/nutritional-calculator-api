using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using nutritional_calculator_api.Data;
using nutritional_calculator_api.Helpers;
using nutritional_calculator_api.Options;
using nutritional_calculator_api.Services;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SqliteConnection")
    ?? throw new InvalidOperationException("Sqlite database connection string not found");

builder.Services.AddControllers().AddJsonOptions(x =>
   x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<PopularityTracker>();
builder.Services.AddHostedService<PopularityUpdateService>();

builder.Services.AddSwaggerGen(s =>
{
    s.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "V1",
        Title = "Nutrition Calculator API",
        Description = "API for retrieving nutritional information for food"
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    s.IncludeXmlComments(xmlPath);
});

builder.Services.AddDbContext<NutritionContext>(opt =>
{
    opt.UseSqlite(connectionString);
    opt.AddInterceptors(new SqliteFKInterceptor());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NutritionContext>();
    var dataOptions = app.Configuration.GetSection(DataOptions.DataPaths).Get<DataOptions>()
        ?? throw new InvalidOperationException("Data paths not found");
    context.Database.EnsureCreated();
    await SeedData.ExecuteAsync(context, dataOptions);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();