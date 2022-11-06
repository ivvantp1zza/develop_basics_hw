using System.Net;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;

namespace HttpServer;

public class HttpServer
{
    private ServerSettings _settings = new() { Port = 1488, Directory = @"..\..\..\site" };
    private static HttpListener _listener;
    private bool _isRunning = false;
    public HttpServer()
    {
        _listener = new HttpListener();
    }

    public void Start()
    {
        if (_isRunning)
            return;
        var path = @"../../../Settings.json";
        if (File.Exists(path))
        {
            var f = File.ReadAllText(path);
            _settings = JsonConvert.DeserializeObject<ServerSettings>(f);
        }
        _listener.Prefixes.Add($"http://localhost:{_settings.Port}/");
        _listener.Start();
        _isRunning = true;
        Console.WriteLine("Server started");
        Manage();
    }

    public async Task Manage()
    {
        while (true)
        {
            var context = await _listener.GetContextAsync();
            var request = context.Request;
            var response = context.Response;
            var url = request.RawUrl;
            var file = GetFileBytes(url);
            if (file is null)
            {
                response.Headers.Set("Content-Type", "text/plain");
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Resource not found"));
                response.OutputStream.Close();
                response.Close();
            }
            else
            {
                var lastSegment = request.Url.Segments.Last().Split(".");
                var fileType = lastSegment.Length == 2 ? lastSegment[1] : "";
                var contentType = "text/plain";
                switch (fileType)
                {
                    case "html":
                        contentType = "text/html";
                        break;
                    case "css":
                        contentType = "text/css";
                        break;
                    case "png":
                        contentType = "image/png";
                        break;
                }
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Headers.Set("Content-Type",contentType);
                response.ContentLength64 = file.Length;
                var output = response.OutputStream;
                output.Write(file, 0, file.Length);
                output.Close();
                response.Close();
            }
        }
    }

    public void Stop()
    {
        if (!_isRunning)
            return;
        _listener.Stop();
        _isRunning = false;
        Console.WriteLine("Server stopped");
        
    }

    private byte[] GetFileBytes(string url)
    {
        byte[] buffer = null;
        var directoryPath = _settings.Directory;
        var fullPath = directoryPath + url.Replace("/", "\\");
        if (Directory.Exists(fullPath))
        {
            fullPath += "\\index.html";
            if (File.Exists(fullPath))
            {
                using (var sourceStream = File.Open(fullPath, FileMode.Open))
                {
                    buffer = new byte[sourceStream.Length];
                    sourceStream.Read(buffer, 0, (int)sourceStream.Length);
                }
            }
        }
        else if (File.Exists(fullPath))
        {
            using (var sourceStream = File.Open(fullPath, FileMode.Open))
            {
                buffer = new byte[sourceStream.Length];
                sourceStream.Read(buffer, 0, (int)sourceStream.Length);
            }
        }
        return buffer;
    }
}