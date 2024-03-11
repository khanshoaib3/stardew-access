using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class NamingMenuPatch : IPatch
{
    private static bool _firstTimeInNamingMenu = true;
    private static string? _previousName = null;

    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(NamingMenu), "draw"),
            postfix: new HarmonyMethod(typeof(NamingMenuPatch), nameof(DrawPatch))
        );
    }

    private static void DrawPatch(NamingMenu __instance, TextBox ___textBox, string ___title)
    {
        try
        {
            if (_firstTimeInNamingMenu)
            {
                _firstTimeInNamingMenu = false;
                ___textBox.Selected = false;
            }

            if (TextBoxPatch.IsAnyTextBoxActive) return;

            string translationKey = "";
            object? translationTokens = null;
            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
            if (!string.IsNullOrEmpty(___textBox.Text) && ___textBox.Text != _previousName)
            {    
                MainClass.ScreenReader.Say(___textBox.Text, true);
                _previousName = ___textBox.Text;
                return;
            }

            if (__instance.textBoxCC != null && __instance.textBoxCC.containsPoint(x, y))
            {
                translationKey = $"options_element-text_box_info";
                translationTokens = new
                {
                    label = ___title,
                    value = string.IsNullOrEmpty(___textBox.Text) ? "null" : ___textBox.Text,
                };
            }
            else if (__instance.doneNamingButton != null && __instance.doneNamingButton.containsPoint(x, y))
            {
                translationKey = "menu-naming-done_naming_button";
            }
            else if (__instance.randomButton != null && __instance.randomButton.containsPoint(x, y))
            {
                translationKey = "menu-naming-random_button";
            }

            MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens);
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in naming menu patch:\n{e.Message}\n{e.StackTrace}");
        }
    }

    internal static void Cleanup()
    {
        _firstTimeInNamingMenu = true;
        _previousName = null;
    }
}