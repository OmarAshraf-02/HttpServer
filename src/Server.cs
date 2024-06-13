using System.Net;
using System.Net.Sockets;
using System.Reflection;
using codecrafters_http_server;

class Program
{
    private static async Task Main(string[] args)
    {
        TcpListener server = new(IPAddress.Any, Constants.Port);
        server.Start();

        Console.WriteLine($"Server started on port {Constants.Port}");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            await Task.Run(() => Handlers.HandleRequest(client));
        }
    }
}