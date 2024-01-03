using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NissayaEditor_Web.Data;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;
//using IPinfo;
//using IPinfo.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSyncfusionBlazor();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
//builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddScoped<SfDialogService>();
builder.Services.AddSingleton<State>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
//Register Syncfusion license
//Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("YOUR LICENSE KEY");
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo + DSMBMAY9C3t2VlhhQlJCfV5AQmBIYVp / TGpJfl96cVxMZVVBJAtUQF1hSn9TdEFjW3xWc3dcQ2Ve; Mjc3NjQ1OEAzMjMzMmUzMDJlMzBjMGJpQWIwRHUxdWVkcDliRGhkNjBtZTU5OEJqWndTc3V6ZWZqTytEYnlFPQ ==; Mjc3NjQ1OUAzMjMzMmUzMDJlMzBvUnU4NDZodVBmSHZVbTk4eW13YVhDZnNvd3ozOVIzSzRUb1JrNmNVSFhFPQ ==");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
