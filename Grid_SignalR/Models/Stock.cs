namespace Grid_SignalR.Models;

public class Stock
{
    /// <summary>
    /// Gets or sets the unique identifier for the stock.
    /// </summary>
    public int StockId { get; set; }

    /// <summary>
    /// Gets or sets the ticker symbol of the stock (e.g., AAPL, MSFT).
    /// </summary>
    public string Symbol { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full company name.
    /// </summary>
    public string Company { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current price of the stock.
    /// </summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// Gets or sets the previous price before the last update.
    /// Used to calculate price changes.
    /// </summary>
    public decimal PreviousPrice { get; set; }

    /// <summary>
    /// Gets or sets the price change in absolute value.
    /// Calculated as CurrentPrice - PreviousPrice.
    /// </summary>
    public decimal Change { get; set; }

    /// <summary>
    /// Gets or sets the percentage change of the stock price.
    /// Calculated as (Change / PreviousPrice) * 100.
    /// </summary>
    public decimal ChangePercent { get; set; }

    /// <summary>
    /// Gets or sets the trading volume (number of shares traded).
    /// </summary>
    public long Volume { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the last price update.
    /// </summary>
    public DateTime LastUpdated { get; set; }
}
