# C# HTTP Server

This is a basic HTTP server implementation from scratch made for the CodeCrafters "Build Your Own HTTP Server" challenge.

## What I Learned

### Built-In C# Libraries

- **System.Net and System.Net.Sockets**: Used for creating TCP listeners and handling network streams.
- **System.Threading**: Used threading and tasks to support multiple, concurrent requests.
- **System.Text**: Used for encoding and decoding text data.
- **System.IO**: Used for file operations.
- **System.IO.Compression**: Used for compressing data streams.

### HTTP Concepts

- **HTTP Methods**: Implemented basic handling for GET and POST requests.
- **Headers**: Learned how to parse and use HTTP headers.
- **Status Codes**: Learned some new HTTP status codes like 405 Method Not Allowed.
- **Content Encoding**: Implemented support for gzip content encoding.

### C# Features

- **Asynchronous Programming**: Used `async` and `await` for asynchronous IO operations.
- **LINQ**: Used LINQ for string manipulation and collection operations.
- **File Handling**: Learned how to read from and write to files.

## How to Run

**Ensure you have .NET 8.0 installed**

Follow these steps to run the project:

1. **Clone the repository**:

    ```sh
    git clone https://github.com/OmarAshraf-02/HttpServer.git
    ```

2. **Navigate to the project directory**:

    ```sh
    cd HttpServer
    ```

3. **Build the project**:

    ```sh
    dotnet build
    ```

4. **Run the server**:

    ```sh
    dotnet run
    ```

The server will start and listen on the specified port. You can send HTTP requests to test its functionality.

### Supported Requests

#### GET Requests

To make a GET request, you can use `curl`:

```sh
curl http://localhost:4221/
```

