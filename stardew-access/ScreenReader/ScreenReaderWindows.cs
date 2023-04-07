using DavyKager;
using System.IO;

namespace stardew_access.ScreenReader
{
    public class ScreenReaderWindows : IScreenReader
    {
        private bool isLoaded = false;
        public string prevText = "", prevTextTile = " ", prevChatText = "", prevMenuText = "";

        public string PrevTextTile
        {
            get { return prevTextTile; }
            set { prevTextTile = value; }
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
            if (text == null)
                return;

            if (!isLoaded)
                return;

            if (!MainClass.Config.TTS)
                return;

            if (text.Contains('^')) text = text.Replace('^', '\n');

            if (Tolk.Output(text, interrupt))
            {
                MainClass.DebugLog($"Speaking(interrupt: {interrupt}) = {text}");
            }
            else
            {
                MainClass.ErrorLog($"Failed to output text: {text}");
            }
        }

        public void SayWithChecker(string text, bool interrupt)
        {
            if (prevText != text)
            {
                prevText = text;
                Say(text, interrupt);
            }
        }

        public void SayWithMenuChecker(string text, bool interrupt)
        {
            if (prevMenuText != text)
            {
                prevMenuText = text;
                Say(text, interrupt);
            }
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
