# ABC Money Transfer

ABC Money Transfer is a web application built using ASP.NET Core 8 MVC, designed to facilitate user registration, login, and money transfer operations. The application integrates with external APIs for exchange rates and securely manages user transactions.

## Features

- **User Registration and Login:** Secure user authentication with registration and login functionalities.
- **Money Transfer:** Allows users to transfer money from Malaysia to Nepal with real-time exchange rates.
- **Transaction Reports:** Generate and view transaction reports based on date ranges.
- **API Integration:** Integrates with external APIs to fetch real-time exchange rates.

### Prerequisites

Before proceeding with the setup, ensure that you have the following prerequisites installed on your machine:

- .NET Core 8 SDK
- SQL Server (or SQL Server Express)

### Clone/Download the Repository

Run the following command to clone the repository:

```terminal
    git clone https://github.com/ishwors/MoneyTransfer.git
```

### Configure & Run the Application

1. navigate to the cloned repository's MoneyTransfer.Web.
2. create the appsettings.json file.
3. setup database
4. run project

#### Sample appsettings.json for BE

```json
{
    "ConnectionStrings": {
        "DefaultConnection": "Server=<server-name>;Database=<database-name>;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
    },

    "Jwt": {
        "Key": "jOApC3nNlJ8XzPBwV+Mx9/saE0mWu0/NvSdsQzJ5VoM=",
        "Issuer": "ABCMoneyTransfer",
        "Audience": "ABCMoneyTransferUsers"
    },

    "Hash": {
        "PasswordHashSecret": "#32!xAz)27:zXa@3" //min size 16 from 16 bytes
    },

    "ExternalApi": {
        "ExchangeRateNRB": "https://www.nrb.org.np/api/forex/v1/rates"
    },

    "AllowedHosts": "*",

    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    }
}

```

### Database Setup

1. Open a command prompt or terminal.
2. Navigate to the cloned repository's root directory.
3. Run the following command to apply migrations and create the database schema:

    ```terminal
        dotnet ef -p MoneyTransfer.Data -s MoneyTransfer.Web database update
    ```

### Running the Application

1. Open a command prompt or terminal.
2. Navigate to the cloned repository's root directory.
3. Run the following command to start the Aloi backend application:

    ```terminal
    dotnet run --project MoneyTransfer.Web
    ```

The application will start running on <https://localhost:7288/>
