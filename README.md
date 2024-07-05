# Zona Deportiva Telegram Bot

## Description

This is a Telegram bot developed in C# for **Zona Deportiva**, a sportswear store. The bot allows users to interact with the store in the following ways:

- View the product catalog.
- List available branches.
- See accepted payment methods.
- Reserve products.

## Requirements

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [SQLite](https://www.sqlite.org/index.html)
- [Telegram Bot API Token](https://core.telegram.org/bots#3-how-do-i-create-a-bot)

## Setup

1. Clone this repository to your local machine:
    ```bash
    git clone https://github.com/your-username/zona-deportiva-bot.git
    ```

2. Navigate to the project directory:
    ```bash
    cd zona-deportiva-bot
    ```

3. Restore the required NuGet packages:
    ```bash
    dotnet restore
    ```

4. Configure the SQLite database connection string in the code (`Program.cs`):
    ```csharp
    private const string ConnectionString = "Data Source=C:\\Users\\fgfed\\Desktop\\TelegramBot\\TelegramBot\\db\\zonaDeportiva.db";
    ```

5. Configure the Telegram bot token in the code (`Program.cs`):
    ```csharp
    botClient = new TelegramBotClient("YOUR_TELEGRAM_BOT_API_TOKEN");
    ```

## Usage

1. Run the bot:
    ```bash
    dotnet run
    ```

2. Start a conversation with your bot on Telegram by sending the `/start` command.

3. Use the following menu options to interact with the bot:
    - **Product Catalog 📦**: View the available product catalog.
    - **Payment Options 💳**: Show accepted payment methods.
    - **Reserve a Product 📅**: Reserve a product by providing name, email, and product ID.
    - **Branches 📍**: List the store's branches.
    - **Say Goodbye 👋**: End the conversation with the bot.

## Code Structure

- `Main`: Configures and runs the bot.
- `HandleUpdateAsync`: Handles incoming updates from the bot.
- `HandlePollingErrorAsync`: Handles polling errors.
- `ObtenerCatalogoProductos`: Fetches and returns the product catalog from the database.
- `ReservarProducto`: Registers a reservation in the database.

## Demo

https://www.loom.com/share/29b49bec5a714525b76478a8aeea9d2e?sid=8545a9b8-26ed-4638-b22e-16c91114a33b



