namespace NewGenBridge.Helpers
{
    public static class NameLatinizer
    {
        static readonly Dictionary<char, string> _cyrillicToLatin = new()
        {
            { 'а', "a" },
            { 'б', "b" },
            { 'в', "v" },
            { 'г', "g" },
            { 'д', "d" },
            { 'е', "e" },
            { 'ё', "yo" },
            { 'ж', "zh" },
            { 'з', "z" },
            { 'и', "i" },
            { 'й', "y" },
            { 'к', "k" },
            { 'л', "l" },
            { 'м', "m" },
            { 'н', "n" },
            { 'о', "o" },
            { 'п', "p" },
            { 'р', "r" },
            { 'с', "s" },
            { 'т', "t" },
            { 'у', "u" },
            { 'ф', "f" },
            { 'х', "kh" },
            { 'ц', "ts" },
            { 'ч', "ch" },
            { 'ш', "sh" },
            { 'щ', "sch" },
            { 'ъ', "" },
            { 'ы', "y" },
            { 'ь', "" },
            { 'э', "e" },
            { 'ю', "yu" },
            { 'я', "ya" }
        };
        public static string LatinizeUserName(string name)
        {
            string latinized = "";
            name = name.ToLower();
            foreach (char c in name)
            {
                bool isLatinOrNumber = (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9');
                if (isLatinOrNumber)
                {
                    latinized += c;
                    continue;
                }
                latinized += _cyrillicToLatin.TryGetValue(c, out string? value) ? value : "";
            }
            return latinized;
        }

        public static string LatinizeFileName(string fileName)
        {
            string latinized = "";
            fileName = fileName.ToLowerInvariant();
            foreach (char c in fileName)
            {
                bool isLatinOrNumber = (c >= 'a' && c <= 'z') || (c >= '0' && c <= '9');
                if (isLatinOrNumber)
                {
                    latinized += c;
                    continue;
                }
                latinized += _cyrillicToLatin.TryGetValue(c, out string? value) ? value : "";
            }
            return latinized;
        }

    }

    public enum Language
    {
        Russian,
        English
    }
}
