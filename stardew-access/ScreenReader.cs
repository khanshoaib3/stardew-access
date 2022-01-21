using AccessibleOutput;
using StardewModdingAPI;

namespace stardew_access
{
    public class ScreenReader
    {
        public IAccessibleOutput? screenReader = null;
        public string prevText = "", prevTextTile = " ", prevChatText = "", prevMenuText = "";

        /// <summary>Initializes the screen reader.</summary>
        public void InitializeScreenReader()
        {
            NvdaOutput? nvdaOutput = null;
            JawsOutput? jawsOutput = null;
            SapiOutput? sapiOutput = null;

            // Initialize NVDA
            try
            {
                nvdaOutput = new NvdaOutput();
            }
            catch (Exception) { }

            // Initialize JAWS
            try
            {
                jawsOutput = new JawsOutput();
            }
            catch (Exception) { }

            // Initialize SAPI
            try
            {
                sapiOutput = new SapiOutput();
            }
            catch (Exception){ }

            if (nvdaOutput != null && nvdaOutput.IsAvailable())
                screenReader = nvdaOutput;
            else if (jawsOutput != null && jawsOutput.IsAvailable())
                screenReader = jawsOutput;
            else if (sapiOutput != null && sapiOutput.IsAvailable())
                screenReader = sapiOutput;
            else
                MainClass.monitor.Log($"Unable to load any screen reader!", LogLevel.Error);
        }

        /// <summary>Speaks the text via the loaded screen reader (if any).</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        public void Say(string text, bool interrupt)
        {
            if (screenReader == null)
                return;

            screenReader.Speak(text, interrupt);
        }

        /// <summary>Speaks the text via the loaded screen reader (if any).
        /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        public void SayWithChecker(string text, bool interrupt)
        {
            if (screenReader == null)
                return;

            if (prevText != text)
            {
                prevText = text;
                screenReader.Speak(text, interrupt);
            }
        }

        /// <summary>Speaks the text via the loaded screen reader (if any).
        /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
        /// <br/><br/>Use this when narrating hovered component in menus to avoid interference.</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        public void SayWithMenuChecker(string text, bool interrupt)
        {
            if (screenReader == null)
                return;

            if (prevMenuText != text)
            {
                prevMenuText = text;
                screenReader.Speak(text, interrupt);
            }
        }

        /// <summary>Speaks the text via the loaded screen reader (if any).
        /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
        /// <br/><br/>Use this when narrating chat messages to avoid interference.</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        public void SayWithChatChecker(string text, bool interrupt)
        {
            if (screenReader == null)
                return;

            if (prevChatText != text)
            {
                prevChatText = text;
                screenReader.Speak(text, interrupt);
            }
        }

        /// <summary>Speaks the text via the loaded screen reader (if any).
        /// <br/>Skips the text narration if the previously narrated text was the same as the one provided.
        /// <br/><br/>Use this when narrating texts based on tile position to avoid interference.</summary>
        /// <param name="text">The text to be narrated.</param>
        /// <param name="x">The X location of tile.</param>
        /// <param name="y">The Y location of tile.</param>
        /// <param name="interrupt">Whether to skip the currently speaking text or not.</param>
        public void SayWithTileQuery(string text, int x, int y, bool interrupt)
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
