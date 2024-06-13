using System.Net;
using System.Net.Sockets;
using codecrafters_http_server;

class Program
{
    public static readonly string DefaultDataDirPath = "/tmp/data/codecrafters.io/http-server-tester";

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