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
        private bool initialized = false;

        public string PrevTextTile
        {
            get { return prevTextTile; }
            set { prevTextTile = value; }
        }

        public void InitializeScreenReader()
        {
            MainClass.InfoLog("Initializing speech dispatcher...");
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), DllImportResolver);
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
            if (text == null) return;
            if (!initialized) return;
            if (!MainClass.Config.TTS) return;

            if (text.Contains('^')) text = text.Replace('^', '\n');

            GoString str = new(text, text.Length);
            int re = Speak(str, interrupt);

            if (re == 1)
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
