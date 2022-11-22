namespace HttpServer.Attributes;

public class HttpGET : HttpMethodAttribute
{
    public HttpGET(string route = null) : base(route)
    {
    }
}