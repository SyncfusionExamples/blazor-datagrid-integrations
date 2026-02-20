using Grid_SignalR.Models;

namespace Grid_SignalR.Services;

public class StockDataService
{
    private readonly List<Stock> _stocks = [];
    private readonly Random _random = new();
    
    public StockDataService()
    {
        InitializeStocks();
    }

    private void InitializeStocks()
    {
        var stockData = new[]
        {
           // Technology
            ("AAPL", "Apple Inc.", 190.50m),
            ("MSFT", "Microsoft Corporation", 380.25m),
            ("GOOGL", "Alphabet Inc.", 140.75m),
            ("AMZN", "Amazon.com Inc.", 180.50m),
            ("NVDA", "NVIDIA Corporation", 870.20m),
            ("META", "Meta Platforms Inc.", 520.15m),
            ("TSLA", "Tesla Inc.", 242.80m),
            ("CRM", "Salesforce Inc.", 285.40m),
            ("ADBE", "Adobe Inc.", 520.60m),
            ("INTC", "Intel Corporation", 32.15m),
            ("AMD", "Advanced Micro Devices", 210.80m),
            ("QCOM", "Qualcomm Inc.", 165.30m),
            ("CSCO", "Cisco Systems Inc.", 48.20m),
            ("AMAT", "Applied Materials Inc.", 220.75m),
            ("LRCX", "Lam Research Corporation", 780.45m),
            ("ASML", "ASML Holding N.V.", 720.20m),
            ("AVGO", "Broadcom Inc.", 145.60m),
            ("MU", "Micron Technology Inc.", 95.40m),
            ("NXPI", "NXP Semiconductors", 210.15m),
            ("MCHP", "Microchip Technology Inc.", 72.85m),

            // Financial Services
            ("JPM", "JPMorgan Chase & Co.", 195.75m),
            ("BAC", "Bank of America Corp.", 42.50m),
            ("WFC", "Wells Fargo & Company", 70.20m),
            ("GS", "The Goldman Sachs Group Inc.", 510.85m),
            ("MS", "Morgan Stanley", 98.40m),
            ("BLK", "BlackRock Inc.", 890.15m),
            ("AXP", "American Express Company", 245.60m),
            ("USB", "U.S. Bancorp", 45.75m),
            ("PNC", "PNC Financial Services", 185.40m),
            ("TD", "Toronto-Dominion Bank", 68.90m),
            ("RY", "Royal Bank of Canada", 115.20m),
            ("BNS", "Scotiabank", 72.50m),
            ("BMO", "Bank of Montreal", 108.75m),
            ("CM", "Canadian Imperial Bank", 58.40m),
            ("SLF", "Sun Life Financial Inc.", 68.20m),
            ("MFC", "Manulife Financial Corporation", 23.15m),
            ("GWC", "Great-West Lifeco Inc.", 32.80m),
            ("TRI", "Thomson Reuters Corporation", 165.45m),
            ("RCI", "Rogers Communications Inc.", 44.75m),
            ("BCE", "BCE Inc.", 40.20m),

            // Healthcare & Pharmaceuticals
            ("JNJ", "Johnson & Johnson", 158.45m),
            ("UNH", "UnitedHealth Group Incorporated", 495.80m),
            ("PFE", "Pfizer Inc.", 28.60m),
            ("ABBV", "AbbVie Inc.", 285.20m),
            ("LLY", "Eli Lilly and Company", 745.30m),
            ("MRK", "Merck & Co Inc.", 65.90m),
            ("TMO", "Thermo Fisher Scientific Inc.", 525.75m),
            ("AZN", "AstraZeneca PLC", 68.40m),
            ("AMGN", "Amgen Inc.", 295.15m),
            ("GILD", "Gilead Sciences Inc.", 78.85m),
            ("REGN", "Regeneron Pharmaceuticals Inc.", 975.20m),
            ("BIIB", "Biogen Inc.", 240.50m),
            ("CVS", "CVS Health Corporation", 75.40m),
            ("WBA", "Walgreens Boots Alliance Inc.", 28.75m),
            ("HCA", "HCA Healthcare Inc.", 310.60m),
            ("UHS", "Universal Health Services Inc.", 245.85m),
            ("THC", "Tenet Healthcare Corporation", 95.20m),
            ("VEEV", "Veeva Systems Inc.", 172.45m),
            ("DXCM", "DexCom Inc.", 105.80m),
            ("INTU", "Intuit Inc.", 685.25m),

            // Consumer Discretionary
            ("AMZN", "Amazon.com Inc.", 180.50m),
            ("TSLA", "Tesla Inc.", 242.80m),
            ("MCD", "McDonald's Corporation", 298.75m),
            ("NKE", "Nike Inc.", 82.40m),
            ("HD", "The Home Depot Inc.", 418.20m),
            ("LOW", "Lowe's Companies Inc.", 245.60m),
            ("WMT", "Walmart Inc.", 92.50m),
            ("TGT", "Target Corporation", 78.20m),
            ("COST", "Costco Wholesale Corporation", 945.80m),
            ("ROST", "Ross Stores Inc.", 102.35m),
            ("DXY", "Dixie Brands", 12.40m),
            ("GPS", "The Gap Inc.", 24.15m),
            ("KSS", "Kohl's Corporation", 18.95m),
            ("BBY", "Best Buy Co. Inc.", 95.60m),
            ("GME", "GameStop Corp.", 28.75m),
            ("DISH", "DISH Network Corporation", 18.40m),
            ("F", "Ford Motor Company", 10.20m),
            ("GM", "General Motors Company", 42.85m),
            ("TM", "Toyota Motor Corporation", 185.40m),
            ("HMC", "Honda Motor Company Inc.", 28.60m),

            // Industrials & Materials
            ("BA", "The Boeing Company", 218.50m),
            ("CAT", "Caterpillar Inc.", 385.20m),
            ("GE", "General Electric Company", 172.40m),
            ("MMM", "3M Company", 108.75m),
            ("RTX", "Raytheon Technologies Corporation", 125.60m),
            ("LMT", "Lockheed Martin Corporation", 485.80m),
            ("NOC", "Northrop Grumman Corporation", 545.20m),
            ("GD", "General Dynamics Corporation", 285.40m),
            ("DOW", "Dow Inc.", 52.15m),
            ("DD", "DuPont de Nemours Inc.", 78.85m),
            ("FCX", "Freeport-McMoRan Inc.", 42.60m),
            ("NUCOR", "Nucor Corporation", 182.75m),
            ("CMC", "Commercial Metals Company", 48.20m),
            ("X", "United States Steel Corporation", 38.45m),
            ("AA", "Alcoa Corporation", 52.80m),
            ("CLF", "Cleveland-Cliffs Inc.", 18.60m),
            ("STLD", "Steel Dynamics Inc.", 72.40m),
            ("LIN", "Linde PLC", 465.90m),
            ("APD", "Air Products and Chemicals Inc.", 285.20m),
            ("EMN", "Eastman Chemical Company", 78.50m),

            // Energy
            ("XOM", "Exxon Mobil Corporation", 118.40m),
            ("CVX", "Chevron Corporation", 165.80m),
            ("COP", "ConocoPhillips", 125.20m),
            ("SLB", "Schlumberger Limited", 55.40m),
            ("EOG", "EOG Resources Inc.", 105.60m),
            ("MPC", "Marathon Petroleum Corporation", 185.75m),
            ("PSX", "Phillips 66", 125.40m),
            ("VLO", "Valero Energy Corporation", 145.80m),
            ("HES", "Hess Corporation", 165.20m),
            ("PXD", "Pioneer Natural Resources", 245.60m),
            ("OXY", "Occidental Petroleum Corporation", 62.80m),
            ("APA", "APA Corporation", 28.40m),
            ("DVN", "Devon Energy Corporation", 45.20m),
            ("FANG", "Diamondback Energy Inc.", 165.85m),
            ("MRO", "Marathon Oil Corporation", 22.60m),
            ("NBL", "Noble Corporation PLC", 52.40m),
            ("CIVI", "Civista Bancshares Inc.", 24.15m),
            ("KMI", "Kinder Morgan Inc.", 28.75m),
            ("ET", "Energy Transfer LP", 12.85m),
            ("MMP", "Magellan Midstream Partners L.P.", 48.20m),

            // Utilities
            ("NEE", "NextEra Energy Inc.", 68.40m),
            ("DUK", "Duke Energy Corporation", 98.20m),
            ("SO", "The Southern Company", 72.50m),
            ("DTE", "DTE Energy Company", 125.80m),
            ("EXC", "Exelon Corporation", 42.15m),
            ("AEP", "American Electric Power Company Inc.", 88.60m),
            ("XEL", "Xcel Energy Inc.", 65.40m),
            ("PEG", "Public Service Enterprise Group Inc.", 68.75m),
            ("ED", "Consolidated Edison Inc.", 85.20m),
            ("AES", "The AES Corporation", 25.45m),
            ("EIX", "Edison International", 78.40m),
            ("AWK", "American Water Works Company Inc.", 155.80m),
            ("CWT", "California Water Service Group", 48.20m),
            ("AWH", "Aspire Water Holdings Inc.", 18.60m),
            ("NRG", "NRG Energy Inc.", 38.75m),
            ("CMS", "CMS Energy Corporation", 62.40m),
            ("FE", "FirstEnergy Corp.", 38.85m),
            ("LNT", "Alliant Energy Corporation", 58.20m),
            ("WEC", "WEC Energy Group Inc.", 92.50m),
            ("PPL", "PPL Corporation", 32.15m),

            // Real Estate
            ("DLR", "Digital Realty Trust Inc.", 185.40m),
            ("EQIX", "Equinix Inc.", 625.80m),
            ("VICI", "VICI Properties Inc.", 38.20m),
            ("PLD", "Prologis Inc.", 108.60m),
            ("AMT", "American Tower Corporation", 265.20m),
            ("CCI", "Crown Castle International Corp.", 125.40m),
            ("SBAC", "SBA Communications Corporation", 398.75m),
            ("SPG", "Simon Property Group Inc.", 185.50m),
            ("PEI", "Pennsylvania REIT", 8.40m),
            ("MAC", "Macerich Company", 18.60m),
            ("BXP", "Boston Properties Inc.", 95.20m),
            ("OFC", "Monmouth Real Estate Investment Corporation", 2.50m),
            ("VNO", "Vornado Realty Trust", 38.45m),
            ("RXP", "REX Real Estate Investment Trust", 18.75m),
            ("ARE", "Alexandria Real Estate Equities Inc.", 145.80m),
            ("WELL", "Welltower Inc.", 72.40m),
            ("PTC", "PotlatchDeltic Corporation", 52.80m),
            ("UMH", "UMH Properties Inc.", 22.60m),
            ("SRC", "Sotherly Bank Inc.", 15.20m),
            ("AKR", "Acadia Realty Trust", 18.95m),

            // Consumer Staples
            ("PG", "The Procter & Gamble Company", 168.40m),
            ("KO", "The Coca-Cola Company", 65.20m),
            ("PEP", "PepsiCo Inc.", 195.80m),
            ("MO", "Altria Group Inc.", 58.40m),
            ("PM", "Philip Morris International Inc.", 105.60m),
            ("GIS", "General Mills Inc.", 78.20m),
            ("CAG", "Conagra Brands Inc.", 32.15m),
            ("K", "Kellogg Company", 22.80m),
            ("MNST", "Monster Beverage Corporation", 58.40m),
            ("CELH", "Celsius Holdings Inc.", 35.60m),
            ("BF/B", "Brown-Forman Corporation", 48.50m),
            ("DEO", "Diageo PLC", 88.40m),
            ("STZ", "Constellation Brands Inc.", 285.20m),
            ("TAP", "Molson Coors Beverage Company", 48.60m),
            ("SJM", "The J.M. Smucker Company", 142.45m),
            ("TSN", "Tyson Foods Inc.", 38.80m),
            ("JBS", "JBS S.A.", 28.40m),
            ("HRL", "Hormel Foods Corporation", 52.75m),
            ("SMPL", "Simply Goods Inc.", 18.60m),
            ("AGRO", "Adecoagro S.A.", 8.40m),

            // Communications
            ("CMCSA", "Comcast Corporation", 45.20m),
            ("CHTR", "Charter Communications Inc.", 385.80m),
            ("TWX", "Time Warner Inc.", 85.40m),
            ("FOX", "Fox Corporation", 28.50m),
            ("FOXA", "Fox Corporation Class A", 28.60m),
            ("VIAC", "ViacomCBS Inc.", 18.40m),
            ("DIS", "The Walt Disney Company", 95.20m),
            ("NFLX", "Netflix Inc.", 285.40m),
            ("ROKU", "Roku Inc.", 68.20m),
            ("PENN", "Penn Entertainment Inc.", 22.85m),
            ("LYV", "Live Nation Entertainment Inc.", 142.60m),
            ("RCI", "Rogers Communications Inc.", 44.75m),
            ("BCE", "BCE Inc.", 40.20m),
            ("T", "AT&T Inc.", 22.50m),
            ("VZ", "Verizon Communications Inc.", 42.80m),
            ("TMUS", "T-Mobile US Inc.", 195.40m),
            ("S", "Sprint Corporation", 5.20m),
            ("DISH", "DISH Network Corporation", 18.40m),
            ("SIRIUSX", "Sirius XM Holdings Inc.", 28.60m),
            ("LBRDK", "Liberty Braves Group", 32.40m),

            // Additional Tech & Software
            ("FTNT", "Fortinet Inc.", 68.20m),
            ("PALO", "Palo Alto Networks Inc.", 285.40m),
            ("NET", "Cloudflare Inc.", 95.80m),
            ("CRWD", "CrowdStrike Holdings Inc.", 425.20m),
            ("OKTA", "Okta Inc.", 118.40m),
            ("ZS", "Zscaler Inc.", 142.60m),
            ("WORK", "Slack Technologies Inc.", 48.20m),
            ("ZOOM", "Zoom Video Communications Inc.", 165.80m),
            ("TWLO", "Twilio Inc.", 52.40m),
            ("RBLX", "Roblox Corporation", 42.15m),
            ("SNAP", "Snap Inc.", 25.80m),
            ("PINS", "Pinterest Inc.", 32.40m),
            ("COIN", "Coinbase Global Inc.", 125.60m),
            ("MSTR", "MicroStrategy Incorporated", 485.20m),
            ("SQ", "Square Inc.", 68.40m),
            ("PYPL", "PayPal Holdings Inc.", 78.20m),
            ("V", "Visa Inc.", 295.40m),
            ("MA", "Mastercard Incorporated", 518.80m),
            ("DFS", "Discover Financial Services", 125.40m),
            ("ACI", "Advance Auto Parts Inc.", 15.20m),

            // Additional Technology & Software
            ("SPLK", "Splunk Inc.", 155.40m),
            ("DDOG", "Datadog Inc.", 195.80m),
            ("SNOW", "Snowflake Inc.", 185.20m),
            ("DBX", "Dropbox Inc.", 42.60m),
            ("CrowdStrike", "CrowdStrike Holdings Inc.", 425.20m),
            ("NFLX", "Netflix Inc.", 285.40m),
            ("ROKU", "Roku Inc.", 68.20m),
            ("GTLB", "Gitlab Inc.", 78.40m),
            ("MNDY", "Monday.com Ltd.", 195.60m),
            ("SMCI", "Super Micro Computer Inc.", 68.40m),

            // Additional Healthcare
            ("SGEN", "Seagen Inc.", 165.80m),
            ("VEEV", "Veeva Systems Inc.", 172.45m),
            ("EXAS", "Exact Sciences Corporation", 78.20m),
            ("ILMN", "Illumina Inc.", 115.40m),
            ("VRTX", "Vertex Pharmaceuticals Inc.", 485.20m),
            ("ALKS", "Alkermes PLC", 42.60m),
            ("SAGE", "Sage Therapeutics Inc.", 28.40m),
            ("CARA", "Cara Therapeutics Inc.", 35.20m),
            ("BNTX", "Biontech SE", 125.40m),
            ("MRNA", "Moderna Inc.", 185.80m),

            // Additional Financial Services
            ("SCHW", "Charles Schwab Corporation", 82.40m),
            ("HOOD", "Robinhood Markets Inc.", 32.60m),
            ("UPST", "Upstart Holdings Inc.", 48.20m),
            ("SQ", "Square Inc.", 68.40m),
            ("PYPL", "PayPal Holdings Inc.", 78.20m),
            ("INFA", "Informatica Inc.", 38.60m),
            ("SOFI", "SoFi Technologies Inc.", 22.80m),
            ("COIN", "Coinbase Global Inc.", 125.60m),
            ("MSTR", "MicroStrategy Incorporated", 485.20m),
            ("MARA", "Marathon Digital Holdings Inc.", 18.40m),

            // Additional Consumer & Retail
            ("ULTA", "Ulta Beauty Inc.", 428.60m),
            ("GPC", "Genuine Parts Company", 185.20m),
            ("FIVE", "Five Below Inc.", 52.40m),
            ("AMTM", "Artisan Partners Asset Management", 65.80m),
            ("LULU", "Lululemon Athletica Inc.", 385.40m),
            ("DECK", "Deckers Outdoor Corporation", 795.20m),
            ("CROX", "Crocs Inc.", 95.80m),
            ("VIPS", "Vipshop Holdings Limited", 28.40m),
            ("NU", "Nu Holdings Ltd.", 8.60m),
            ("MGNX", "MagneX Holdings Inc.", 5.40m),

            // Additional Industrials & Manufacturing
            ("RBLX", "Roblox Corporation", 42.15m),
            ("PTON", "Peloton Interactive Inc.", 12.40m),
            ("LCID", "Lucid Motors Inc.", 2.80m),
            ("NIO", "NIO Inc.", 5.20m),
            ("XP", "XP Inc.", 18.40m),
            ("AVAV", "AeroVironment Inc.", 285.60m),
            ("ATGE", "Allegiant Travel Company", 95.20m),
            ("ALK", "Alaska Air Group Inc.", 42.60m),
            ("DAL", "Delta Air Lines Inc.", 52.40m),
            ("UAL", "United Airlines Holdings Inc.", 68.20m),

            // Additional Energy & Resources
            ("RDS.A", "Royal Dutch Shell PLC Class A", 58.40m),
            ("TTE", "TotalEnergies SE", 65.80m),
            ("ENB", "Enbridge Inc.", 38.20m),
            ("TC", "TC Energy Corporation", 52.40m),
            ("PAA", "Plains All American Pipeline L.P.", 28.60m),
            ("WMB", "Williams Companies Inc.", 42.40m),
            ("NGL", "NGL Energy Partners L.P.", 22.80m),
            ("ARCH", "Arch Coal Inc.", 148.60m),
            ("BTU", "Peabody Energy Corporation", 28.40m),
            ("AR", "Antero Resources Corporation", 32.20m),

            // Additional Real Estate & Property
            ("SKT", "Tanger Inc.", 18.40m),
            ("WPG", "Washington Prime Group Inc.", 2.80m),
            ("KIM", "Kimco Realty Corporation", 25.20m),
            ("REG", "Regency Centers Corporation", 68.40m),
            ("RHP", "Retail Opportunity Investments Corp.", 38.60m),
            ("SITM", "Sitemark Holdings Inc.", 15.40m),
            ("STOR", "STORE Capital Corporation", 38.20m),
            ("MAIN", "Mainstay Apartment Communities Inc.", 12.80m),
            ("NHI", "National Health Investors Inc.", 52.40m),
            ("LTC", "LTC Properties Inc.", 28.60m),

            // Additional Utilities & Energy Infrastructure
            ("PSA", "Public Storage", 385.20m),
            ("EQR", "Equity Residential", 68.40m),
            ("AVB", "AvalonBay Communities Inc.", 242.60m),
            ("CPT", "Camden Property Trust", 95.20m),
            ("UMH", "UMH Properties Inc.", 22.60m),
            ("AGR", "Agrify Holdings Inc.", 2.40m),
            ("VATE", "Viata Energy Inc.", 8.20m),
            ("PRIM", "Primotech Inc.", 5.60m),
            ("NWLI", "National Western Life Group Inc.", 385.80m),
            ("TXRH", "Texas Roadhouse Inc.", 78.20m),

            // Additional Diversified / Conglomerate
            ("ITW", "Illinois Tool Works Inc.", 245.80m),
            ("HOG", "Harley-Davidson Inc.", 35.20m),
            ("LEG", "Leggett & Platt Incorporated", 28.40m),
            ("WHR", "Whirlpool Corporation", 125.60m),
            ("NWL", "Newell Brands Inc.", 18.40m),
            ("SCCO", "Southern Copper Corporation", 95.20m),
            ("TECK", "Teck Resources Limited", 28.60m),
            ("VALE", "Vale S.A.", 12.40m),
            ("RIO", "Rio Tinto Limited", 68.20m),
            ("BHP", "BHP Group Limited", 52.80m),
        };

        int id = 1;
        foreach (var (symbol, company, price) in stockData)
        {
            _stocks.Add(new Stock
            {
                StockId = id++,
                Symbol = symbol,
                Company = company,
                CurrentPrice = price,
                PreviousPrice = price,
                Change = 0,
                ChangePercent = 0,
                Volume = _random.Next(1000000, 100000000),
                LastUpdated = DateTime.Now
            });
        }
    }

    public List<Stock> GetAllStocks()
    {
        return _stocks.OrderBy(s => s.Symbol).ToList();
    }

    public void UpdateStockPrices()
    {
        var now = DateTime.Now;
        
        foreach (var stock in _stocks)
        {
            stock.PreviousPrice = stock.CurrentPrice;
            
            decimal changePercent = (decimal)(_random.NextDouble() - 0.5) * 0.04m;
            decimal newPrice = stock.CurrentPrice * (1 + changePercent);
            
            stock.CurrentPrice = decimal.Round(newPrice, 2);
            stock.Change = decimal.Round(stock.CurrentPrice - stock.PreviousPrice, 2);
            stock.ChangePercent = decimal.Round((stock.Change / stock.PreviousPrice) * 100, 2);
            stock.Volume = _random.Next(1000000, 100000000);
            stock.LastUpdated = now;
        }
    }
}
