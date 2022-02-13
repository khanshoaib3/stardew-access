namespace stardew_access.ScreenReader
{
    public class API
    {

        public API()
        {
        }

        public void Say(String text, Boolean interrupt)
        {
            if (MainClass.GetScreenReader() == null)
                return;

            MainClass.GetScreenReader().Say(text, interrupt);
        }

        public void SayWithChecker(String text, Boolean interrupt)
        {
            if (MainClass.GetScreenReader() == null)
                return;

            MainClass.GetScreenReader().SayWithChecker(text, interrupt);
        }

        public void SayWithMenuChecker(String text, Boolean interrupt)
        {
            if (MainClass.GetScreenReader() == null)
                return;

            MainClass.GetScreenReader().SayWithMenuChecker(text, interrupt);
        }

        public void SayWithChatChecker(String text, Boolean interrupt)
        {
            if (MainClass.GetScreenReader() == null)
                return;

            MainClass.GetScreenReader().SayWithChatChecker(text, interrupt);
        }

        public void SayWithTileQuery(String text, int x, int y, Boolean interrupt)
        {
            if (MainClass.GetScreenReader() == null)
                return;

            MainClass.GetScreenReader().SayWithTileQuery(text, x, y, interrupt);
        }

    }
}