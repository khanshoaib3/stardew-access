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

    public class ScreenReaderLinux : ScreenReaderAbstract
    {
        [DllImport("libspeechdwrapper")]
        private static extern int Initialize();

        [DllImport("libspeechdwrapper")]
        private static extern int Speak(GoString text, bool interrupt);

        [DllImport("libspeechdwrapper")]
        private static extern int Close();

        private bool initialized = false, resolvedDll = false;

        public override void InitializeScreenReader()
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

        public override void CloseScreenReader()
        {
            if (initialized)
            {
                _ = Close();
                initialized = false;
            }
        }

        public override bool Say(string text, bool interrupt)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            if (!initialized) return false;
            if (!MainClass.Config.TTS) return false;

            if (text.Contains('^')) text = text.Replace('^', '\n');

            GoString str = new(text, text.Length);
            int re = Speak(str, interrupt);

            if (re != 1)
            {
                MainClass.ErrorLog($"Failed to output text: {text}");
                return false;
            }
            else
            {
                #if DEBUG
                MainClass.DebugLog($"Speaking(interrupt: {interrupt}) = {text}");
                #endif
                return true;
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
