using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class TitleMenuPatch
    {
        private static string titleMenuQueryKey = "";

        internal static void DrawPatch(TitleMenu __instance, bool ___isTransitioningButtons)
        {
            try
            {
                if (___isTransitioningButtons)
                    return;

                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                string toSpeak = "";

                if (__instance.muteMusicButton.containsPoint(x, y))
                {
                    toSpeak = "Mute Music Button";
                }
                else if (__instance.aboutButton.containsPoint(x, y))
                {
                    toSpeak = "About Button";
                }
                else if (__instance.languageButton.containsPoint(x, y))
                {
                    toSpeak = "Language Button";
                }
                else if (__instance.windowedButton.containsPoint(x, y))
                {
                    toSpeak = "Fullscreen: " + ((Game1.isFullscreen) ? "enabled" : "disabled");
                }
                else if (TitleMenu.subMenu != null && __instance.backButton.containsPoint(x, y))
                {
                    string text = "Back Button";
                    MainClass.ScreenReader.SayWithChecker(text, true);
                }
                else
                {
                    __instance.buttons.ForEach(component =>
                    {
                        if (!component.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                            return;

                        string name = component.name;
                        string label = component.label;
                        toSpeak = $"{name} {label} Button";
                    });
                }

                // Fix for back button not working using keyboard
                if (TitleMenu.subMenu is CharacterCustomization && ((CharacterCustomization)TitleMenu.subMenu).backButton.containsPoint(x, y))
                {
                    // Perform Left Click
                    if (MainClass.Config.LeftClickMainKey.JustPressed())
                    {
                        __instance.backButtonPressed();
                    }
                }

                if (TitleMenu.subMenu == null && titleMenuQueryKey!=toSpeak)
                {
                    titleMenuQueryKey = toSpeak;
                    MainClass.ScreenReader.Say(toSpeak, true);
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"An error occured in title menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
        
        internal static void Cleanup()
        {
            titleMenuQueryKey = "";
        }
    }
}
