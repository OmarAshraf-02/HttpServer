namespace codecrafters_http_server;

public class Helpers
{
    public static string ExtractUserAgentHeader(string[] lines)
    {
        string[] headersAndBody = lines[1..];
        string headerValue = headersAndBody[1].Remove(0, 12); // Removes the header key 'User-Agent: '

        return headerValue;
    }
    public static Dictionary<string, string> ParseHttpHeaders(string[] lines)
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

}
