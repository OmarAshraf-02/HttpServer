using System.Text;

namespace codecrafters_http_server;

public static class Constants
{
    public const int Port = 4221;
    public static readonly byte[] NotFoundResponse = Encoding.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\n\r\n");
    public static readonly byte[] SuccessResponse = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n\r\n");
    public static readonly byte[] CreatedResponse = Encoding.UTF8.GetBytes("HTTP/1.1 201 Created\r\n\r\n");
    public static readonly string DefaultDataDirPath = "/tmp/data/codecrafters.io/http-server-tester";
}
