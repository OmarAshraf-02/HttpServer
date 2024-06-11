using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    private const int Port = 4221;
    private static readonly string NotFoundResponse = "HTTP/1.1 404 Not Found\r\n\r\n";
    private static readonly string SuccessResponse = "HTTP/1.1 200 OK\r\n\r\n";
    private static readonly string CreatedResponse = "HTTP/1.1 201 Created\r\n\r\n";
    private static readonly string DefaultDataDirPath = "/tmp/data/codecrafters.io/http-server-tester";

    private static async Task Main(string[] args)
    {
        TcpListener server = new TcpListener(IPAddress.Any, Port);
        server.Start();
        Console.WriteLine($"Server started on port {Port}");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            await Task.Run(() => HandleRequest(client));
        }

        static Task HandleRequest(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();

            byte[] reqBuffer = new byte[1024];
            string reqData;
            int bytes = stream.Read(reqBuffer, 0, reqBuffer.Length);
            reqData = Encoding.UTF8.GetString(reqBuffer, 0, bytes);

            string[] lines = reqData.Split("\r\n");
            string requestLine = lines[0];
            string body = lines[lines.Length - 1];

            string[] splitRequestLine = requestLine.Split(" ");
            string httpVerb = splitRequestLine[0];
            string path = splitRequestLine[1];

            string responseString = "";
            byte[] responseBytes;
            StringBuilder responseBuilder = new StringBuilder();

            if (httpVerb == "GET")
            {
                responseString = HandleGet(path, responseBuilder, lines, splitRequestLine);
            }
            else if (httpVerb == "POST")
            {
                responseString = HandlePost(path, splitRequestLine, body);
            }

            responseBytes = Encoding.UTF8.GetBytes(responseString);
            stream.Write(responseBytes);
            return Task.CompletedTask;
        }
    }

    private static string HandlePost(string path, string[] splitRequestLine, string body)
    {
        if (path.StartsWith("/files/"))
        {
            string fileNameInRequestPath = splitRequestLine[1].Split("/")[2];
            string filePath = DefaultDataDirPath + $"/{fileNameInRequestPath}";

            using StreamWriter streamWriter = File.CreateText(filePath);
            streamWriter.Write($"{body}");

            return CreatedResponse;
        }
        else
        {
            return NotFoundResponse;
        }

    }

    private static string HandleGet(string path, StringBuilder builder, string[] lines, string[] splitReqLine)
    {
        if (path == "/") // checks if 2nd argument in request line (aka. request target) is correct
        {
            return SuccessResponse;
        }
        else if (path == "/user-agent")
        {
            string headerValue = ExtractUserAgentHeader(lines);

            builder.Append("HTTP/1.1 200 OK\r\n");
            builder.Append("Content-Type: text/plain\r\n");
            builder.Append($"Content-Length: {headerValue.Length}\r\n");
            builder.Append($"\r\n{headerValue}");

            return builder.ToString();
        }
        else if (path.StartsWith("/echo/"))
        {
            string[] endpoint = splitReqLine[1].Split("/");

            builder.Append("HTTP/1.1 200 OK\r\n");
            builder.Append("Content-Type: text/plain\r\n");
            builder.Append($"Content-Length: {endpoint[2].Length}\r\n"); // endpoint[2] is /echo/endpoint[2]
            builder.Append($"\r\n{endpoint[2]}");

            return builder.ToString();
        }
        else if (path.StartsWith("/files/"))
        {
            string fNameInPath = splitReqLine[1].Split("/")[2];
            string[] filePaths = Directory.GetFiles(DefaultDataDirPath);

            return HandleFileRequest(filePaths, fNameInPath, builder);
        }
        else
        {
            return NotFoundResponse;
        }
    }

    private static string ExtractUserAgentHeader(string[] lines)
    {
        string[] headersAndBody = lines[1..];
        string headerValue = headersAndBody[1].Remove(0, 12); // Removes the header key 'User-Agent: '

        return headerValue;
    }

    private static string HandleFileRequest(string[] filePathArr, string requestedFile, StringBuilder builder)
    {
        string result = "";

        foreach (string pth in filePathArr)
        {
            string fName = Path.GetFileName(pth);
            if (fName == requestedFile)
            {
                FileInfo fInfo = new FileInfo(pth);
                byte[] fContents = File.ReadAllBytes(fInfo.FullName);
                string content = Encoding.UTF8.GetString(fContents);

                builder.Append("HTTP/1.1 200 OK\r\n");
                builder.Append("Content-Type: application/octet-stream\r\n");
                builder.Append($"Content-Length: {fInfo.Length}\r\n");
                builder.Append($"\r\n{content}");

                result = builder.ToString();
                break;
            }
        }

        if (result == "")
        {
            result = NotFoundResponse;
        }

        return result;
    }

}