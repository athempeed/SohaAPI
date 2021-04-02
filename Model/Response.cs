using System.Net;

namespace WebApplication10.Model
{
    public class Response
    {

        public HttpStatusCode StatusCode { get; set; }
        public object ResponseObject { get; set; }
    }
}
