using Grid_SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Grid_SignalR.Services;

public class StockUpdateBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<StockUpdateBackgroundService> _logger;
    private const int UpdateIntervalMs = 1000;

    public StockUpdateBackgroundService(IServiceProvider serviceProvider, ILogger<StockUpdateBackgroundService> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Stock Update Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(UpdateIntervalMs, stoppingToken);

                using (var scope = _serviceProvider.CreateScope())
                {
                    var stockDataService = scope.ServiceProvider.GetRequiredService<StockDataService>();
                    var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<StockHub>>();

                    stockDataService.UpdateStockPrices();
                    var stocks = stockDataService.GetAllStocks();

                    await hubContext.Clients.Group("StockTraders").SendAsync("ReceiveStockUpdate", stocks, cancellationToken: stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating stocks");
            }
        }

        _logger.LogInformation("Stock Update Background Service stopped");
    }
}
