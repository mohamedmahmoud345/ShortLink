using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShortLink.Api.Filters;

public class InternalOnlyAttribute : Attribute, IAsyncActionFilter
{
    private const string TokenHeaderName = "INTERNAL_SECURE_TOKEN";
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.HttpContext.Request;

        if (!request.Headers.TryGetValue(TokenHeaderName, out var extractedToken))
        {
            context.Result = new ContentResult()
            {
                StatusCode = 401,
                Content = "Access Denied: Missing Internal Token."
            };
            return;
        }
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var expectedToken = configuration["INTERNAL_SECURE_TOKEN"];
        if (string.IsNullOrEmpty(expectedToken))
        {
            context.Result = new ContentResult()
            {
                StatusCode = 401,
                Content = "Access Denied: Missing Internal Token."
            };
            return;
        }
        byte[] expectedTokenBytes = Encoding.UTF8.GetBytes(expectedToken);
        byte[] extractedTokenBytes = Encoding.UTF8.GetBytes(extractedToken!);

        if (expectedTokenBytes.Length != extractedTokenBytes.Length)
        {
            context.Result = new ContentResult()
            {
                StatusCode = 403,
                Content = "Access Denied: Invalid Internal Token."
            };
            return;
        }
        else if (!CryptographicOperations.FixedTimeEquals(extractedTokenBytes, expectedTokenBytes))
        {
            context.Result = new ContentResult()
            {
                StatusCode = 403,
                Content = "Access Denied: Invalid Internal Token."
            };
            return;
        }

        await next();
    }
}
