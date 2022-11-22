namespace HttpServer.Attributes;

public class HttpPOST : HttpMethodAttribute
{
    public HttpPOST(string route = null) : base(route)
    { }
}