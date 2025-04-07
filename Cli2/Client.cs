// Client.cs
using System;
using System.Net.Sockets;
using System.Text;
using System.IO;

class CurrencyClient
{
    private const string serverIP = "127.0.0.1";
    private const int port = 9001;

    public void Start()
    {
        using TcpClient client = new TcpClient(serverIP, port);
        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new(stream, Encoding.UTF8);
        using StreamWriter writer = new(stream, Encoding.UTF8) { AutoFlush = true };

        Console.WriteLine("=== Клиент: Курс валют ===");
        Console.WriteLine("Введите пару валют через пробел (например: USD EURO), или 'exit' для выхода.");

        while (true)
        {
            Console.Write(">> ");
            string? input = Console.ReadLine()?.Trim();

            if (input == null || input.ToLower() == "exit") break;

            writer.WriteLine(input);
            string response = reader.ReadLine() ?? "Нет ответа от сервера.";
            Console.WriteLine("Ответ: " + response);
        }
    }
}

// Program.cs
class Program
{
    static void Main()
    {
        CurrencyClient client = new CurrencyClient();
        client.Start();
    }
}
