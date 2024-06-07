using System.Net;
using System.Net.Sockets;
using System.Text;

// Uncomment this block to pass the first stage
try
{
    int port = 4221;
    TcpListener server = new TcpListener(IPAddress.Any, port);

    server.Start();

    byte[] reqBuffer = new byte[1024];
    string reqData;

    while (true)
    {
        Console.WriteLine("Waiting for connection");

        TcpClient client = server.AcceptTcpClient();
        Console.WriteLine("Connected");

        NetworkStream stream = client.GetStream();

        int bytes = stream.Read(reqBuffer, 0, reqBuffer.Length);

        // Basic responses
        string successResponse = "HTTP/1.1 200 OK\r\n\r\n";
        string notFoundResponse = "HTTP/1.1 404 Not Found\r\n\r\n";

        while (bytes != 0)
        {
            reqData = Encoding.UTF8.GetString(reqBuffer, 0, bytes);
            string[] lines = reqData.Split("\r\n");

            string requestLine = lines[0];

            string[] splitRequestLine = requestLine.Split(" ");
            string path = splitRequestLine[1];

            string responseString;
            byte[] responseBytes;

            if (path == "/") // checks if 2nd argument in request line (aka. request target) is correct
            {
                responseString = successResponse;
            }
            else if (path.StartsWith("/echo/"))
            {
                string[] endpoint = splitRequestLine[1].Split("/");
                responseString = $"HTTP/1.1 200 OK\r\n" +
                                  "Content-Type: text/plain\r\n" +
                                 $"Content-Length: {endpoint[2].Length}\r\n" + // endpoint[2] is /echo/endpoint[2]
                                 $"\r\n{endpoint[2]}";
            }
            else if (path == "/user-agent")
            {
                string[] headersAndBody = lines[1..];
                string headerValue = headersAndBody[1].Remove(0, 12); // Removes the header key 'User-Agent'

                responseString = "HTTP/1.1 200 OK\r\n" +
                                 "Content-Type: text/plain\r\n" +
                                $"Content-Length: {headerValue.Length}\r\n" +
                                $"\r\n{headerValue}";
            }
            else
            {
                responseString = notFoundResponse;
            }

            responseBytes = Encoding.UTF8.GetBytes(responseString);
            stream.Write(responseBytes);
        }
    }
}
catch (SocketException ex)
{
    Console.WriteLine($"SocketException: {ex}");
}
