using AccessibleOutput;

namespace stardew_access.ScreenReader
{
    public class ScreenReaderWindows : ScreenReaderInterface
    {
        public IAccessibleOutput? screenReader = null;
        public string prevText = "", prevTextTile = " ", prevChatText = "", prevMenuText = "";

        public string PrevTextTile{
            get{ return prevTextTile; }
            set{ prevTextTile=value; }
        }

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
        }

        public void CloseScreenReader(){
            
        }

        public void Say(string text, bool interrupt)
        {
            if (screenReader == null)
                return;

            screenReader.Speak(text, interrupt);
        }

        public void SayWithChecker(string text, bool interrupt)
        {
            if (screenReader == null)
                return;

            if (prevText != text)
            {
                prevText = text;
                Say(text, interrupt);
            }
        }

        public void SayWithMenuChecker(string text, bool interrupt)
        {
            if (screenReader == null)
                return;

            if (prevMenuText != text)
            {
                prevMenuText = text;
                Say(text, interrupt);
            }
        }

        public void SayWithChatChecker(string text, bool interrupt)
        {
            if (screenReader == null)
                return;

            if (prevChatText != text)
            {
                prevChatText = text;
                Say(text, interrupt);
            }
        }
        
        public void SayWithTileQuery(string text, int x, int y, bool interrupt)
        {
            if (screenReader == null)
                return;

            string query = $"{text} x:{x} y:{y}";

            if (prevTextTile != query)
            {
                prevTextTile = query;
                Say(text, interrupt);
            }
        }
    }
}
