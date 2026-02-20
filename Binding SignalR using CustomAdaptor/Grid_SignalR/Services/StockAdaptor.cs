using System.Collections;
using Grid_SignalR.Models;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;

namespace Grid_SignalR.Services;

public class StockAdaptor : DataAdaptor
{
    private readonly StockDataService _stockDataService;

    public StockAdaptor(StockDataService stockDataService)
    {
        _stockDataService = stockDataService ?? throw new ArgumentNullException(nameof(stockDataService));
    }

    public override async Task<object> ReadAsync(DataManagerRequest dataManagerRequest, string? key = null)
    {
        ArgumentNullException.ThrowIfNull(dataManagerRequest);

        IEnumerable stocks = _stockDataService.GetAllStocks();

        if (dataManagerRequest.Search?.Count > 0)
        {
            stocks = DataOperations.PerformSearching(stocks, dataManagerRequest.Search);
        }

        if (dataManagerRequest.Where?.Count > 0)
        {
            stocks = DataOperations.PerformFiltering(stocks, dataManagerRequest.Where, dataManagerRequest.Where[0].Operator);
        }

        if (dataManagerRequest.Sorted?.Count > 0)
        {
            stocks = DataOperations.PerformSorting(stocks, dataManagerRequest.Sorted);
        }

        int totalRecordsCount = stocks.Cast<Stock>().Count();

        if (dataManagerRequest.Skip != 0)
        {
            stocks = DataOperations.PerformSkip(stocks, dataManagerRequest.Skip);
        }

        if (dataManagerRequest.Take != 0)
        {
            stocks = DataOperations.PerformTake(stocks, dataManagerRequest.Take);
        }

        return dataManagerRequest.RequiresCounts 
            ? new DataResult() { Result = stocks, Count = totalRecordsCount } 
            : (object)stocks;
    }
}
