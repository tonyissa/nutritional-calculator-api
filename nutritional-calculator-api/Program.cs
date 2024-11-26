using Microsoft.EntityFrameworkCore;
using nutritional_calculator_api.Data;
using nutritional_calculator_api.Helpers;
using nutritional_calculator_api.Options;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SqliteConnection")
    ?? throw new InvalidOperationException("Sqlite database connection string not found");

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
