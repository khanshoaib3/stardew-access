using HarmonyLib;
using stardew_access.Translation;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

// TODO Maybe add keybind to cycle through unlocked powers and keybinds to snap cursor to forward/backward buttons
internal class PowersTabPatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(PowersTab), "draw"),
            postfix: new HarmonyMethod(typeof(PowersTabPatch), nameof(PowersTabPatch.DrawPatch))
        );
    }

    private static void DrawPatch(PowersTab __instance, string ___hoverText, string ___descriptionText)
    {
        try
        {
            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

            foreach (ClickableTextureComponent item in __instance.powers[__instance.currentPage])
            {
                if (!item.containsPoint(x, y)) continue;

                if (item.drawShadow)
                    MainClass.ScreenReader.SayWithMenuChecker($"{___hoverText}\n{___descriptionText}".Trim(), true);
                else
                    MainClass.ScreenReader.SayWithMenuChecker(Translator.Instance.Translate("common-unknown"), true);
                return;
            }

            if (__instance.backButton is { visible: true } && __instance.backButton.containsPoint(x, y))
            {
                MainClass.ScreenReader.TranslateAndSayWithMenuChecker("common-ui-previous_page_button", true);
                return;
            }

            if (__instance.forwardButton is { visible: true } && __instance.forwardButton.containsPoint(x, y))
            {
                MainClass.ScreenReader.TranslateAndSayWithMenuChecker("common-ui-next_page_button", true);
                return;
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in powers tab patch:\n{e.Message}\n{e.StackTrace}");
        }
    }
}
