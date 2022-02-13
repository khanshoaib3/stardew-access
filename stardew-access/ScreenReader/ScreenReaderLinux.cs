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
        [DllImport("libspeechdwrapper.so")]
        private static extern int Initialize();

        [DllImport("libspeechdwrapper.so")]
        private static extern int Speak(GoString text, bool interrupt);

        [DllImport("libspeechdwrapper.so")]
        private static extern int Close();

        public string prevText = "", prevTextTile = " ", prevChatText = "", prevMenuText = "";
        private bool initialized = false;

        public string PrevTextTile
        {
            get { return prevTextTile; }
            set { prevTextTile = value; }
        }

        public void InitializeScreenReader()
        {
            int res = Initialize();
            if (res == 1)
            {
                initialized = true;
            }
        }

        public void CloseScreenReader()
        {
            if (initialized)
            {
                Close();
                initialized = false;
            }
        }

        public void Say(string text, bool interrupt)
        {
            if (text == null)
                return;

            if (initialized)
            {
                GoString str = new GoString(text, text.Length);
                Speak(str, interrupt);
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
    }
}
