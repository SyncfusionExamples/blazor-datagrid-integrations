using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using Elastic.Clients.Elasticsearch.Mapping;

namespace Grid_ElasticSearch.Data
{
    /// <summary>
    /// Service to initialize ElasticSearch indexes and settings on application startup.
    /// Ensures required indexes exist before the application attempts to use them.
    /// Also syncs static inventory data to ElasticSearch index.
    /// </summary>
    public class ElasticSearchInitializationService
    {
        private readonly ElasticsearchClient _elasticClient;
        private readonly InventoryDataService _inventoryDataService;
        private const string IndexName = "inventory-items";

        /// <summary>
        /// Constructor to inject ElasticSearch client and inventory data service dependencies.
        /// </summary>
        public ElasticSearchInitializationService(ElasticsearchClient elasticClient, InventoryDataService inventoryDataService)
        {
            _elasticClient = elasticClient;
            _inventoryDataService = inventoryDataService;
        }

        /// <summary>
        /// Initializes ElasticSearch indexes on application startup.
        /// Creates the 'inventory-items' index if it doesn't already exist.
        /// Also syncs static inventory data to ElasticSearch.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                var existsResponse = await _elasticClient.Indices.ExistsAsync(IndexName);
                if (existsResponse.Exists) return;

                _inventoryDataService.InitializeSeedData();

                var createRequest = new CreateIndexRequest(IndexName)
                {
                    Settings = new IndexSettings
                    {
                        NumberOfShards = 1,
                        NumberOfReplicas = 0,
                        MaxResultWindow = 10000000
                    },
                    Mappings = new TypeMapping
                    {
                        Properties = new Properties
                        {
                            { "itemId", new IntegerNumberProperty() },
                            { "sku", new TextProperty
                                {
                                    Fields = new Properties
                                    {
                                        { "keyword", new KeywordProperty() },
                                    }
                                } 
                            },

                            {
                                "itemName",
                                new TextProperty
                                {
                                    Fields = new Properties
                                    {
                                        { "keyword", new KeywordProperty() },
                                    }
                                }
                            },

                            { "category", new TextProperty
                                {
                                    Fields = new Properties
                                    {
                                        { "keyword", new KeywordProperty() },
                                    }
                                } 
                            },

                            { "supplier", new TextProperty
                                {
                                    Fields = new Properties
                                    {
                                        { "keyword", new KeywordProperty() },
                                    }
                                }  
                            },

                            { "unitPrice", new DoubleNumberProperty() },

                            { "quantityInStock", new IntegerNumberProperty() },

                            { "reorderLevel", new IntegerNumberProperty() },

                            { "reorderQuantity", new IntegerNumberProperty() },

                            { "warehouse", new TextProperty
                                {
                                    Fields = new Properties
                                    {
                                        { "keyword", new KeywordProperty() },
                                    }
                                }  
                            },

                            { "lastRestocked", new DateProperty() },

                            { "status", new TextProperty
                                {
                                    Fields = new Properties
                                    {
                                        { "keyword", new KeywordProperty() },
                                    }
                                }  
                            }
                        }
                    }
                };

                var createResponse = await _elasticClient.Indices.CreateAsync(createRequest);

                if (createResponse.IsValidResponse)
                {
                    Console.WriteLine($"✓ ElasticSearch index '{IndexName}' created successfully with field mappings!");
                }
                else
                {
                    Console.WriteLine($"✗ Failed to create ElasticSearch index '{IndexName}': {createResponse.ApiCallDetails?.DebugInformation}");
                    throw new Exception($"Failed to create index: {createResponse.ApiCallDetails?.DebugInformation}");
                }
                await SyncSeedDataToElasticSearchAsync();

                Console.WriteLine("✓ ElasticSearch Initialization completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ ElasticSearch Initialization Error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Syncs the in-memory seed data to ElasticSearch index.
        /// Clears the index first if it's not a new index, then indexes all seed data.
        /// </summary>
        private async Task SyncSeedDataToElasticSearchAsync()
        {
            try
            {
                // Get all seed data items
                var inventoryItems = _inventoryDataService.GetAllInventoryItems();

                if (inventoryItems.Count == 0)
                {
                    Console.WriteLine("⚠️  No inventory items to sync to ElasticSearch");
                    return;
                }

                // Bulk index all seed data using IndexMany
                var bulkResponse = await _elasticClient.BulkAsync(b => b
                    .IndexMany(inventoryItems, (descriptor, item) => descriptor
                        .Index(IndexName)
                        .Id(item.ItemId.ToString())
                    )
                );

                if (bulkResponse.IsValidResponse)
                {
                    Console.WriteLine($"✓ Successfully synced {inventoryItems.Count} seed items to ElasticSearch");
                }
                else
                {
                    Console.WriteLine($"✗ Bulk indexing failed: {bulkResponse.ApiCallDetails?.DebugInformation}");
                    throw new Exception($"Failed to sync seed data: {bulkResponse.ApiCallDetails?.DebugInformation}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error syncing seed data to ElasticSearch: {ex.Message}");
                throw;
            }
        }
    }
}
