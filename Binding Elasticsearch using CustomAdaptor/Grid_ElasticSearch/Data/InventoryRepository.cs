using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.Fluent;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Data;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Query = Elastic.Clients.Elasticsearch.QueryDsl.Query;
using SortOrder = Elastic.Clients.Elasticsearch.SortOrder;


namespace Grid_ElasticSearch.Data
{
    /// <summary>
    /// Repository pattern implementation for InventoryStock entity using Elastic.Clients.Elasticsearch.
    /// Handles all CRUD operations and search access for inventory items using ElasticSearch QueryDSL.
    /// Replaces DataOperations with native ES queries for optimal performance.
    /// </summary>
    public class InventoryRepository
    {
        private readonly ElasticsearchClient _elasticClient;
        private readonly InventoryDataService _inventoryDataService;
        private const string IndexName = "inventory-items";

        /// <summary>
        /// Constructor to inject ElasticSearch client and inventory data service dependencies.
        /// </summary>
        public InventoryRepository(ElasticsearchClient elasticClient, InventoryDataService inventoryDataService)
        {
            _elasticClient = elasticClient;
            _inventoryDataService = inventoryDataService;
        }

        /// <summary>
        /// Retrieves all inventory items from static list.
        /// Returns the complete in-memory dataset for reference.
        /// </summary>
        /// <returns>List of all inventory items</returns>
        public async Task<List<InventoryStock>> GetInventoryItemsAsync()
        {
            try
            {
                // Return from static list (primary data source)
                return await Task.FromResult(_inventoryDataService.GetAllInventoryItems());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving inventory items: {ex.Message}");
                throw;
            }
        }

        public async Task<DataResult> SearchAndFilterAsync(DataManagerRequest dm)
        {
            // Build ES sorts from dm.Sorted
            var esSorts = BuildEsSorts(dm.Sorted);

            var response = await _elasticClient.SearchAsync<InventoryStock>(s => s
                .Indices(IndexName)
                .Query(q => BuildEsQueryFromDm(q, dm))
                .TrackTotalHits(true)
                .Sort(esSorts)
                .From(dm.Skip)
                .Size(dm.Take)
                .Aggregations(agg => BuildEsAggregations(agg, dm.Aggregates))
            );

            if (!response.IsValidResponse)
            {
                throw new InvalidOperationException(response.ApiCallDetails?.DebugInformation ?? "Search failed.");
            }
            long totalCount = response.Total > 0 ? response.Total : 0L;

            IDictionary<string, object>? aggregates = null;
            if (response.Aggregations != null && response.Aggregations.Count > 0)
            {
                aggregates = ExtractAggregatesFromResponse(response.Aggregations);
            }


            return new DataResult { Result = response.Documents, Count = (int)totalCount, Aggregates = aggregates };
        }

        /// <summary>
        /// Builds ElasticSearch aggregations from DataManagerRequest.Aggregates
        /// Format: "FieldName - AggregateType" (e.g., "QuantityInStock - sum")
        /// Uses FluentDictionaryOfStringAggregation API pattern
        /// </summary>
        private static FluentDictionaryOfStringAggregation<InventoryStock> BuildEsAggregations(
            FluentDictionaryOfStringAggregation<InventoryStock> aggregations,
            List<Aggregate>? aggregates)
        {
            if (aggregates == null || aggregates.Count == 0)
                return aggregations;

            foreach (var aggregate in aggregates)
            {
                if (string.IsNullOrWhiteSpace(aggregate.Field))
                    continue;

                string aggName = $"{aggregate.Field} - {aggregate?.Type?.ToLower()}";
                string? field = GetJsonPropertyName(aggregate.Field);

                // Add aggregation based on type using .Add() pattern
                switch ((aggregate.Type ?? "sum").ToLowerInvariant())
                {
                    case "sum":
                        aggregations.Add(aggName, agg => agg.Sum(s => s.Field(field)));
                        break;

                    case "average":
                    case "avg":
                        aggregations.Add(aggName, agg => agg.Avg(s => s.Field(field)));
                        break;

                    case "count":
                        aggregations.Add(aggName, agg => agg.ValueCount(s => s.Field(field)));
                        break;

                    case "max":
                        aggregations.Add(aggName, agg => agg.Max(s => s.Field(field)));
                        break;

                    case "min":
                        aggregations.Add(aggName, agg => agg.Min(s => s.Field(field)));
                        break;

                    case "distinct":
                    case "cardinality":
                        aggregations.Add(aggName, agg => agg.Cardinality(s => s.Field(field)));
                        break;

                    default:
                        aggregations.Add(aggName, agg => agg.Sum(s => s.Field(field))); // Default to sum
                        break;
                }
            }

            return aggregations;
        }

