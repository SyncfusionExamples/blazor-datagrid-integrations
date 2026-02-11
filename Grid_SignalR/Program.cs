using Grid_SignalR.Components;
using Grid_SignalR.Services;
using Grid_SignalR.Hubs;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddSyncfusionBlazor();

// Register custom services
builder.Services.AddSingleton<StockDataService>();
builder.Services.AddScoped<StockAdaptor>();
builder.Services.AddSignalR(options =>
{
    options.MaximumReceiveMessageSize = 32 * 1024;      // 32 KB – adjust if sending large stock lists
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
});
builder.Services.AddHostedService<StockUpdateBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map SignalR Hub
app.MapHub<StockHub>("/stockhub");

app.Run();