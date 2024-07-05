using System;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Program
{
    private static ITelegramBotClient botClient;
    private const string ConnectionString = "Data Source=C:\\Users\\fgfed\\Desktop\\TelegramBot\\TelegramBot\\db\\zonaDeportiva.db";

    static void Main(string[] args)
    {
        botClient = new TelegramBotClient("7249966602:AAGpOBW3iHrz6x7BqnonSg7jtfaYkDnwRAE");

        using var cts = new CancellationTokenSource();

        ReceiverOptions receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>() 
        };
        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = botClient.GetMeAsync().Result;
        Console.WriteLine($"Start listening for @{me.Username}");
        Console.ReadLine();

        
        cts.Cancel();
    }

    private static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message || update.Message!.Type != MessageType.Text)
            return;

        var chatId = update.Message.Chat.Id;
        var messageText = update.Message.Text;

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

        if (messageText.ToLower() == "/start")
        {
            string welcomeMessage = "👋 ¡Hola! Soy el asistente virtual de Zona Deportiva, la tienda donde encontrarás un diverso catálogo de indumentaria deportiva masculina.\n\n" +
                                    "Podrás consultar:\n" +
                                    "📦 Catálogo de productos\n" +
                                    "💳 Opciones de pago\n" +
                                    "📍 Sucursales\n" +
                                    "📅 Reservar un producto\n\n" +
                                    "Por favor, selecciona una opción del menú.";
            var replyKeyboardMarkup = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "Catálogo de productos 📦", "Opciones de pago 💳" },
                new KeyboardButton[] { "Reservar un producto 📅", "Sucursales 📍" },
                new KeyboardButton[] { "Despedirse 👋" }
            })
            {
                ResizeKeyboard = true
            };

            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: welcomeMessage,
                replyMarkup: replyKeyboardMarkup,
                cancellationToken: cancellationToken
            );
        }
        else if (messageText.ToLower() == "catálogo de productos 📦")
        {
            string catalogo = ObtenerCatalogoProductos();
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: catalogo,
                cancellationToken: cancellationToken
            );
        }
        else if (messageText.ToLower() == "opciones de pago 💳")
        {
            string opcionesPago = "Aceptamos los siguientes métodos de pago:\n\n" +
                                  "1. Tarjeta de crédito\n" +
                                  "2. Tarjeta de débito\n" +
                                  "3. Efectivo";
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: opcionesPago,
                cancellationToken: cancellationToken
            );
        }
        else if (messageText.ToLower() == "reservar un producto 📅")
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Por favor, proporciona tu nombre, email y el ID del producto a reservar, separados por comas. Ejemplo:\nJuan Pérez, juan@example.com, 1",
                cancellationToken: cancellationToken
            );
        }
        else if (messageText.Contains(","))
        {
            string[] datos = messageText.Split(',');
            if (datos.Length == 3)
            {
                string nombre = datos[0].Trim();
                string email = datos[1].Trim();
                if (int.TryParse(datos[2].Trim(), out int productoId))
                {
                    string resultadoReserva = ReservarProducto(nombre, email, productoId);
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: resultadoReserva,
                        cancellationToken: cancellationToken
                    );
                }
                else
                {
                    await botClient.SendTextMessageAsync(
                        chatId: chatId,
                        text: "ID de producto inválido. Por favor, intenta nuevamente.",
                        cancellationToken: cancellationToken
                    );
                }
            }
        }
        else if (messageText.ToLower() == "sucursales 📍")
        {
            string sucursales = "Nuestras sucursales están ubicadas en:\n- Av. Siempre Viva 123\n- Calle Falsa 456\n- Plaza Principal 789";
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: sucursales,
                cancellationToken: cancellationToken
            );
        }
        else if (messageText.ToLower() == "despedirse 👋")
        {
            string despedida = "¡Gracias por visitarnos! Si necesitas más ayuda, no dudes en escribirnos.";
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: despedida,
                cancellationToken: cancellationToken
            );
        }
        else
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Lo siento, no entiendo esa opción. Por favor, elige una opción del menú.",
                cancellationToken: cancellationToken
            );
        }
    }

    private static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

    private static string ObtenerCatalogoProductos()
    {
        string catalogo = "Catálogo de productos:\n";
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string query = "SELECT Id, Nombre, Precio FROM Productos";
            using (var command = new SQLiteCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        catalogo += $"{reader["Id"]}. {reader["Nombre"]} - ${reader["Precio"]}\n";
                    }
                }
            }
        }
        return catalogo;
    }

    private static string ReservarProducto(string nombre, string email, int productoId)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string query = "INSERT INTO Reserva (ClienteNombre, ClienteEmail, ProductoId, FechaReserva) VALUES (@nombre, @correo, @productoId, @fecha)";
            using (var command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@nombre", nombre);
                command.Parameters.AddWithValue("@correo", email);
                command.Parameters.AddWithValue("@productoId", productoId);
                command.Parameters.AddWithValue("@fecha", DateTime.Now);

                try
                {
                    command.ExecuteNonQuery();
                    return "Reserva realizada con éxito! Retirar y pagar por cualquiera de nuestras sucursales.";
                }
                catch (Exception ex)
                {
                    return $"Error al realizar la reserva: {ex.Message}";
                }
            }
        }
    }
}
