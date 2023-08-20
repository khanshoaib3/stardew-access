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
            Log.Info("Initializing Tolk...");
            Tolk.TrySAPI(true);
            Tolk.Load();

            Log.Info("Querying for the active screen reader driver...");
            string name = Tolk.DetectScreenReader();
            if (name != null)
            {
                Log.Info($"The active screen reader driver is: {name}");
                isLoaded = true;
            }
            else
            {
                Log.Error("None of the supported screen readers is running");
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

        public override bool Say(string text, bool interrupt)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            if (!isLoaded) return false;
            if (!MainClass.Config.TTS) return false;

            if (text.Contains('^')) text = text.Replace('^', '\n');

            if (!Tolk.Output(text, interrupt))
            {
                Log.Error($"Failed to output text: {text}");
                return false;
            }
            else
            {
                Log.Verbose($"Speaking(interrupt: {interrupt}) = {text}");
                return true;
            }
        }

    }
}
