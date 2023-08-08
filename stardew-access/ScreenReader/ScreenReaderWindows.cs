using DavyKager;

namespace stardew_access.ScreenReader
{
    public class ScreenReaderWindows : IScreenReader
    {
        private bool isLoaded = false;
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

        public void InitializeScreenReader()
        {
            if (MainClass.ModHelper is not null)
            {
                // Set the path to load Tolk.dll from
                string dllDirectory = Path.Combine(MainClass.ModHelper.DirectoryPath, "libraries", "windows");;
                // Call SetDllDirectory to change the DLL search path
                NativeMethods.SetDllDirectory(dllDirectory);
            }
            MainClass.InfoLog("Initializing Tolk...");
            Tolk.TrySAPI(true);
            Tolk.Load();

            MainClass.InfoLog("Querying for the active screen reader driver...");
            string name = Tolk.DetectScreenReader();
            if (name != null)
            {
                MainClass.InfoLog($"The active screen reader driver is: {name}");
                isLoaded = true;
            }
            else
            {
                MainClass.ErrorLog("None of the supported screen readers is running");
                isLoaded = false;
            }
        }

        public void CloseScreenReader()
        {
            if (isLoaded)
            {
                Tolk.Unload();
                isLoaded = false;
            }
        }

        public void Say(string text, bool interrupt)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            if (!isLoaded) return;
            if (!MainClass.Config.TTS) return;

            if (text.Contains('^')) text = text.Replace('^', '\n');

            if (!Tolk.Output(text, interrupt))
            {
                MainClass.ErrorLog($"Failed to output text: {text}");
            }
            #if DEBUG
            else
            {
                MainClass.DebugLog($"Speaking(interrupt: {interrupt}) = {text}");
            }
            #endif

        }

        public void SayWithChecker(string text, bool interrupt, string? customQuery = null)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            if (prevText == text)
                return;

            prevText = text;
            Say(text, interrupt);
        }

        public void SayWithMenuChecker(string text, bool interrupt, string? customQuery = null)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            if (prevMenuText == text && prevMenuSuffixText == MenuSuffixText && prevMenuPrefixText == MenuPrefixText)
                return;

            prevMenuText = text;
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
