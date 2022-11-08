using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    internal class HttpHelper
    {
        public static string GetContentType(Uri url)
        {
            var lastSegment = url.Segments.Last().Split(".");
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
            return contentType;
        }
    }
}
