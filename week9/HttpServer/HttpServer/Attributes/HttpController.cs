namespace HttpServer.Attributes;

public class HttpController : Attribute
{
    public string Route { get; }

    public HttpController(string route)
    {
        Route = route;
    }
}