        /// <summary>
        /// Extracts aggregation results from ElasticSearch response
        /// and converts them to the format expected by Syncfusion Grid
        /// </summary>
        private static IDictionary<string, object> ExtractAggregatesFromResponse(IReadOnlyDictionary<string, IAggregate> esAggregations)
        {
            var aggregates = new Dictionary<string, object>();

            foreach (var aggEntry in esAggregations)
            {
                var aggName = aggEntry.Key; // e.g., "QuantityInStock - sum"
                var aggValue = aggEntry.Value;

                try
                {
                    // Extract numeric value from different aggregation types
                    double? value = null;

                    if (aggValue is SumAggregate sumAgg)
                        value = sumAgg.Value;
                    else if (aggValue is AverageAggregate avgAgg)
                        value = avgAgg.Value;
                    else if (aggValue is MaxAggregate maxAgg)
                        value = maxAgg.Value;
                    else if (aggValue is MinAggregate minAgg)
                        value = minAgg.Value;
                    else if (aggValue is ValueCountAggregate countAgg)
                        value = countAgg.Value;
                    else if (aggValue is CardinalityAggregate cardAgg)
                        value = cardAgg.Value;

                    if (value.HasValue)
                    {
                        aggregates[aggName] = value.Value;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Error extracting aggregation '{aggName}': {ex.Message}");
                }
            }

            return aggregates;
        }


        private static QueryDescriptor<InventoryStock> BuildEsQueryFromDm(QueryDescriptor<InventoryStock> queryDescriptor, DataManagerRequest dm)
        {
            var must = new List<Query>();     // AND
            var should = new List<Query>();   // OR (global search or user ORs)
            var mustNot = new List<Query>();  // NOT

            if (dm.Where != null && dm.Where.Count > 0)
            {
                // dm.Where can be complex (nested AND/OR). Build recursively.
                var whereQuery = BuildWhereQuery(dm.Where);
                if (whereQuery is not null)
                    must.Add(whereQuery);
            }

            if (dm.Search != null && dm.Search.Count > 0)
            {
                foreach (var s in dm.Search)
                {
                    if (string.IsNullOrWhiteSpace(s.Key) || s.Fields == null || s.Fields.Count == 0)
                        continue;

                    // For each field, determine type and build appropriate query
                    foreach (var field in s.Fields)
                    {
                        Query? fieldQuery = null;

                        if (IsTextField(field))
                        {
                            // Text field: use "contains" operator (generates WildcardQuery)
                            fieldQuery = BuildLeafQuery(field, "contains", s.Key, s.IgnoreCase);
                        }
                        else if (IsNumericField(field))
                        {
                            // Numeric field: try to parse search value as number
                            var parsedValue = TryParseNumeric(s.Key);
                            if (parsedValue != null)
                            {
                                fieldQuery = BuildLeafQuery(field, "equal", parsedValue, s.IgnoreCase);
                            }
                            // If parsing fails, skip this field (don't add to should clause)
                        }
                        else if (IsDateField(field))
                        {
                            // Date field: try to parse search value as date
                            var parsedDate = TryParseDate(s.Key);
                            if (parsedDate.HasValue)
                            {
                                fieldQuery = BuildLeafQuery(field, "equal", parsedDate.Value, s.IgnoreCase);
                            }
                        }

                        if (fieldQuery != null)
                            should.Add(fieldQuery);
                    }
                }
            }

            var bq = new BoolQuery
            {
                Must = must,
                Should = should.Count > 0 ? should : null,
                MinimumShouldMatch = should.Count > 0 ? 1 : null
            };
            queryDescriptor.Bool(bq);
            return queryDescriptor;
        }

        private static Query? BuildWhereQuery(List<WhereFilter> nodes)
        {
            if (nodes == null || nodes.Count == 0)
                return null;

            // If the top-level list contains multiple independent filters,
            // Syncfusion treats them as AND by default.
            var qList = new List<Query>();
            foreach (var n in nodes)
            {
                var q = BuildWhereNode(n);
                if (q is not null) qList.Add(q);
            }
            if (qList.Count == 0) return null;

            return new BoolQuery { Must = qList }; // AND
        }

        private static Query? BuildWhereNode(WhereFilter node)
        {
            if (node == null) return null;

            if (node.IsComplex && node.predicates != null && node.predicates.Count > 0)
            {
                // Group: combine child predicates using node.Condition ("and"/"or")
                var children = node.predicates
                    .Select(BuildWhereNode)
                    .Where(q => q is not null)
                    .Cast<Query>()
                    .ToList();

                if (children.Count == 0) return null;

                var isOr = string.Equals(node.Condition, "or", StringComparison.OrdinalIgnoreCase);
                return isOr
                    ? new BoolQuery { Should = children, MinimumShouldMatch = 1 }
                    : new BoolQuery { Must = children };
            }

            // Leaf: Field / Operator / value
            if (string.IsNullOrWhiteSpace(node.Field))
                return null;

            return BuildLeafQuery(node.Field, node.Operator, node.value, node.IgnoreCase);
        }

        private static Query BuildLeafQuery(string field, string? op, object? value, bool ignoreCase)
        {
            // Map field to correct ES field for exact/equality operations
            var esField = MapToKeywordOrSelf(field); // strings -> .keyword, numbers/dates -> self

            // Handle array values (e.g., "in" operator)
            if (value is IEnumerable<object> list && op?.Equals("in", StringComparison.OrdinalIgnoreCase) == true)
            {
                return new TermsQuery
                {
                    Field = esField,
                    Terms = new TermsQueryField(list.Select(v => (FieldValue)ToFieldValue(v)).ToList())
                };
            }

            // Normalize scalar value
            var fv = ToFieldValue(value);

            // Branch by operator
            switch ((op ?? "equal").ToLowerInvariant())
            {
                case "equal":
                    return new TermQuery { Field = esField, Value = fv };

                case "notequal":
                    return new BoolQuery { MustNot = new List<Query> { new TermQuery { Field = esField, Value = fv } } };

                case "contains":
                    return new WildcardQuery
                    {
                        Field = esField,
                        Value = $"*{EscapeForWildcard(value?.ToString())}*",
                        CaseInsensitive = true
                    };

                case "startswith":
                    // prefix query is faster than wildcard "value*"
                    return new PrefixQuery
                    {
                        Field = esField,
                        Value = value?.ToString() ?? string.Empty,
                        CaseInsensitive = true
                    };

                case "endswith":
                    return new WildcardQuery
                    {
                        Field = esField,
                        Value = $"*{EscapeForWildcard(value?.ToString())}",
                        CaseInsensitive = true
                    };

                case "greaterthan":
                    return new UntypedRangeQuery { Field = esField, Gt = fv };

                case "greaterthanorequal":
                    return new UntypedRangeQuery { Field = esField, Gte = fv };

                case "lessthan":
                    return new UntypedRangeQuery { Field = esField, Lt = fv };

                case "lessthanorequal":
                    return new UntypedRangeQuery { Field = esField, Lte = fv };

                default:
                    // Fallback: try a match on text, else term
                    return new TermQuery { Field = esField, Value = fv };
            }
        }

        private static FieldValue ToFieldValue(object? value)
        {
            // Convert to FieldValue with basic types
            return value switch
            {
                null => FieldValue.Null,
                int i => i,
                long l => l,
                double d => d,
                float f => f,
                decimal m => (double)m,
                bool b => b,
                //DateTime dt => dt,
                DateTime dt => dt.Kind == DateTimeKind.Unspecified
                            ? dt.ToUniversalTime().ToString("o") // normalize if needed
                            : dt.ToString("o"),
                string s => s,
                _ => value.ToString() ?? string.Empty
            };
        }

        private static string EscapeForWildcard(string? s)
        {
            // Escape special wildcard chars that could affect patterns
            // \, /, ?, *, [, ]
            if (string.IsNullOrEmpty(s)) return string.Empty;
            return s.Replace("\\", "\\\\")
                    .Replace("/", "\\/")
                    .Replace("?", "\\?")
                    .Replace("*", "\\*")
                    .Replace("[", "\\[")
                    .Replace("]", "\\]");
        }

        private static List<SortOptions> BuildEsSorts(List<Sort>? sorted)
        {
            var sorts = new List<SortOptions>();
            if (sorted == null || sorted.Count == 0)
            {
                sorts.Add(new SortOptions
                {
                    Field = new FieldSort
                    {
                        Field = "itemId",
                        Order = SortOrder.Asc,
                    }
                });
                return sorts;
            }

            for (int i = sorted.Count() - 1; i >= 0; i--)
            {
                var field = string.IsNullOrWhiteSpace(sorted[i]?.Name) ? "itemId" : sorted[i]!.Name!;
                var esField = MapToKeywordOrSelf(field); // e.g., itemName -> itemName.keyword
                var order = (sorted[i]?.Direction?.Equals("Descending", StringComparison.OrdinalIgnoreCase) ?? false)
                    ? SortOrder.Desc : SortOrder.Asc;

                sorts.Add(new SortOptions
                {
                    Field = new FieldSort
                    {
                        Field = esField,
                        Order = order,
                    }
                });
            }

            return sorts;
        }

        private static readonly HashSet<string> NumericOrDate = new(StringComparer.OrdinalIgnoreCase)
        {
            "itemId", "unitPrice", "quantityInStock", "reorderLevel", "reorderQuantity", "lastRestocked"
        };

        private static readonly HashSet<string> DateFields = new(StringComparer.OrdinalIgnoreCase)
        {
            "lastRestocked"
        };

        private static string MapToKeywordOrSelf(string field)
        {
            var jsonPropertyName = GetJsonPropertyName(field);
            if (NumericOrDate.Contains(jsonPropertyName)) return jsonPropertyName;
            return $"{jsonPropertyName}.keyword";
        }

        /// <summary>
        /// Determines if a field is a text field (should use wildcard/text queries).
        /// </summary>
        private static bool IsTextField(string field)
        {
            var jsonPropertyName = GetJsonPropertyName(field);
            if (jsonPropertyName == null) return false;
            return !NumericOrDate.Contains(jsonPropertyName);
        }

        /// <summary>
        /// Determines if a field is a numeric field (int, decimal, double, etc).
        /// </summary>
        private static bool IsNumericField(string field)
        {
            var jsonPropertyName = GetJsonPropertyName(field);
            if (jsonPropertyName == null) return false;
            return NumericOrDate.Contains(jsonPropertyName) && !DateFields.Contains(jsonPropertyName);
        }

        /// <summary>
        /// Determines if a field is a date field.
        /// </summary>
        private static bool IsDateField(string field)
        {
            var jsonPropertyName = GetJsonPropertyName(field);
            if (jsonPropertyName == null) return false;
            return DateFields.Contains(jsonPropertyName);
        }

        /// <summary>
        /// Attempts to parse a string value as a number (int, long, double, decimal).
        /// Returns the parsed value or null if parsing fails.
        /// </summary>
        private static object? TryParseNumeric(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            // Try parsing in order of specificity
            if (int.TryParse(value, out int intVal)) return intVal;
            if (long.TryParse(value, out long longVal)) return longVal;
            if (decimal.TryParse(value, out decimal decimalVal)) return decimalVal;
            if (double.TryParse(value, out double doubleVal)) return doubleVal;

            return null;
        }

        /// <summary>
        /// Attempts to parse a string value as a DateTime.
        /// Returns the parsed DateTime or null if parsing fails.
        /// </summary>
        private static DateTime? TryParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            if (DateTime.TryParse(value, out DateTime dateVal))
                return dateVal;

            return null;
        }

