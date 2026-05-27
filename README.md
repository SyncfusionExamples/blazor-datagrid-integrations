# Blazor DataGrid Integrations

A comprehensive guide demonstrating how to integrate and use the **[Blazor DataGrid](https://www.syncfusion.com/blazor-components/blazor-datagrid)** component with multiple backend services. This repository contains production-ready sample applications showing complete CRUD operations, real-time data synchronization, and advanced data management patterns.

## Overview

This project showcases two distinct integration patterns for the Blazor DataGrid using custom adaptors:

- **Elasticsearch Integration**: Full-text search with document indexing and distributed querying capabilities
- **SignalR Real-Time Updates**: Live data synchronization with server-pushed updates for real-time dashboards

Each sample demonstrates best practices including dependency injection, repository patterns, custom data adaptors, and proper error handling.

## Features

Both samples share the following capabilities across their respective backends:

- **Complete CRUD Operations**: Add, edit, delete, and retrieve records directly from the grid
- **Advanced Filtering & Sorting**: Multi-column filtering, custom sort orders, and range queries
- **Server-Side Paging**: Efficient handling of large datasets with configurable page sizes
- **Custom Data Adaptors**: Full control over grid data operations using DataAdaptor pattern
- **Search Functionality**: Query data across multiple fields with flexible search capabilities
- **Dependency Injection**: Clean service architecture with DI container integration
- **Repository Pattern**: Separation of concerns between data access and business logic
- **Error Handling**: Comprehensive exception management and validation
- **Configuration Management**: Environment-based settings via `appsettings.json`
  
## Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or later
- [Visual Studio Code](https://code.visualstudio.com/)

## Getting Started

### Clone the repository

```bash
git clone https://github.com/SyncfusionExamples/blazor-datagrid-integrations.git
cd blazor-datagrid-integrations
```

### Run with Visual Studio

1. Open the solution file using Visual Studio 2022 or later.
2. Restore the NuGet packages by rebuilding the solution.
3. Build the project to ensure there are no compilation errors.
4. Run the project.

### Run with .NET CLI

```bash
# Restore dependencies
dotnet restore

# Run the project
dotnet run
```
## References

- [Blazor Documentation](https://blazor.syncfusion.com/documentation/introduction)
- [Blazor DataGrid Documentation](https://blazor.syncfusion.com/documentation/datagrid/)
- [Online DataGrid Integration Demo](https://blazor.syncfusion.com/demos/query-builder/grid?theme=fluent2)




