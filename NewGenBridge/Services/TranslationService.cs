using Microsoft.Extensions.Caching.Memory;
using NewGenBridge.UoW;

namespace NewGenBridge.Services
{
    public interface ITranslationService
    {
        string T(string key);
    }
    public class TranslationService : ITranslationService
    {
        UnitOfWork _unitOfWork;
        HttpContext _httpContext;
        IMemoryCache _memoryCache;

        public TranslationService(UnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _httpContext = httpContextAccessor.HttpContext;
            _memoryCache = memoryCache;
        }

        public string T(string key)
        {
            string lang = _httpContext?.Request.Cookies["lang"] ?? "en";

            var cacheKey = $"translation_{key}_{lang}";
            if (_memoryCache.TryGetValue(cacheKey, out string cachedTranslation))
            {
                return cachedTranslation;
            }

            var translation = _unitOfWork.Translations.GetByKey(key);
            if (translation == null)
                return key;

            var translatedValue = lang switch
            {
                "en" => translation.EnValue,
                "fr" => translation.FrValue,
                "it" => translation.ItValue,
                "tr" => translation.TrValue,
                "de" => translation.DeValue,
                "ru" => translation.RuValue,
                _ => translation.EnValue
            } ?? key;

            _memoryCache.Set(cacheKey, translatedValue, TimeSpan.FromMinutes(10));
            return translatedValue;
        }
    }
}
