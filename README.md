# EBookStore - ASP.NET Core MVC E-Commerce Application

EBookStore is a modern e-commerce web application built with ASP.NET Core MVC, designed for selling and managing books. The application features a clean architecture, role-based authorization, and a responsive user interface.

## Features

- 🔐 **User Authentication & Authorization**
  - Role-based access control (Admin, User)
  - Email confirmation
  - Password recovery

- 📚 **Book Management**
  - Book catalog with pagination
  - Genre-based categorization
  - Stock management
  - Book details with images

- 🛒 **Shopping Features**
  - Shopping cart functionality
  - Order management
  - Like/Wishlist system

- 👤 **User Features**
  - Profile management
  - Order history
  - Email notifications

- 🔧 **Admin Features**
  - Book inventory management
  - User management
  - Order processing
  - Genre management

## Prerequisites

- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [Visual Studio Code](https://code.visualstudio.com/)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Local or Express)

## Setup Instructions

### Using Visual Studio 2022

1. **Clone the Repository**
   ```bash
   git clone https://github.com/yourusername/EBookStore.git
   ```

2. **Open the Solution**
   - Open `BookShopingCartMVC.sln` in Visual Studio 2022
   - Wait for all dependencies to restore

3. **Configure Database**
   - Open `appsettings.json`
   - Update the `DefaultConnection` string with your SQL Server details
   - In Package Manager Console, run:
     ```bash
     Update-Database
     ```

4. **Run the Application**
   - Press F5 or click the Run button
   - The application will launch in your default browser

### Using Visual Studio Code

1. **Clone the Repository**
   ```bash
   git clone https://github.com/yourusername/EBookStore.git
   cd EBookStore
   ```

2. **Install Required Extensions**
   - C# for Visual Studio Code
   - C# Dev Kit
   - IntelliCode

3. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

4. **Configure Database**
   - Update connection string in `appsettings.json`
   - Run migrations:
     ```bash
     dotnet ef database update
     ```

5. **Run the Application**
   ```bash
   dotnet run
   ```

## Project Structure

- `Areas/`: Contains area-specific MVC components
  - `Admin/`: Admin panel features
  - `User/`: User-specific features
  - `Guest/`: Public access features
  - `Identity/`: Authentication pages

- `Models/`: Domain models and view models
- `Data/`: Database context and migrations
- `Repository/`: Data access layer
- `Services/`: Business logic layer
- `wwwroot/`: Static files (CSS, JS, images)

## Default Credentials

**Admin Account:**
- Email: admin@gmail.com
- Password: Admin@123

## Additional Configuration

### Email Settings
Update `appsettings.json` with your SMTP settings:
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password"
  }
}
```

## Development Guidelines

1. **Database Changes**
   - Create new migration: `dotnet ef migrations add MigrationName`
   - Update database: `dotnet ef database update`

2. **Code Style**
   - Follow C# coding conventions
   - Use async/await for database operations
   - Implement repository pattern
   - Use dependency injection

3. **Security**
   - Never commit sensitive data
   - Use user secrets for development
   - Implement proper authorization

## License

This project is licensed under the MIT License - see the LICENSE file for details.
