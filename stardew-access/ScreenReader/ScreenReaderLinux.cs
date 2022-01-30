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

    public class ScreenReaderLinux : ScreenReaderInterface
    {
        [DllImport("libspeechdwrapper.so")]
        private static extern void Initialize();

        [DllImport("libspeechdwrapper.so")]
        private static extern void Speak(GoString text, bool interrupt);

        [DllImport("libspeechdwrapper.so")]
        private static extern void Close();

        public string prevText = "", prevTextTile = " ", prevChatText = "", prevMenuText = "";

        public string PrevTextTile{
            get{ return prevTextTile; }
            set{ prevTextTile=value; }
        }

        public void InitializeScreenReader()
        {
            Initialize();
        }

        public void CloseScreenReader(){
            Close();
        }

        public void Say(string text, bool interrupt)
        {
            GoString str = new GoString(text, text.Length);
            Speak(str, interrupt);
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
