using System;
using System.Net;
using System.IO;
 
namespace NetConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new HttpServer("http://localhost:1488/google/");
            server.Start();
            while(true)
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