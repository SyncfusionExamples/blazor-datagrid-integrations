namespace Grid_ElasticSearch.Data
{
    /// <summary>
    /// Service to manage static inventory data in memory.
    /// Provides seed data initialization with 20 initial inventory items.
    /// This serves as the primary data source, with ElasticSearch as the search index.
    /// </summary>
    public class InventoryDataService
    {
        /// <summary>
        /// Static in-memory collection of inventory items
        /// Persists throughout the application lifetime
        /// </summary>
        private static List<InventoryStock> _inventoryData = new List<InventoryStock>();

        /// <summary>
        /// Gets all inventory items from the static list
        /// </summary>
        /// <returns>List of all inventory items</returns>
        public List<InventoryStock> GetAllInventoryItems()
        {
            return new List<InventoryStock>(_inventoryData);
        }

        /// <summary>
        /// Initializes the static list with dynamically generated seed inventory items
        /// Called during application startup
        /// All dates are normalized to midnight (00:00:00) for consistent date-only filtering
        /// </summary>
        public void InitializeSeedData()
        {
            // Only initialize if list is empty
            if (_inventoryData.Count > 0)
            {
                Console.WriteLine("✓ Seed data already initialized. Skipping initialization.");
                return;
            }

            int numberOfRecords = 100000;

            var warehouses = new List<string> { "Warehouse-A", "Warehouse-B", "Warehouse-C", "Warehouse-D", "Warehouse-E" };

            var itemNames = new List<string>
            {
                "Dell XPS 15 Laptop", "Lenovo ThinkPad X1", "HP ProBook 450", "24\" Monitor 4K", "Mechanical Keyboard RGB",
                "Logitech Mouse MX Master", "Microsoft Office 365", "Adobe Creative Cloud", "Visual Studio Professional",
                "Office Desk Chair", "Standing Desk 60\"", "Bookshelf 5-Tier", "USB-C Hub 7-in-1", "Wireless Charger Pad",
                "USB-A Cable (3m)", "Notebook A4 100 Pages", "Ballpoint Pen (Pack of 50)", "Printer Paper Ream 500 Sheets",
                "Stapler Heavy Duty", "Filing Cabinet 4-Drawer", "Monitor Stand Adjustable", "Desk Lamp LED", "Cable Organizer",
                "External Hard Drive 2TB", "Laptop Stand Aluminum", "USB Hub 4-Port", "Screen Protector", "Keyboard Wrist Rest",
                "Mouse Pad XL", "Webcam Full HD", "Headphones Wireless", "Microphone Condenser"
            };

            var categories = new List<string>
            {
                "Electronics", "Hardware", "Software", "Furniture", "Office Supplies", "Accessories", "Networking", "Storage"
            };

            var suppliers = new List<string>
            {
                "Dell Direct", "Lenovo Corp", "HP Inc", "LG Electronics", "Corsair Gaming", "Logitech Inc", "Microsoft",
                "Adobe Systems", "Herman Miller", "Flexispot", "IKEA", "Anker", "Belkin", "AmazonBasics", "Rhodia",
                "Parker", "Xerox", "Swingline", "Steelcase", "3M", "Canon", "Epson", "Sony", "Samsung"
            };

            var statuses = new List<string> { "Active", "Inactive", "Discontinued" };

            // ============ RANDOM GENERATION ============
            var random = new Random();
            var today = DateTime.Today;

            _inventoryData = new List<InventoryStock>();

            for (int i = 1; i <= numberOfRecords; i++)
            {
                int itemId = 1000 + i;
                string sku = $"SKU-{i:D6}"; // Generates SKU-000001, SKU-000002, etc.
                
                // RandomDays for LastRestocked: between -30 and -1
                int randomDays = random.Next(-30, 0); // -30 to -1 inclusive

                var item = new InventoryStock
                {
                    ItemId = itemId,
                    SKU = sku,
                    ItemName = itemNames[random.Next(itemNames.Count)],
                    Category = categories[random.Next(categories.Count)],
                    Supplier = suppliers[random.Next(suppliers.Count)],
                    UnitPrice = (decimal)(random.NextDouble() * 1500 + 5), // Price between $5 and $1505
                    QuantityInStock = random.Next(10, 5000),
                    ReorderLevel = random.Next(5, 500),
                    ReorderQuantity = random.Next(20, 2000),
                    Warehouse = warehouses[random.Next(warehouses.Count)],
                    LastRestocked = today.AddDays(randomDays),
                    Status = statuses[random.Next(statuses.Count)]
                };

                _inventoryData.Add(item);
            }

            Console.WriteLine($"✓ Initialized seed data with {_inventoryData.Count} inventory items (all dates normalized to midnight)");
        }
    }
}
