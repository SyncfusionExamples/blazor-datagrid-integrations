# Blazor DataGrid with Elasticsearch and Custom Adaptor

## Project Overview

This repository demonstrates a production-ready pattern for binding **Elasticsearch** data to **Syncfusion Blazor DataGrid** using a **Custom Adaptor**. The sample application provides complete CRUD (Create, Read, Update, Delete) operations, filtering, sorting, paging, grouping, and batch updates. The implementation follows industry best practices using models, repository pattern, and a custom adaptor for seamless grid functionality with Elasticsearch as the search and storage backend.

## Key Features

- **Elasticsearch Integration**: Full-text search, indexing, and document storage capabilities with Elasticsearch
- **Syncfusion Blazor DataGrid**: Built-in search, filter, sort, paging, and grouping capabilities
- **Complete CRUD Operations**: Add, edit, delete, and batch update records directly from the grid
- **Repository Pattern**: Clean separation of concerns with dependency injection support
- **CustomAdaptor**: Full control over grid data operations (read, search, filter, sort, page, group)
- **In-Memory Data Sync**: Static in-memory data source with Elasticsearch indexing for optimized search performance
- **Configurable Elasticsearch Connection**: Elasticsearch URL and credentials managed via `appsettings.json`

## Prerequisites

| Component | Version | Purpose |
|-----------|---------|---------|
| Visual Studio 2026 | 18.0 or later | Development IDE with Blazor workload |
| .NET SDK | net10.0 or compatible | Runtime and build tools |
| Elasticsearch | 8.0 or later | Search and document storage engine |
| Elastic.Clients.Elasticsearch | 9.3.0 or later | Official Elasticsearch .NET client library |
| Syncfusion.Blazor.Grid | Latest | DataGrid and UI components |
| Syncfusion.Blazor.Themes | Latest | Styling for DataGrid components |

## Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd "Binding Elasticsearch using CustomAdaptor"
   cd "Grid_ElasticSearch"
   ```

2. **Ensure Elasticsearch is running**
   
   Start Elasticsearch on your local machine or remote server. By default, it runs on `http://localhost:9200`. You can verify it's running:
   ```bash
   curl http://localhost:9200
   ```

3. **Update the Elasticsearch configuration**
   
   Open `appsettings.json` and configure the Elasticsearch connection:
   ```json
   {
     "ElasticSearch": {
       "Url": "http://localhost:9200",
       "IndexName": "inventory-items",
       "Password": "<elasticsearch-password>"
     },
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning"
       }
     },
     "AllowedHosts": "*"
   }
   ```

4. **Restore packages and build**
   ```bash
   dotnet restore
   dotnet build
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Open the application**
   
   Navigate to the local URL displayed in the terminal (typically `https://localhost:7xxx`). The application will automatically create the Elasticsearch index on startup if it doesn't exist.

## Configuration

### Elasticsearch Configuration

The Elasticsearch settings in `appsettings.json` contain the following components:

| Component | Description | Example |
|-----------|-------------|---------|
| Url | Elasticsearch server URL | `http://localhost:9200` |
| IndexName | Name of the Elasticsearch index | `inventory-items` |
| Password | Elasticsearch password for authentication | `<elasticsearch-password>` |

**Security Note**: For production environments, store sensitive credentials using:
- User secrets for development
- Environment variables for production
- Azure Key Vault or similar secure storage solutions

## Project Layout

| File/Folder | Purpose |
|-------------|---------|
| `/Data/InventoryStock.cs` | Entity model representing inventory items in Elasticsearch index |
| `/Data/InventoryDataService.cs` | Service managing static in-memory inventory data with seed data initialization |
| `/Data/InventoryRepository.cs` | Repository class providing CRUD operations with Elasticsearch integration |
| `/Data/ElasticSearchInitializationService.cs` | Service for initializing Elasticsearch indexes on application startup |
| `/Components/Pages/Home.razor` | DataGrid page with CustomAdaptor implementation |
| `/Program.cs` | Service registration, Elasticsearch client configuration, and dependency injection setup |
| `/appsettings.json` | Application configuration including Elasticsearch URL, index name, and credentials |

## Common Tasks

### Add an Inventory Item
1. Click the **Add** button in the toolbar
2. Fill in the form fields (SKU, ItemName, Category, Supplier, UnitPrice, QuantityInStock, etc.)
3. Click **Save** to persist the record to Elasticsearch

### Edit an Inventory Item
1. Select a row in the grid
2. Click the **Edit** button in the toolbar
3. Modify the required fields
4. Click **Update** to save changes to Elasticsearch

### Delete an Inventory Item
1. Select a row in the grid
2. Click the **Delete** button in the toolbar
3. Confirm the deletion in the dialog

### Search Records
1. Use the **Search** box in the toolbar
2. Enter keywords to filter records (searches across all fields in Elasticsearch index)

### Filter Records
1. Click the filter icon in any column header
2. Select filter criteria (equals, contains, greater than, etc.)
3. Click **Filter** to apply

### Sort Records
1. Click the column header to sort ascending
2. Click again to sort descending

## Troubleshooting

### Elasticsearch Connection Error
- Verify Elasticsearch is running on the specified URL (typically `http://localhost:9200`)
- Confirm the Elasticsearch URL and password in `appsettings.json` are correct
- Test connectivity using `curl http://localhost:9200`

### Index Not Created
- Verify `ElasticSearchInitializationService` is properly registered in `Program.cs`
- Check application logs for initialization errors
- Ensure the Elasticsearch cluster is accessible and healthy

### Static Files Not Loading
- Verify Syncfusion stylesheet and script references are present in `App.razor`
- Check browser developer tools for 404 errors on static resources

### Version Conflicts
- Align Elastic.Clients.Elasticsearch and Syncfusion package versions
- Run `dotnet restore` to update NuGet packages
- Check the `.csproj` file for conflicting version constraints

### Grid Data Not Appearing
- Verify seed data is being initialized in `InventoryDataService`
- Check that the Elasticsearch index has been populated with documents
- Review the CustomAdaptor implementation in the Razor component

## Full Documentation

For more information about Syncfusion Blazor DataGrid and CustomAdaptor implementation, refer to the [Syncfusion documentation](https://blazor.syncfusion.com/documentation/datagrid/connecting-to-backends/elasticsearch).

For Elasticsearch client library details, see the [Elastic.Clients.Elasticsearch documentation](https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/index.html).
