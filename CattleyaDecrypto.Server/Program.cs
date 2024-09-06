using CattleyaDecrypto.Server.Architecture;
using CattleyaDecrypto.Server.Services;
using CattleyaDecrypto.Server.Services.Interfaces;
using CattleyaDecrypto.Server.Services.SignalR;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonDictionaryTKeyEnumTValueConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.Converters.Add(new JsonDictionaryTKeyEnumTValueConverter());
});
builder.Services.AddMemoryCache();

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
    });

// User Services
builder.Services.AddSingleton<IWordPuzzleService, WordPuzzleService>();
builder.Services.AddSingleton<INameGeneratorService, NameGeneratorService>();
builder.Services.AddSingleton<IDecryptoMatchService, DecryptoMatchService>();
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

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
});
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<DecryptoMessageHub>("/api/decryptoMessageHub");

app.MapFallbackToFile("/index.html");

app.Run();
