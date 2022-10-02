using System.Net;

namespace NetConsoleApp;

public class HttpServer
{
    private static HttpListener _listener;
    private bool ServerIsRunning = false;
    public HttpServer(string url)
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add(url);
    }

    public void Start()
    {
        if (ServerIsRunning)
            return;
        _listener.Start();
        ServerIsRunning = true;
        Manage();
        Console.WriteLine("Server started");
    }

    public async Task Manage()
    {
        while (true)
        {
                var context = await _listener.GetContextAsync();
                var request = context.Request;
                var response = context.Response;
                byte[] googleBuffer;
                var exePath = AppDomain.CurrentDomain.BaseDirectory;
                var path = Path.Combine(exePath, @"..\..\..\index.html");
                await using (var sourceStream = 
                             File.Open(path, FileMode.Open))
                {
                    googleBuffer = new byte[sourceStream.Length];
                    sourceStream.Read(googleBuffer, 0, (int)sourceStream.Length);
                }
                response.ContentLength64 = googleBuffer.Length;
                var output = response.OutputStream;
                output.Write(googleBuffer, 0, googleBuffer.Length);
                output.Close();
        }
    }

    public void Stop()
    {
        if (!ServerIsRunning)
            return;
        _listener.Stop();
        ServerIsRunning = false;
        Console.WriteLine("Server stopped");
        
    }
}