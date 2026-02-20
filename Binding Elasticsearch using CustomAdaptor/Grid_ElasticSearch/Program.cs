using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Grid_ElasticSearch.Components;
using Grid_ElasticSearch.Data;
using Syncfusion.Blazor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ========== SYNCFUSION BLAZOR CONFIGURATION ==========
builder.Services.AddSyncfusionBlazor();
// =====================================================

// ========== ELASTICSEARCH CONFIGURATION ==========
// Get ElasticSearch configuration from appsettings.json
var elasticSearchUrl = builder.Configuration["ElasticSearch:Url"];
var elasticSearchPwd = builder.Configuration["ElasticSearch:Password"] ?? "";

if (string.IsNullOrEmpty(elasticSearchUrl))
{
    throw new InvalidOperationException("ElasticSearch URL not found in configuration.");
}

// Create and register ElasticSearch client using Elastic.Clients.Elasticsearch
var settings = new ElasticsearchClientSettings(new Uri(elasticSearchUrl)).Authentication(new BasicAuthentication("elastic", elasticSearchPwd));

var client = new ElasticsearchClient(settings);

builder.Services.AddSingleton<ElasticsearchClient>(client);

// Register Inventory Data Service (static in-memory data)
builder.Services.AddSingleton<InventoryDataService>();

// Register Repository for dependency injection
builder.Services.AddScoped<InventoryRepository>();

// Register ElasticSearch Initialization Service
builder.Services.AddScoped<ElasticSearchInitializationService>();
// ===================================================

var app = builder.Build();

// ========== INITIALIZE ELASTICSEARCH INDEXES ==========
// Create indexes on application startup if they don't exist
using (var scope = app.Services.CreateScope())
{
    var initializationService = scope.ServiceProvider.GetRequiredService<ElasticSearchInitializationService>();
    await initializationService.InitializeAsync();
}
// ===================================================

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