        /// <summary>
        /// Adds a new inventory item to both static list and ElasticSearch.
        /// </summary>
        /// <param name="item">The inventory item to add</param>
        public async Task AddInventoryItemAsync(InventoryStock item)
        {
            try
            {
                if (item == null)
                    throw new ArgumentNullException(nameof(item), "Inventory item cannot be null");

                if (string.IsNullOrWhiteSpace(item.SKU))
                    throw new ArgumentException("SKU cannot be empty", nameof(item.SKU));

                if (string.IsNullOrWhiteSpace(item.ItemName))
                    throw new ArgumentException("Item name cannot be empty", nameof(item.ItemName));

                // Generate itemId if not provided (itemId is 0 or invalid)
                if (item.ItemId <= 0)
                {
                    item.ItemId = await GetNextItemIdAsync();
                }

                var response = await _elasticClient.IndexAsync<InventoryStock>(
                    item,
                    i => i.Index(IndexName).Id(item.ItemId.ToString())
                );

                if (!response.IsValidResponse)
                {
                    Console.WriteLine($"ElasticSearch error: {response.ApiCallDetails?.HttpStatusCode}");
                    throw new Exception($"Failed to add inventory item to ES: {response.ApiCallDetails?.DebugInformation}");
                }

                // Force ElasticSearch index refresh to make document immediately searchable
                await RefreshIndexAsync();

                Console.WriteLine($"✓ Item {item.ItemId} added to static list and ElasticSearch");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding inventory item: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Updates an existing inventory item in both static list and ElasticSearch.
        /// </summary>
        /// <param name="item">The inventory item with updated values</param>
        public async Task UpdateInventoryItemAsync(InventoryStock item)
        {
            try
            {
                if (item == null)
                    throw new ArgumentNullException(nameof(item), "Inventory item cannot be null");

                if (item.ItemId <= 0)
                    throw new ArgumentException("Item ID must be valid", nameof(item.ItemId));

                var response = await _elasticClient.IndexAsync<InventoryStock>(
                    item,
                    i => i.Index(IndexName).Id(item.ItemId.ToString())
                );

                if (!response.IsValidResponse)
                {
                    Console.WriteLine($"ElasticSearch error: {response.ApiCallDetails?.HttpStatusCode}");
                    throw new Exception($"Failed to update inventory item in ES: {response.ApiCallDetails?.DebugInformation}");
                }

                // Force ElasticSearch index refresh to make document immediately searchable
                await RefreshIndexAsync();

                Console.WriteLine($"✓ Item {item.ItemId} updated in static list and ElasticSearch");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating inventory item: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes an inventory item from both static list and ElasticSearch.
        /// </summary>
        /// <param name="itemId">The inventory item ID to delete</param>
        public async Task RemoveInventoryItemAsync(int? itemId)
        {
            try
            {
                if (itemId == null || itemId <= 0)
                    throw new ArgumentException("Item ID cannot be null or invalid", nameof(itemId));

                var response = await _elasticClient.DeleteAsync(IndexName, itemId.ToString());

                if (!response.IsValidResponse && response.ApiCallDetails?.HttpStatusCode != 404)
                {
                    Console.WriteLine($"ElasticSearch error: {response.ApiCallDetails?.HttpStatusCode}");
                    throw new Exception($"Failed to delete inventory item from ES: {response.ApiCallDetails?.DebugInformation}");
                }

                // Force ElasticSearch index refresh to make deletion immediately visible
                await RefreshIndexAsync();

                Console.WriteLine($"✓ Item {itemId} deleted from static list and ElasticSearch");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting inventory item: {ex.Message}");
                throw;
            }
        }


        /// <summary>
        /// Generates the next itemId by finding the maximum itemId in ElasticSearch and incrementing it.
        /// If no items exist, returns 1.
        /// </summary>
        /// <returns>The next available itemId</returns>
        private async Task<int> GetNextItemIdAsync()
        {
            try
            {
                var response = await _elasticClient.SearchAsync<InventoryStock>(s => s
                    .Indices(IndexName)
                    .Size(0)
                    .Aggregations(aggregations => aggregations
                        .Add("max_item_id", aggregation => aggregation
                            .Max(m => m.Field("itemId"))
                        )
                    )
                );

                if (response.IsValidResponse && response.Aggregations != null)
                {
                    var maxAggregate = response.Aggregations.GetMax("max_item_id");
                    return Convert.ToInt32(maxAggregate?.Value) + 1;
                }

                // Default to 1 if no items exist or error occurs
                return 1001;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating itemId: {ex.Message}. Defaulting to 1");
                return 1001;
            }
        }

        /// <summary>
        /// Forces ElasticSearch to refresh the index, making newly added/updated/deleted documents immediately searchable.
        /// This ensures that CRUD operations are reflected immediately in Grid queries without delay.
        /// </summary>
        private async Task RefreshIndexAsync()
        {
            try
            {
                var response = await _elasticClient.Indices.RefreshAsync(IndexName);
                if (!response.IsValidResponse)
                {
                    Console.WriteLine($"⚠ Warning: Index refresh failed - {response.ApiCallDetails?.DebugInformation}");
                }
                else
                {
                    Console.WriteLine($"✓ Index '{IndexName}' refreshed successfully");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Warning: Error refreshing index: {ex.Message}");
                // Don't throw - refresh failure should not block the operation
            }
        }

        // Get JsonPropertyName for a specific property
        public static string? GetJsonPropertyName(string propertyName)
        {
            var property = typeof(InventoryStock).GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
                return null;

            var jsonPropertyAttr = property.GetCustomAttribute<JsonPropertyNameAttribute>();
            return jsonPropertyAttr?.Name ?? property.Name;
        }
    }
}