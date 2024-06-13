using System.IO.Compression;
using System.Text;

namespace codecrafters_http_server;

public class Helpers
{
    internal static Dictionary<string, string> ParseHttpHeaders(string[] lines)
    {
        Dictionary<string, string> headers = new();
        string[] headerList = lines.Skip(1).Take(lines.Length - 2).ToArray();

        foreach (string header in headerList)
        {
            int separatorIndex = header.IndexOf(':');
            if (separatorIndex == -1) break;

            string? key = header.Substring(0, separatorIndex);
            string? value = header.Substring(separatorIndex + 1).Trim();
            headers.Add(key, value);
        }

        return headers;
    }

    internal static byte[] BuildFileResponse(string[] filePathArr, string requestedFile, StringBuilder builder)
    {
        byte[] result = new byte[1024];
        bool resultFilled = false;

        foreach (string pth in filePathArr)
        {
            string fileName = Path.GetFileName(pth);
            if (fileName == requestedFile)
            {
                FileInfo fInfo = new(pth);
                byte[] fContents = File.ReadAllBytes(fInfo.FullName);
                string content = Encoding.UTF8.GetString(fContents);

                builder.Append("HTTP/1.1 200 OK\r\n");
                builder.Append("Content-Type: application/octet-stream\r\n");
                builder.Append($"Content-Length: {fInfo.Length}\r\n");
                builder.Append($"\r\n{content}");

                result = Encoding.UTF8.GetBytes(builder.ToString());
                resultFilled = true;

                break;
            }
        }

        if (!resultFilled)
        {
            result = Constants.NotFoundResponse;
        }

        return result;
    }

    internal static byte[] CompressStream(string echoValue)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(echoValue);

        using MemoryStream memoryStream = new();
        using GZipStream gZipStream = new(memoryStream, CompressionMode.Compress, true);

        gZipStream.Write(bytes, 0, bytes.Length);
        gZipStream.Close();

        byte[] gZippedBytes = memoryStream.ToArray();
        return gZippedBytes;
    }
}
