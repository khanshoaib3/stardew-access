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

        public abstract void Say(string text, bool interrupt);

        public void SayWithChecker(string text, bool interrupt, string? customQuery = null)
        {
            customQuery ??= text;

            if (string.IsNullOrWhiteSpace(customQuery))
                return;

            if (prevText == customQuery)
                return;

            prevText = customQuery;
            Say(text, interrupt);
        }

        public void SayWithMenuChecker(string text, bool interrupt, string? customQuery = null)
        {
            customQuery ??= text;

            if (string.IsNullOrWhiteSpace(customQuery))
                return;

            if (prevMenuText == customQuery && prevMenuSuffixText == MenuSuffixText && prevMenuPrefixText == MenuPrefixText)
                return;

            prevMenuText = customQuery;
            prevMenuSuffixText = MenuSuffixText;
            prevMenuPrefixText = MenuPrefixText;
            Say($"{MenuPrefixNoQueryText}{MenuPrefixText}{text}{MenuSuffixText}{MenuSuffixNoQueryText}", interrupt);
            MenuPrefixNoQueryText = "";
            MenuSuffixNoQueryText = "";
        }

        public void SayWithChatChecker(string text, bool interrupt)
        {
            if (prevChatText != text)
            {
                prevChatText = text;
                Say(text, interrupt);
            }
        }

        public void SayWithTileQuery(string text, int x, int y, bool interrupt)
        {
            string query = $"{text} x:{x} y:{y}";

            if (prevTextTile != query)
            {
                prevTextTile = query;
                Say(text, interrupt);
            }
        }
    }
}
