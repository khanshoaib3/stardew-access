using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Minigames;

namespace stardew_access.Patches
{
    public class MiniGamesPatches
    {
        public static string grandpaStoryQuery = " ";

        internal static void IntroPatch(Intro __instance, int ___currentState)
        {
            try
            {
                MainClass.DebugLog(___currentState + "\t intro");
                if (___currentState == 3)
                {
                    string text = "Travelling to Stardew Valley bus stop";
                    if (grandpaStoryQuery != text)
                    {
                        grandpaStoryQuery = text;
                        MainClass.ScreenReader.Say(text, true);
                        return;
                    }
                }
                if (___currentState == 4)
                {
                    string text = "Stardew valley 0.5 miles away";
                    if (grandpaStoryQuery != text)
                    {
                        grandpaStoryQuery = text;
                        MainClass.ScreenReader.Say(text, true);
                        return;
                    }
                }
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void GrandpaStoryPatch(GrandpaStory __instance, StardewValley.Menus.LetterViewerMenu ___letterView, bool ___drawGrandpa, bool ___letterReceived, bool ___mouseActive, Queue<string> ___grandpaSpeech, int ___grandpaSpeechTimer, int ___totalMilliseconds, int ___scene, int ___parallaxPan)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                // TODO add scene 0 explaination
                // if(___scene == 0)
                // {
                //
                // }

                if (___drawGrandpa)
                {
                    if (___grandpaSpeech.Count > 0 && ___grandpaSpeechTimer > 3000)
                    {
                        string text = ___grandpaSpeech.Peek();
                        if (grandpaStoryQuery != text)
                        {
                            grandpaStoryQuery = text;
                            MainClass.ScreenReader.Say(text, true);
                            return;
                        }
                    }
                }
                if (___scene == 3)
                {
                    string text = Game1.content.LoadString("Strings\\StringsFromCSFiles:GrandpaStory.cs.12059");
                    if (grandpaStoryQuery != text)
                    {
                        grandpaStoryQuery = text;
                        MainClass.ScreenReader.Say(text, true);
                        return;
                    }
                }

                // TODO add scene 4 & 5 explaination
                // if(___scene == 4)
                // {
                // 
                // }
                // if(___scene == 5)
                // {
                // 
                // }

                if (___scene == 6)
                {
                    if (___grandpaSpeechTimer > 3000)
                    {
                        if (clickableGrandpaLetterRect(___parallaxPan, ___grandpaSpeechTimer).Contains(x, y))
                        {
                            string text = "Left click to open grandpa's letter";
                            if (grandpaStoryQuery != text)
                            {
                                grandpaStoryQuery = text;
                                MainClass.ScreenReader.Say(text, true);
                                return;
                            }
                        }
                        else if (___letterView == null)
                        {
                            Point pos = clickableGrandpaLetterRect(___parallaxPan, ___grandpaSpeechTimer).Center;
                            Game1.setMousePositionRaw((int)((float)pos.X * Game1.options.zoomLevel), (int)((float)pos.Y * Game1.options.zoomLevel));
                            return;
                        }
                    }
                }
                if (___letterView != null)
                {
                    DialoguePatches.NarrateLetterContent(___letterView);
                }
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static Rectangle clickableGrandpaLetterRect(int ___parallaxPan, int ___grandpaSpeechTimer)
        {
            return new Rectangle((int)Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730).X + (286 - ___parallaxPan) * 4, (int)Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730).Y + 218 + Math.Max(0, Math.Min(60, (___grandpaSpeechTimer - 5000) / 8)), 524, 344);
        }
    }
}