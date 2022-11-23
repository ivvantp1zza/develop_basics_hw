using HttpServer.Cookies;

namespace HttpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new HttpServer();
            server.Start();
            while (true)
            {
                ReadCommand(Console.ReadLine(), server);
            }
        }
        static void ReadCommand(string command, HttpServer server)
        {
            switch (command)
            {
                case "start": 
                    server.Start();
                    break;
                case "stop":
                    server.Stop();
                    break;
                case "restart":
                    server.Stop();
                    server.Start();
                    break;
                default:
                    Console.WriteLine("Unknown command!");
                    break;
            }
        }
    }
}