using System.Globalization;

namespace NewGenBridge.Middleware
{
    public class RequestLocalizationMiddleware
    {
        RequestDelegate _next;

        public RequestLocalizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var lang = context.Request.Cookies["lang"] ?? "en";
            var culture = new CultureInfo(lang);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            await _next(context);
        }
    }
}
