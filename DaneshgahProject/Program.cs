using DaneshgahProject.Infrastructure;
using DaneshgahProject.Infrastructure.BackgroundServices;
using DaneshgahProject.Infrastructure.RealTimeService;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSingleton<ITemperatureService, TemperatureService>();
builder.Services.AddSingleton<ISetAndGetTemperature, SetAndGetTemperature>();
builder.Services.AddSingleton<TemperatureBackgroundService>();
builder.Services.AddSingleton<ITemperatureBackgroundService>(provider => provider.GetRequiredService<TemperatureBackgroundService>());
builder.Services.AddHostedService(provider =>
    (TemperatureBackgroundService)provider.GetRequiredService<ITemperatureBackgroundService>());

builder.Services.AddSignalR();

builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        ["application/octet-stream"]);
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.UseResponseCompression();

app.MapRazorPages()
   .WithStaticAssets();


app.MapHub<SetAndGetTemperatureSignalR>("/temperaturehub");
app.MapHub<TemperatureBackgroundService>("/temperaturebackground");
//app.UseCors(policy => policy
//    .AllowAnyHeader()
//    .AllowAnyMethod()
//    .SetIsOriginAllowed(origin => true)
//    .AllowCredentials());

app.Run();
