# Sales Order Application

A .NET Core Sales Order Application that manages customers, orders, and order items.

## Prerequisites

Make sure you have the following installed on your machine:

- [.NET Core SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Entity Framework Core Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)

## Getting Started

Follow the instructions below to set up and run the application on your local machine.

### 1. Clone the Repository

First, clone the repository using Git:

```bash
git clone <repository-url>
cd <repository-directory>
```

Replace <repository-url> with the URL of your repository and <repository-directory> with the directory name where the project is located.

### 2. Configure the Database Connection
Open the appsettings.json file located in the project root, and update the connection string under the ConnectionStrings section to point to your SQL Server instance.

Example connection string configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=SalesOrderDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

Replace YOUR_SERVER_NAME with your SQL Server instance name.
Ensure the database name is set to SalesOrderDB.
Make sure the database you specify exists, or it will be automatically created during migration.

### 3. Apply Migrations
Once the connection string is properly configured, you can apply the database migrations to create the required tables.

Run the following command in the terminal to create the database and apply migrations:

```bash
dotnet ef database update
```

This command will apply all migrations and create the necessary database schema in your SQL Server instance.

### 4. Run the Application
Now that the database is set up, you can run the application by using the following command:

```bash
dotnet run
```

The application should now be up and running, and you can access it via the browser
