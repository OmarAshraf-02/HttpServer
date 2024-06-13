using System.Net.Sockets;
using System.Text;

namespace codecrafters_http_server;

internal static class Handlers
{
    internal static async Task<Task> HandleRequest(TcpClient tcpClient)
    {
        NetworkStream stream = tcpClient.GetStream();

        byte[] reqBuffer = new byte[1024];
        int bytesRead = await stream.ReadAsync(reqBuffer);
        string reqData = Encoding.UTF8.GetString(reqBuffer, 0, bytesRead);

        string[] lines = reqData.Split("\r\n");
        string requestLine = lines[0];
        string body = lines[lines.Length - 1];

        string[] splitRequestLine = requestLine.Split(" ");
        string httpVerb = splitRequestLine[0];
        string path = splitRequestLine[1];
        Dictionary<string, string> headers = Helpers.ParseHttpHeaders(lines);

        byte[] responseBytes = httpVerb switch
        {
            "GET" => HandleGet(path, new StringBuilder(), lines, splitRequestLine, headers),
            "POST" => HandlePost(path, splitRequestLine, body),
            _ => HandleUnsupportedHttpMethod(),
        };

        stream.Write(responseBytes);
        tcpClient.Close();
        return Task.CompletedTask;
    }

    private static byte[] HandleUnsupportedHttpMethod()
    {
        return Constants.MethodNotAllowedResponse;
    }

    private static byte[] HandlePost(string path, string[] splitRequestLine, string body)
    {
        if (path.StartsWith("/files/"))
        {
            string fileName = splitRequestLine[1].Split("/")[2];
            string filePath = Path.Combine(Constants.DefaultDataDirPath, fileName);

            File.WriteAllText(filePath, body);

            return Constants.CreatedResponse;
        }
        else
        {
            return Constants.NotFoundResponse;
        }
    }

    private static byte[] HandleGet(string path, StringBuilder builder, string[] lines, string[] splitRequestLine, Dictionary<string, string> headers)
    {
        return path switch
        {
            "/" => Constants.SuccessResponse,
            "/user-agent" => GetRequestHandlers.HandleUserAgentRequest(lines, builder, headers),
            string pth when pth.StartsWith("/echo/") => GetRequestHandlers.HandleEchoRequest(splitRequestLine, headers, builder),
            string pth when pth.StartsWith("/files/") => GetRequestHandlers.HandleFileRequest(splitRequestLine, builder),
            _ => Constants.NotFoundResponse
        };
    }
}
