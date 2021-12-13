using AccessibleOutput;
using StardewModdingAPI;

namespace stardew_access
{
    internal class ScreenReader
    {
        public static IAccessibleOutput? screenReader = null;
        private static string prevText = "";

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
    }
}
