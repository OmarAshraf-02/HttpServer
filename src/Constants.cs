namespace codecrafters_http_server;

public static class Constants
{
    public const int Port = 4221;
    public static readonly string NotFoundResponse = "HTTP/1.1 404 Not Found\r\n\r\n";
    public static readonly string SuccessResponse = "HTTP/1.1 200 OK\r\n\r\n";
    public static readonly string CreatedResponse = "HTTP/1.1 201 Created\r\n\r\n";
    public static readonly string DefaultDataDirPath = "/tmp/data/codecrafters.io/http-server-tester";
}
