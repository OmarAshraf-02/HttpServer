using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();
var socket = server.AcceptSocket(); // wait for client

// Responses
byte[] successResponse = Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n\r\n");
byte[] notFoundResponse = Encoding.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\n\r\n");

// Request
byte[] req = new byte[1024];
socket.Receive(req); // Blocks until request is received

string[] reqLines = Encoding.UTF8.GetString(req).Split("\r\n");
string requestLine = reqLines[0];

string[] splitRequestLine = requestLine.Split(" ");

if (splitRequestLine[1] == "/") // checks if 2nd argument in request line (aka. request target) is correct
{
    socket.Send(successResponse);
}
else
{
    socket.Send(notFoundResponse);
}

