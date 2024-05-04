using Asp.Versioning;
using Asp.Versioning.Builder;
using MinimalAPIs.Endpoints;
using MinimalAPIs.OpenAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V"; //v1.0.0
        options.SubstituteApiVersionInUrl = true;
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//Here you can define apiVersionSet for each resource or entire application, 
//Below is the apiVersionSet for just weatherForecast entity
ApiVersionSet weatherForecaseApiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .HasApiVersion(new ApiVersion(2))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder weatherForecastGroup = app
    .MapGroup("api/v{version:apiVersion}/weatherforecast")
    .WithApiVersionSet(weatherForecaseApiVersionSet);

weatherForecastGroup.MapWeatherForecaseEndpoints();
//Swagger must come after all the minimal APIs in order to pick up all the versioning
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions();
        foreach (var desc in descriptions)
        {
            string url = $"{desc.GroupName}/swagger.json";
            string name = desc.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });
}

app.UseHttpsRedirection();
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
