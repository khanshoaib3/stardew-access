namespace stardew_access.ScreenReader
{
    public class ScreenReaderAPI
    {

        public ScreenReaderAPI()
        {
        }

        public void Say(String text, Boolean interrupt)
        {
            if (MainClass.ScreenReader == null)
                return;

            MainClass.ScreenReader.Say(text, interrupt);
        }

        public void SayWithChecker(String text, Boolean interrupt)
        {
            if (MainClass.ScreenReader == null)
                return;

            MainClass.ScreenReader.SayWithChecker(text, interrupt);
        }

        public void SayWithMenuChecker(String text, Boolean interrupt)
        {
            if (MainClass.ScreenReader == null)
                return;

            MainClass.ScreenReader.SayWithMenuChecker(text, interrupt);
        }

        public void SayWithChatChecker(String text, Boolean interrupt)
        {
            if (MainClass.ScreenReader == null)
                return;

            MainClass.ScreenReader.SayWithChatChecker(text, interrupt);
        }

        public void SayWithTileQuery(String text, int x, int y, Boolean interrupt)
        {
            if (MainClass.ScreenReader == null)
                return;

            MainClass.ScreenReader.SayWithTileQuery(text, x, y, interrupt);
        }

    }
}