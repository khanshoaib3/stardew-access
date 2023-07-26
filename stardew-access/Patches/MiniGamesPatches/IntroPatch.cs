using StardewValley.Minigames;

namespace stardew_access.Patches
{
    public class IntroPatch
    {
        internal static string introQuery = " ";

        internal static void DrawPatch(Intro __instance, int ___currentState)
        {
            try
            {
                if (MainClass.ModHelper == null)
                    return;

                string toSpeak = " ";

                if (___currentState == 3)
                {
                    toSpeak = Translator.Instance.Translate("intro-scene3");
                }
                else if (___currentState == 4)
                {
                    toSpeak = Translator.Instance.Translate("intro-scene4");
                }

                if (toSpeak != " " && introQuery != toSpeak)
                {
                    introQuery = toSpeak;
                    MainClass.ScreenReader.Say(toSpeak, false);
                    return;
                }
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"An error occured in intro minigame patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
