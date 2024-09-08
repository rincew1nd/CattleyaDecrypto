using System.Reflection;
using CattleyaDecrypto.Server.Architecture;
using CattleyaDecrypto.Server.Services;
using CattleyaDecrypto.Server.Services.Interfaces;
using CattleyaDecrypto.Server.Services.SignalR;
using Microsoft.AspNetCore.Authentication.Cookies;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Controllers and SignalR
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonDictionaryTKeyEnumTValueConverter());
});
builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.Converters.Add(new JsonDictionaryTKeyEnumTValueConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.InstanceName = "master";
    options.ConfigurationOptions = new ConfigurationOptions
    {
        EndPoints = { builder.Configuration.GetSection("Redis").Value! }
    };
});
builder.Services.AddSingleton<ICacheService, CacheService>();

// Cookie authentication
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "auth";
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Forbidden";
        
    });

// User Services
builder.Services.AddSingleton<IWordPuzzleService, WordPuzzleService>();
builder.Services.AddSingleton<INameGeneratorService, NameGeneratorService>();
builder.Services.AddSingleton<IOrderGeneratorService, OrderGeneratorService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Cookie authentication
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
});
app.UseAuthentication();
app.UseAuthorization();

// Controllers and SignalR
app.MapControllers();
app.MapHub<DecryptoMessageHub>("/api/decryptoMessageHub");
app.MapFallbackToFile("/index.html");

app.Run();
