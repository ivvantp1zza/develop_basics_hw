using System.Collections.Specialized;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using HttpServer.Attributes;
using HttpServer.Controllers;
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

    public async void Start()
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
            if(!MethodHandler(context))
                StaticFileHandler(request, response);
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

    private byte[] TryGetFileBytes(string url, out bool returnedDefault)
    {
        returnedDefault = true;
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
            returnedDefault = false;
            using (var sourceStream = File.Open(fullPath, FileMode.Open))
            {
                buffer = new byte[sourceStream.Length];
                sourceStream.Read(buffer, 0, (int)sourceStream.Length);
            }
        }
        return buffer;
    }

    private void StaticFileHandler(HttpListenerRequest request, HttpListenerResponse response)
    {
        var file = TryGetFileBytes(request.RawUrl, out var retDefault);
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
            response.StatusCode = (int)HttpStatusCode.OK;
            response.Headers.Set("Content-Type", retDefault ? "text/html" : HttpHelper.GetContentType(request.Url));
            response.ContentLength64 = file.Length;
            var output = response.OutputStream;
            output.Write(file, 0, file.Length);
            output.Close();
            response.Close();
        }
    }

    private bool MethodHandler(HttpListenerContext _httpContext)
    {
        HttpListenerRequest request = _httpContext.Request;
        HttpListenerResponse response = _httpContext.Response;
        NameValueCollection parsed = null;
        if (request.HasEntityBody)
        {
            Stream body = request.InputStream;
            System.Text.Encoding encoding = request.ContentEncoding;
            StreamReader reader = new StreamReader(body, encoding);
            var bodyRet = reader.ReadToEnd();
            parsed = System.Web.HttpUtility.ParseQueryString(bodyRet);
        }

        if (_httpContext.Request.Url.Segments.Length < 2) return false;
        string controllerName = _httpContext.Request.Url.Segments[1].Replace("/", "");
        string[] strParams = _httpContext.Request.Url
                                .Segments
                                .Skip(2)
                                .Select(s => s.Replace("/", ""))
                                .ToArray();

        var assembly = Assembly.GetExecutingAssembly();
        var controller = assembly.GetTypes()
            .Where(t => Attribute.IsDefined(t, typeof(HttpController)))
            .FirstOrDefault(c => c.GetCustomAttribute<HttpController>().Route == null ? c.Name.ToLower().Equals(controllerName.ToLower()) : c.GetCustomAttribute<HttpController>().Route.ToLower().Equals(controllerName.ToLower()));

        if (controller == null) return false;

        var method = controller.GetMethods()
                .Where(m => m.GetCustomAttributes().Any(attr => attr.GetType().Name == $"Http{_httpContext.Request.HttpMethod}"))
                .FirstOrDefault(t => strParams.Length > 0 ? t.GetParameters().Length == strParams.Length : t.GetCustomAttribute<HttpMethodAttribute>().Route == null);


        if (method == null) return false;
        object? res = null; 
        if(request.HttpMethod == HttpMethod.Post.Method)
        {
            var login = parsed["Login"];
            var password = parsed["Password"];
            res = method.Invoke(Activator.CreateInstance(controller), new object[] { login, password });
            if (res is int && (int)res != -1)
            {
                response.AddHeader("Set-Cookie", $"IsAuthorize: true, Id = {(int)res}");
                response.ContentType = "text/plain";
                var str = $"welcome {login}!";
                byte[] buff = Encoding.ASCII.GetBytes(str);
                response.ContentLength64 = buff.Length;
                Stream op = response.OutputStream;
                op.Write(buff, 0, buff.Length);
                op.Close();
                response.Close();
                return true;
            }
            else
            {
                response.ContentType = "text/plain";
                var str = $"No such user like {login} registered in system.";
                byte[] buff = Encoding.ASCII.GetBytes(str);
                response.ContentLength64 = buff.Length;
                Stream op = response.OutputStream;
                op.Write(buff, 0, buff.Length);
                op.Close();
                response.Close();
                return true;
            }
        }
        else
        {
            object[] queryParams = method.GetParameters()
                            .Select((p, i) => Convert.ChangeType(strParams[i], p.ParameterType))
                            .ToArray();
            res = method.Invoke(Activator.CreateInstance(controller), queryParams);
        }
        response.ContentType = "Application/json";

        byte[] buffer = Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(res));
        response.ContentLength64 = buffer.Length;

        Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
        output.Close();
        response.Close();
        return true;
    }
}