using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var tableSettings = builder.Configuration.GetSection(nameof(TableSettings)).Get<TableSettings>();
builder.Services.AddSingleton(tableSettings);

var instanceSettings = new InstanceSettings
{
    InstanceId = builder.Configuration.GetValue<string>(nameof(InstanceSettings.InstanceId))
};
builder.Services.AddSingleton(instanceSettings);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

public class TableSettings
{
    public string ConnectionString { get; set; }
}

public class InstanceSettings
{
    public string InstanceId { get; set; }
}
