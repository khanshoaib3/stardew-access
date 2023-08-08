using DavyKager;

namespace stardew_access.ScreenReader
{
    public class ScreenReaderWindows : ScreenReaderAbstract
    {
        private bool isLoaded = false;

        public override void InitializeScreenReader()
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

        public override void CloseScreenReader()
        {
            if (isLoaded)
            {
                Tolk.Unload();
                isLoaded = false;
            }
        }

        public override void Say(string text, bool interrupt)
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
    }
}
