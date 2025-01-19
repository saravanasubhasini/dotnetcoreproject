using Entities;
using FirstWebApp;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RepositoryContracts;
using Repositories;
using ServiceContracts;
using Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider service,
    LoggerConfiguration loggerConfiguration ) => {
        loggerConfiguration.ReadFrom .Configuration(context.Configuration).
        ReadFrom.Services(service);
});
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<FirstAppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString"));
});



//builder.Services.Configure<WeatherApiOption>(builder.Configuration.GetSection("Weatherapi"));

builder.Services.AddHttpClient();
builder.Services.AddScoped<StockService>();

builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IPersonsRepository, PersonsRepository>();
var app = builder.Build();

//app.UseHttpLogging();
app.UseStaticFiles();

app.UseRouting();


app.MapControllers();
app.Run();