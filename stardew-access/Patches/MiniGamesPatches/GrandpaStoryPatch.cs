using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Minigames;
using stardew_access.Translation;

namespace stardew_access.Patches
{
    public class GrandpaStoryPatch : IPatch
    {
        private static string grandpaStoryQuery = " ";

        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(GrandpaStory), "draw"),
                postfix: new HarmonyMethod(typeof(GrandpaStoryPatch), nameof(DrawPatch))
            );
        }

        private static void DrawPatch(GrandpaStory __instance, StardewValley.Menus.LetterViewerMenu? ___letterView,
            bool ___drawGrandpa, bool ___letterReceived, bool ___mouseActive, Queue<string> ___grandpaSpeech,
            int ___grandpaSpeechTimer, int ___totalMilliseconds, int ___scene, int ___parallaxPan)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                string toSpeak = "";

                if (___letterView != null)
                {
                    LetterViewerMenuPatch.NarrateMenu(___letterView);
                }

                if (MainClass.ModHelper == null)
                    return;

                if (___scene == 0)
                {
                    toSpeak = Translator.Instance.Translate("grandpastory-scene0", TranslationCategory.MiniGames);
                }
                else if (___drawGrandpa)
                {
                    if (___grandpaSpeech.Count > 0 && ___grandpaSpeechTimer > 3000)
                    {
                        toSpeak = ___grandpaSpeech.Peek();
                    }
                }
                else if (___scene == 3)
                {
                    toSpeak = Game1.content.LoadString("Strings\\StringsFromCSFiles:GrandpaStory.cs.12059");
                }
                else if (___scene == 4)
                {
                    toSpeak = Translator.Instance.Translate("grandpastory-scene4", TranslationCategory.MiniGames);
                }
                else if (___scene == 5)
                {
                    toSpeak = Translator.Instance.Translate("grandpastory-scene5", TranslationCategory.MiniGames);
                }
                else if (___scene == 6)
                {
                    if (___grandpaSpeechTimer > 3000)
                    {
                        if (ClickableGrandpaLetterRect(___parallaxPan, ___grandpaSpeechTimer).Contains(x, y))
                        {
                            toSpeak = Translator.Instance.Translate("grandpastory-letteropen", TranslationCategory.MiniGames);
                        }
                        else if (___letterView == null)
                        {
                            Point pos = ClickableGrandpaLetterRect(___parallaxPan, ___grandpaSpeechTimer).Center;
                            Game1.setMousePositionRaw((int)((float)pos.X * Game1.options.zoomLevel),
                                (int)((float)pos.Y * Game1.options.zoomLevel));
                            return;
                        }
                    }
                    else
                    {
                        toSpeak = Translator.Instance.Translate("grandpastory-scene6", TranslationCategory.MiniGames);
                    }
                }

                if (grandpaStoryQuery == toSpeak) return;
                grandpaStoryQuery = toSpeak;
                
                MainClass.ScreenReader.Say(toSpeak, false);
            }
            catch (System.Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        // This method is taken from the game's source code
        private static Rectangle ClickableGrandpaLetterRect(int ___parallaxPan, int ___grandpaSpeechTimer)
        {
            return new Rectangle(
                (int)Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730).X +
                (286 - ___parallaxPan) * 4,
                (int)Utility.getTopLeftPositionForCenteringOnScreen(Game1.viewport, 1294, 730).Y + 218 +
                Math.Max(0, Math.Min(60, (___grandpaSpeechTimer - 5000) / 8)), 524, 344);
        }
    }
}