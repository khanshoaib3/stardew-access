/*
    Linux speech dispatcher library used:
    https://github.com/shoaib11120/libspeechdwrapper
*/

using System.Reflection;
using System.Runtime.InteropServices;

namespace stardew_access.ScreenReader
{
    public struct GoString
    {
        public string msg;
        public long len;

        public GoString(string msg, long len)
        {
            this.msg = msg;
            this.len = len;
        }
    }

    public class ScreenReaderLinux : IScreenReader
    {
        [DllImport("libspeechdwrapper")]
        private static extern int Initialize();

        [DllImport("libspeechdwrapper")]
        private static extern int Speak(GoString text, bool interrupt);

        [DllImport("libspeechdwrapper")]
        private static extern int Close();

        public string prevText = "", prevTextTile = "", prevChatText = "", prevMenuText = "";
        private bool initialized = false, resolvedDll = false;
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
            MainClass.InfoLog("Initializing speech dispatcher...");
            if (!resolvedDll)
            {
                NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);
                resolvedDll = true;
            }
            int res = Initialize();
            if (res == 1)
            {
                initialized = true;
                MainClass.InfoLog("Successfully initialized.");
            }
            else
            {
                MainClass.ErrorLog("Unable to initialize.");
            }
        }

        public void CloseScreenReader()
        {
            if (initialized)
            {
                _ = Close();
                initialized = false;
            }
        }

        public void Say(string text, bool interrupt)
        {
            if (string.IsNullOrWhiteSpace(text)) return;
            if (!initialized) return;
            if (!MainClass.Config.TTS) return;

            if (text.Contains('^')) text = text.Replace('^', '\n');

            GoString str = new(text, text.Length);
            int re = Speak(str, interrupt);

            if (re != 1)
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

        private static IntPtr DllImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            // libraryName is the name provided in DllImport i.e., [DllImport(libraryName)]
            if (libraryName != "libspeechdwrapper") return IntPtr.Zero;
            if (MainClass.ModHelper is null) return IntPtr.Zero;

            string libraryPath = Path.Combine(MainClass.ModHelper.DirectoryPath, "libraries", "linux", "libspeechdwrapper.so");
            return NativeLibrary.Load(libraryPath, assembly, searchPath);
        }
    }
}
