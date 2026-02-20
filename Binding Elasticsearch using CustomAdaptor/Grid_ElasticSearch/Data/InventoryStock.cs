using System.Text.Json.Serialization;

namespace Grid_ElasticSearch.Data
{
    /// <summary>
    /// Represents an inventory stock item in the warehouse management system.
    /// Maps to the 'inventory-items' index in ElasticSearch.
    /// </summary>
    public class InventoryStock
    {
        /// <summary>
        /// Gets or sets the unique identifier for the inventory item.
        /// </summary>
        [JsonPropertyName("itemId")]
        public int ItemId { get; set; }

        /// <summary>
        /// Gets or sets the SKU (Stock Keeping Unit) code for the item.
        /// </summary>
        [JsonPropertyName("sku")]
        public string? SKU { get; set; }

        /// <summary>
        /// Gets or sets the name of the inventory item.
        /// </summary>
        [JsonPropertyName("itemName")]
        public string? ItemName { get; set; }

        /// <summary>
        /// Gets or sets the category of the item (e.g., Electronics, Hardware, Software).
        /// </summary>
        [JsonPropertyName("category")]
        public string? Category { get; set; }

        /// <summary>
        /// Gets or sets the supplier name for the item.
        /// </summary>
        [JsonPropertyName("supplier")]
        public string? Supplier { get; set; }

        /// <summary>
        /// Gets or sets the unit price of the item.
        /// </summary>
        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Gets or sets the current quantity in stock.
        /// </summary>
        [JsonPropertyName("quantityInStock")]
        public int QuantityInStock { get; set; }

        /// <summary>
        /// Gets or sets the minimum quantity threshold for reordering.
        /// </summary>
        [JsonPropertyName("reorderLevel")]
        public int ReorderLevel { get; set; }

        /// <summary>
        /// Gets or sets the standard quantity to order when restocking.
        /// </summary>
        [JsonPropertyName("reorderQuantity")]
        public int ReorderQuantity { get; set; }

        /// <summary>
        /// Gets or sets the warehouse location of the item.
        /// </summary>
        [JsonPropertyName("warehouse")]
        public string? Warehouse { get; set; }

        /// <summary>
        /// Gets or sets the date when the item was last restocked.
        /// </summary>
        [JsonPropertyName("lastRestocked")]
        public DateTime? LastRestocked { get; set; }

        /// <summary>
        /// Gets or sets the current status of the item (e.g., Active, Inactive, Discontinued).
        /// </summary>
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }
}