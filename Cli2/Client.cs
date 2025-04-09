using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    private static TcpListener listener;
    private static Dictionary<string, decimal> partsPriceList = new()
    {
        { "процессор", 25000 },
        { "видеокарта", 50000 },
        { "материнская плата", 15000 },
        { "оперативная память", 8000 },
        { "жесткий диск", 6000 },
        { "ssd", 10000 },
        { "блок питания", 7000 },
        { "корпус", 4000 }
    };

    static void Main()
    {
        listener = new TcpListener(IPAddress.Any, 5001);
        listener.Start();
        Console.WriteLine("Сервер запущен и ожидает подключения клиентов...");

        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Thread thread = new(() => HandleClient(client));
            thread.Start();
        }
    }

    static void HandleClient(TcpClient client)
    {
        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new(stream, Encoding.UTF8);
        using StreamWriter writer = new(stream, Encoding.UTF8) { AutoFlush = true };

        writer.WriteLine("Добро пожаловать в магазин комплектующих. Введите название запчасти:");

        while (true)
        {
            string? partName = reader.ReadLine();
            if (partName == null || partName.ToLower() == "выход") break;

            if (partsPriceList.TryGetValue(partName.ToLower(), out decimal price))
            {
                writer.WriteLine($"Цена на '{partName}': {price} руб.");
            }
            else
            {
                writer.WriteLine("Запчасть не найдена. Попробуйте снова.");
            }

            writer.WriteLine("Введите новую запчасть или напишите 'выход' для завершения:");
        }

        client.Close();
    }
}
