using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

class Client
{
    static void Main()
    {
        TcpClient client = new("127.0.0.1", 5001);
        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new(stream, Encoding.UTF8);
        using StreamWriter writer = new(stream, Encoding.UTF8) { AutoFlush = true };

        Console.WriteLine(reader.ReadLine());

        while (true)
        {
            Console.Write("Введите запчасть: ");
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) continue;

            writer.WriteLine(input);
            string? response = reader.ReadLine();
            Console.WriteLine(response);

            string? nextPrompt = reader.ReadLine();
            if (nextPrompt != null) Console.WriteLine(nextPrompt);

            if (input.ToLower() == "выход") break;
        }
    }
}
