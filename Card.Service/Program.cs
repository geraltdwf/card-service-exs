using Asp.Versioning;
using Card.Service.Configurations;
using Card.Service.Interfaces;
using Card.Service.Middleware;
using Card.Service.Repositories;
using Card.Service.Services;
using Card.Service.Validators;
using Microsoft.OpenApi.Models;
using RulesEngine.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Configuration.AddJsonFile(opt => {
    var engineRules = builder.Configuration["RulesEngine:ActionRulesPath"];
    if(string.IsNullOrEmpty(engineRules))
        throw new InvalidOperationException("Configuration key 'RulesEngine:ActionRulesPath' is not set. Please add it to your appsettings.json or environment variables.");

    opt.Path = engineRules;
    opt.Optional = false;
    opt.ReloadOnChange = true;
});

builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddProblemDetails().AddErrorObjects();
builder.Services.AddApiVersioning(apiOptions =>
{
    apiOptions.DefaultApiVersion = new ApiVersion(1, 0);
    apiOptions.AssumeDefaultVersionWhenUnspecified = false;
    apiOptions.ReportApiVersions = true; 
    apiOptions.ApiVersionReader = new HeaderApiVersionReader("X-API-Version");
}).AddMvc();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>{
    var apiVersion = builder.Configuration["ApiVersion"];
    if(string.IsNullOrEmpty(apiVersion))
        throw new InvalidOperationException("Configuration key 'ApiVersion' is not set. Please add it to your appsettings.json or environment variables.");
    opt.OperationFilter<AddApiVersionHeaderParameter>(apiVersion);
});

builder.Services.AddSingleton(provider => {
    var ruleConfig = builder.Configuration.GetSection("Workflows").Get<List<Workflow>>();
    return new RulesEngine.RulesEngine(ruleConfig?.ToArray(), null);
});
builder.Services.AddSingleton<ICardCacheService, CardCacheService>();
builder.Services.AddSingleton<ICardValidator, CardValidator>();
builder.Services.AddSingleton<IMatchingEngineService, MatchingEngineService>();
builder.Services.AddSingleton<ICardRepository, CardRepository>();
builder.Services.AddScoped<ICardService, CardService>();


var app = builder.Build();

app.UseExceptionMiddleware();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();

}
app.MapControllers();
app.Run();





