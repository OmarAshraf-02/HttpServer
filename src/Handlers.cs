using System.Net.Sockets;
using System.Text;

namespace codecrafters_http_server;
public static class Handlers
{
    public static Task HandleRequest(TcpClient tcpClient)
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
        Dictionary<string, string> headers = Helpers.ParseHttpHeaders(lines);

        string responseString = "";
        byte[] responseBytes;
        StringBuilder responseBuilder = new StringBuilder();

        if (httpVerb == "GET")
        {
            responseString = HandleGet(path, responseBuilder, lines, splitRequestLine, headers);
        }
        else if (httpVerb == "POST")
        {
            responseString = HandlePost(path, splitRequestLine, body);
        }

        responseBytes = Encoding.UTF8.GetBytes(responseString);
        stream.Write(responseBytes);
        return Task.CompletedTask;
    }
    public static string HandlePost(string path, string[] splitRequestLine, string body)
    {
        if (path.StartsWith("/files/"))
        {
            string fileNameInRequestPath = splitRequestLine[1].Split("/")[2];
            string filePath = Constants.DefaultDataDirPath + $"/{fileNameInRequestPath}";

            using StreamWriter streamWriter = File.CreateText(filePath);
            streamWriter.Write($"{body}");

            return Constants.CreatedResponse;
        }
        else
        {
            return Constants.NotFoundResponse;
        }
    }

    public static string HandleGet(string path, StringBuilder builder, string[] lines, string[] splitReqLine, Dictionary<string, string> headers)
    {
        if (path == "/") // checks if 2nd argument in request line (aka. request target) is correct
        {
            return Constants.SuccessResponse;
        }
        else if (path == "/user-agent")
        {
            string headerValue = Helpers.ExtractUserAgentHeader(lines);

            builder.Append("HTTP/1.1 200 OK\r\n");
            builder.Append("Content-Type: text/plain\r\n");
            builder.Append($"Content-Length: {headerValue.Length}\r\n");
            builder.Append($"\r\n{headerValue}");

            return builder.ToString();
        }
        else if (path.StartsWith("/echo/"))
        {
            string[] endpoint = splitReqLine[1].Split("/");

            bool containsGzipEncoding = headers.ContainsKey("Accept-Encoding") &&
                                       (headers["Accept-Encoding"] == "gzip" ||
                                           (
                                            headers["Accept-Encoding"].Contains(',') &&
                                            headers["Accept-Encoding"].Split(",").Select(val => val.Trim()).ToList().Contains("gzip")
                                           )
                                       );

            if (!containsGzipEncoding)
            {
                builder.Append("HTTP/1.1 200 OK\r\n");
                builder.Append("Content-Type: text/plain\r\n");
                builder.Append($"Content-Length: {endpoint[2].Length}\r\n"); // endpoint[2] is /echo/endpoint[2]
                builder.Append($"\r\n{endpoint[2]}");

                Console.WriteLine(builder.ToString());

                return builder.ToString();
            }
            else
            {
                builder.Append("HTTP/1.1 200 OK\r\n");
                builder.Append($"Content-Encoding: gzip\r\n");
                builder.Append($"Content-Type: text/plain\r\n");
                builder.Append($"Content-Length: {endpoint[2].Length}\r\n");
                builder.Append($"\r\n{endpoint[2]}");

                Console.WriteLine(builder.ToString());

                return builder.ToString();
            }
        }
        else if (path.StartsWith("/files/"))
        {
            string fNameInPath = splitReqLine[1].Split("/")[2];
            string[] filePaths = Directory.GetFiles(Constants.DefaultDataDirPath);

            return HandleFileRequest(filePaths, fNameInPath, builder);
        }
        else
        {
            return Constants.NotFoundResponse;
        }
    }
    public static string HandleFileRequest(string[] filePathArr, string requestedFile, StringBuilder builder)
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
            result = Constants.NotFoundResponse;
        }

        return result;
    }
}
