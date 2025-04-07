// Server.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class CurrencyServer
{
    private static readonly Dictionary<string, double> currencyRates = new()
    {
        {"USD", 1.0},
        {"EURO", 0.92},
        {"GBP", 0.78},
        {"JPY", 146.2},
        {"CNY", 7.2}
    };

    private const int port = 9001;
    private static readonly string logFile = "server_log.txt";

    static void Main()
    {
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Сервер запущен на порту {port}.");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Thread thread = new Thread(HandleClient);
            thread.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;
        IPEndPoint endPoint = (IPEndPoint)client.Client.RemoteEndPoint;
        string clientInfo = $"[{DateTime.Now}] Подключение от {endPoint}";
        Log(clientInfo);
        Console.WriteLine(clientInfo);

        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new(stream, Encoding.UTF8);
        using StreamWriter writer = new(stream, Encoding.UTF8) { AutoFlush = true };

        try
        {
            while (true)
            {
                string? request = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(request)) break;

                string[] parts = request.ToUpper().Split(' ');
                if (parts.Length != 2 || !currencyRates.ContainsKey(parts[0]) || !currencyRates.ContainsKey(parts[1]))
                {
                    writer.WriteLine("Неверный запрос.");
                    continue;
                }

                double fromRate = currencyRates[parts[0]];
                double toRate = currencyRates[parts[1]];
                double result = toRate / fromRate;

                string response = $"1 {parts[0]} = {result:F4} {parts[1]}";
                writer.WriteLine(response);

                Log($"Запрос от {endPoint}: {parts[0]} {parts[1]} → {response}");
            }
        }
        catch (Exception ex)
        {
            Log($"Ошибка при обработке клиента {endPoint}: {ex.Message}");
        }
        finally
        {
            client.Close();
            Log($"Отключение клиента {endPoint} в {DateTime.Now}\n");
        }
    }

    static void Log(string message)
    {
        File.AppendAllText(logFile, message + Environment.NewLine);
    }
}
