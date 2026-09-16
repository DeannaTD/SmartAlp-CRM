namespace NewGenBridge.Middleware
{
    public class ApiKeyMiddleware
    {
        readonly RequestDelegate _next;
        readonly string _apiKey;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _apiKey = configuration.GetValue<string>("ApiSettings:ApiKey");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if(!context.Request.Path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }
            if(!context.Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key is missing");
                return;
            }

            if (!_apiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            await _next(context);
        }
    }
}
