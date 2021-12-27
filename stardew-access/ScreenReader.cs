using AccessibleOutput;
using StardewModdingAPI;

namespace stardew_access
{
    internal class ScreenReader
    {
        public static IAccessibleOutput? screenReader = null;
        internal static string prevText = "", prevTextTile = " ";

        public static void initializeScreenReader()
        {
            NvdaOutput? nvdaOutput = null;
            JawsOutput? jawsOutput = null;
            SapiOutput? sapiOutput = null;

            // Initialize NVDA
            try
            {
                nvdaOutput = new NvdaOutput();
            }
            catch (Exception ex)
            {
                MainClass.monitor.Log($"Error initializing NVDA:\n{ex.StackTrace}", LogLevel.Info);
            }

            // Initialize JAWS
            try
            {
                jawsOutput = new JawsOutput();
            }
            catch (Exception ex)
            {
                MainClass.monitor.Log($"Error initializing JAWS:\n{ex.StackTrace}", LogLevel.Info);
            }

            // Initialize SAPI
            try
            {
                sapiOutput = new SapiOutput();
            }
            catch (Exception ex)
            {
                MainClass.monitor.Log($"Error initializing SAPI:\n{ex.StackTrace}", LogLevel.Info);
            }

            if (nvdaOutput != null && nvdaOutput.IsAvailable())
                screenReader = nvdaOutput;

            if (jawsOutput != null && jawsOutput.IsAvailable())
                screenReader = jawsOutput;

            if (sapiOutput != null && sapiOutput.IsAvailable())
                screenReader = sapiOutput;
        }

        public static void say(string text, bool interrupt)
        {
            if (screenReader == null)
                return;

            screenReader.Speak(text, interrupt);
        }

        public static void sayWithChecker(string text, bool interrupt)
        {
            if (screenReader == null)
                return;

            if (prevText != text)
            {
                prevText = text;
                screenReader.Speak(text, interrupt);
            }
        }

        public static void sayWithTileQuery(string text, int x, int y, bool interrupt)
        {
            if (screenReader == null)
                return;

            string query = $"{text} x:{x} y:{y}";

            if (prevTextTile != query)
            {
                prevTextTile = query;
                screenReader.Speak(text, interrupt);
            }
        }
    }
}
