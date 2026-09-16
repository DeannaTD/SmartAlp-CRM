using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.DependencyInjection;
using NewGenBridge.Services;

namespace NewGenBridge.Helpers
{
    public static class TranslationExtensions
    {
        public static string T(this IHtmlHelper htmlHelper, string key)
        {
            var translationService = htmlHelper.ViewContext.HttpContext.RequestServices.GetRequiredService<ITranslationService>();
            return translationService?.T(key) ?? key;
        }
    }
}
