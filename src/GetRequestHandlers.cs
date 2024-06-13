using System.Text;

namespace codecrafters_http_server;

internal class GetRequestHandlers
{
    internal static byte[] HandleEchoRequest(string[] splitRequestLine, Dictionary<string, string> headers, StringBuilder builder)
    {
        string[] endpoint = splitRequestLine[1].Split("/");
        string echoValue = endpoint[2]; // endpoint[2] is /echo/endpoint[2]

        bool containsGzipEncoding = headers.ContainsKey("Accept-Encoding") &&
                                    headers["Accept-Encoding"]
                                    .Split(',')
                                    .Select(val => val.Trim())
                                    .Contains("gzip");

        if (!containsGzipEncoding)
        {
            builder.Append("HTTP/1.1 200 OK\r\n");
            builder.Append("Content-Type: text/plain\r\n");
            builder.Append($"Content-Length: {echoValue.Length}\r\n");
            builder.Append($"\r\n{echoValue}");

            return Encoding.UTF8.GetBytes(builder.ToString());
        }
        else
        {
            byte[] gZipBytes = Helpers.CompressStream(echoValue);

            builder.Append("HTTP/1.1 200 OK\r\n");
            builder.Append($"Content-Encoding: gzip\r\n");
            builder.Append($"Content-Type: text/plain\r\n");
            builder.Append($"Content-Length: {gZipBytes.Length}\r\n\r\n");

            byte[] responseWithoutBody = Encoding.UTF8.GetBytes(builder.ToString());

            return responseWithoutBody.Concat(gZipBytes).ToArray();
        }
    }
    internal static byte[] HandleFileRequest(string[] splitRequestLine, StringBuilder builder)
    {
        string fNameInPath = splitRequestLine[1].Split("/")[2];
        string[] filePaths = Directory.GetFiles(Constants.DefaultDataDirPath);

        return Helpers.BuildFileResponse(filePaths, fNameInPath, builder);
    }

    internal static byte[] HandleUserAgentRequest(string[] lines, StringBuilder builder, Dictionary<string, string> headers)
    {
        bool userAgentHeaderExists = headers.TryGetValue("User-Agent", out string? headerValue);

        if (!userAgentHeaderExists)
        {
            return Constants.BadRequestResponse;
        }

        builder.Append("HTTP/1.1 200 OK\r\n");
        builder.Append("Content-Type: text/plain\r\n");
        builder.Append($"Content-Length: {headerValue?.Length}\r\n");
        builder.Append($"\r\n{headerValue}");

        return Encoding.UTF8.GetBytes(builder.ToString());
    }
}
