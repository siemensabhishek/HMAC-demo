using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace YourWebApi.MessageHandlers
{

    public class HmacAuthenticationHandler : DelegatingHandler
    {
        private const string SignatureHeaderName = "X-Signature";
        private const string SharedSecretKey = "yourSharedSecretKey";

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Retrieve the HMAC from the request headers or body
            string hmac = request.Headers.GetValues(SignatureHeaderName).FirstOrDefault() ?? string.Empty;

            // Retrieve the message from the request
            string message = await request.Content.ReadAsStringAsync();

            // Validate the HMAC
            if (!IsValidHmac(hmac, message, SharedSecretKey))
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized)
                {
                    Content = new StringContent("Invalid HMAC")
                };
            }

            // Call the inner handler to process the request
            return await base.SendAsync(request, cancellationToken);
        }

        private bool IsValidHmac(string hmac, string message, string secretKey)
        {
            // Compute the expected HMAC
            string expectedHmac = ComputeHmac(message, secretKey);

            // Compare the expected HMAC with the received HMAC
            return string.Equals(hmac, expectedHmac, StringComparison.OrdinalIgnoreCase);
        }

        private string ComputeHmac(string message, string secretKey)
        {
            using (var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hmacBytes = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(message));
                return Convert.ToBase64String(hmacBytes);
            }
        }
    }



    //public class HmacAuthenticationHandler : IMiddleware
    //{
    //    private const string SignatureHeaderName = "X-Signature";
    //    private const string SharedSecretKey = "yourSharedSecretKey";

    //    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    //    {
    //        // Retrieve the HMAC from the request headers or body
    //        string hmac = context.Request.Headers[SignatureHeaderName].FirstOrDefault() ?? string.Empty;

    //        // Retrieve the message from the request
    //        string message = await GetMessageFromRequest(context.Request);

    //        // Validate the HMAC
    //        if (!IsValidHmac(hmac, message, SharedSecretKey))
    //        {
    //            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    //            await context.Response.WriteAsync("Invalid HMAC");
    //            return;
    //        }

    //        // Call the next middleware in the pipeline
    //        await next(context);
    //    }

    //    private async Task<string> GetMessageFromRequest(HttpRequest request)
    //    {
    //        // Read the request body and return it as a string
    //        using (var reader = new StreamReader(request.Body, Encoding.UTF8))
    //        {
    //            return await reader.ReadToEndAsync();
    //        }
    //    }

    //    private bool IsValidHmac(string hmac, string message, string secretKey)
    //    {
    //        // Compute the expected HMAC
    //        string expectedHmac = ComputeHmac(message, secretKey);

    //        // Compare the expected HMAC with the received HMAC
    //        return string.Equals(hmac, expectedHmac, StringComparison.OrdinalIgnoreCase);
    //    }

    //    private string ComputeHmac(string message, string secretKey)
    //    {
    //        using (var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
    //        {
    //            byte[] hmacBytes = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(message));
    //            return Convert.ToBase64String(hmacBytes);
    //        }
    //    }
    //}



}
