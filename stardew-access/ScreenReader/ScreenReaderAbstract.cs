using stardew_access.Translation;

namespace stardew_access.ScreenReader
{
    public abstract class ScreenReaderAbstract : IScreenReader
    {
        public string prevText = "", prevTextTile = "", prevChatText = "", prevMenuText = "";
        private string menuPrefixText = "";
        private string prevMenuPrefixText = "";
        private string menuSuffixText = "";
        private string prevMenuSuffixText = "";
        private string menuPrefixNoQueryText = "";
        private string menuSuffixNoQueryText = "";

        public string PrevTextTile
        {
            get => prevTextTile;
            set => prevTextTile = value;
        }

        public string PrevMenuQueryText
        {
            get => prevMenuText;
            set => prevMenuText = value;
        }

        public string MenuPrefixText
        {
            get => menuPrefixText;
            set => menuPrefixText = value;
        }

        public string MenuSuffixText
        {
            get => menuSuffixText;
            set => menuSuffixText = value;
        }

        public string MenuPrefixNoQueryText
        {
            get => menuPrefixNoQueryText;
            set => menuPrefixNoQueryText = value;
        }

        public string MenuSuffixNoQueryText
        {
            get => menuSuffixNoQueryText;
            set => menuSuffixNoQueryText = value;
        }

        public abstract void InitializeScreenReader();

        public abstract void CloseScreenReader();

        public abstract bool Say(string text, bool interrupt);

        public bool TranslateAndSay(string translationKey, bool interrupt, object? translationTokens = null, TranslationCategory translationCategory = TranslationCategory.Default, bool disableTranslationWarnings = false)
        {
            if (string.IsNullOrWhiteSpace(translationKey))
                return false;

            return Say(Translator.Instance.Translate(translationKey, translationTokens, translationCategory, disableTranslationWarnings), interrupt);
        }

        public bool SayWithChecker(string text, bool interrupt, string? customQuery = null)
        {
            customQuery ??= text;

            if (string.IsNullOrWhiteSpace(customQuery))
                return false;

            if (prevText == customQuery)
                return false;

            prevText = customQuery;
            return Say(text, interrupt);
        }

        public bool TranslateAndSayWithChecker(string translationKey, bool interrupt, object? translationTokens = null, TranslationCategory translationCategory = TranslationCategory.Default, string? customQuery = null, bool disableTranslationWarnings = false)
        {
            if (string.IsNullOrWhiteSpace(translationKey))
                return false;

            return SayWithChecker(Translator.Instance.Translate(translationKey, translationTokens, translationCategory, disableTranslationWarnings), interrupt, customQuery);
        }

        public bool SayWithMenuChecker(string text, bool interrupt, string? customQuery = null)
        {
            customQuery ??= text;

            if (string.IsNullOrWhiteSpace(customQuery))
                return false;

            if (prevMenuText == customQuery && prevMenuSuffixText == MenuSuffixText && prevMenuPrefixText == MenuPrefixText)
                return false;

            prevMenuText = customQuery;
            prevMenuSuffixText = MenuSuffixText;
            prevMenuPrefixText = MenuPrefixText;
            bool re = Say($"{MenuPrefixNoQueryText}{MenuPrefixText}{text}{MenuSuffixText}{MenuSuffixNoQueryText}", interrupt);
            MenuPrefixNoQueryText = "";
            MenuSuffixNoQueryText = "";

            return re;
        }

        public bool TranslateAndSayWithMenuChecker(string translationKey, bool interrupt, object? translationTokens = null, TranslationCategory translationCategory = TranslationCategory.Menu, string? customQuery = null, bool disableTranslationWarnings = false)
        {
            if (string.IsNullOrWhiteSpace(translationKey))
                return false;

            return SayWithMenuChecker(Translator.Instance.Translate(translationKey, translationTokens, translationCategory, disableTranslationWarnings), interrupt, customQuery);
        }

        public bool SayWithChatChecker(string text, bool interrupt)
        {
            if (prevChatText == text)
                return false;

            prevChatText = text;
            return Say(text, interrupt);
        }

        public bool SayWithTileQuery(string text, int x, int y, bool interrupt)
        {
            string query = $"{text} x:{x} y:{y}";

            if (prevTextTile == query)
                return false;

            prevTextTile = query;
            return Say(text, interrupt);
        }

        public void Cleanup()
        {
            MainClass.ScreenReader.PrevMenuQueryText = "";
            MainClass.ScreenReader.MenuPrefixText = "";
            MainClass.ScreenReader.MenuSuffixText = "";
        }
    }
}
