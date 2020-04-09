using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace APBD_19._03_CW3.Middlewares
{
    public class LogMiddleware
    {
        
        private readonly RequestDelegate _next;
        
        public LogMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task InvokeAsync(HttpContext httpContext)
        {
            string contextMethod = httpContext.Request.Method;
            string contextPath = httpContext.Request.Path;
            string queryString = httpContext.Request?.QueryString.ToString();
            string contextBody;
           
            using (StreamReader reader
                = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                contextBody = await reader.ReadToEndAsync();
            }

            string message = "Method: " + contextMethod + "\nPath: " + contextPath + "\nQueryString: " + queryString + "\nBody: " + contextBody + "\n";

            using (StreamWriter stream = new StreamWriter("requestsLog.txt",true))
            {
                stream.WriteLine(message);
                stream.Flush();
                stream.Close();
            }

            await _next(httpContext);
        }

    }
}