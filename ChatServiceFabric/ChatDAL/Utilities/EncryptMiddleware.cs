using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ChatDAL.Utilities
{
    public class EncryptionMiddleware
    {
        private readonly RequestDelegate _next;

        public EncryptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            //Respose will be encrypted if Return Status is 200.
            if (httpContext.Request.Path.ToString().Contains("api") && httpContext.Response.StatusCode == 200)
            {
                Stream originBody = httpContext.Response.Body;
                Stream encryptedBody;
                try
                {
                    using(Stream memStream = new MemoryStream())
                    {
                        httpContext.Response.Body = memStream;

                        await _next(httpContext).ConfigureAwait(false);

                        memStream.Position = 0;
                        var responseBody = new StreamReader(memStream).ReadToEnd();

                        //Custom logic to modify response
                        responseBody = Utilities.EncryptString(responseBody);

                        var memoryStreamModified = new MemoryStream();
                        var sw = new StreamWriter(memoryStreamModified);
                        sw.Write(responseBody);
                        sw.Flush();
                        memoryStreamModified.Position = 0;

                        await memoryStreamModified.CopyToAsync(originBody).ConfigureAwait(false);
                    }
                }
                finally
                {
                    httpContext.Response.Body = originBody;
                }

            }
            else
            {
             await _next(httpContext);
            }

        }
    }
}
