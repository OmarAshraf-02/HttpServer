# C# HTTP Server

This is a basic HTTP server implementation from scratch made for the CodeCrafters "Build Your Own HTTP Server" challenge.

## Setup

### Ensure you have .NET 8.0 installed

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

## Supported Requests

### `GET` Requests

#### `GET /`

To make a GET request, you can use `curl`:

```sh
curl -v http://localhost:4221/
```
This will return a `200 Success` response.

#### `GET /unsupported`

```sh
curl -v http://localhost:4221/abcdefg
```
This will return a `404 Not Found` response as it is not defined in the server.

#### `GET /echo/{str}`

```sh
curl -v http://localhost:4221/echo/foo
```
This will return a `200 Success` response, along with the `Content-Type: text/plain` & `Content-Length` with the length of your `{str}` in the path. It will also append the `{str}` to the response body.

#### `GET /echo/{str}` with `Accept-Encoding` header

```sh
curl -v --header "Accept-Encoding: gzip" http://localhost:4221/echo/foo
```
This will return a `200 Success` response. The raw `{str}` will be gzip encoded by the server and then appended to the response body. 

If you're using `curl` and want to see the response body in the terminal, use the `--output -` curl option (however this will be gibberish and may mess up your terminal). 

`Content-Length` will be set to the length of the gzip encoded data and the `Content-Type: text/plain`.

If `Accept-Encoding` is set to a comma-separated list of encoding methods, this will be parsed by the server to see if `gzip` is one of the options, so a request such as this will also work:

```sh
curl -v --header "Accept-Encoding: encoding1, gzip, encoding2" http://localhost:4221/echo/foo
```

#### `GET /user-agent`

```sh
curl -v --header "User-Agent: foobar/1.2.3" http://localhost:4221/user-agent
```
For this request, the server parses the `User-Agent` header and responds with a `200 Success` with `Content-Type: text/plain` & `Content-Length` set to the length of your `User-Agent` header's value. This value will also be appended to the response body.

#### `GET /files/{filename}`

```sh
curl -i http://localhost:4221/files/foo.txt
```
This request will search for a file with the name `filename` in the `/data` directory in the project root (not src). 

If a file with the same name is found, a `200 Success` response will be returned with `Content-Type: application/octet-stream` & `Content-Length` set to the size of the file in bytes. The file contents will also be appended to the response body.

If the file is not found, a `404 Not Found` response is returned. 

[`foo.txt`](./data/foo.txt) is included in the `/data` folder of the repository as an example for testing.

### `POST` Requests

#### `POST /unsupported`

```sh
curl -v --data "12345" -H "Content-Type: application/octet-stream" http://localhost:4221/unsupported
```
Any unsupported post request will return a `404 Not Found` response.

#### `POST /files/{filename}`

```sh
curl -v --data "12345" -H "Content-Type: application/octet-stream" http://localhost:4221/files/foo
```
If a `POST` request is sent to `/files/{filename}`, a file with the name `filename` will be created in the project's `/data` directory. This file's contents will consist of the contents of the request body (In the above case = 12345). A `201 Created` response will then be returned by the server.

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
