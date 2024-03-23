using HarmonyLib;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class BuildingSkinMenuPatch : IPatch
{
    private static BuildingSkinMenu.SkinEntry? currentSkin = null;

    public void Apply(Harmony harmony)
    {
        harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(BuildingSkinMenu), "draw"),
                prefix: new HarmonyMethod(typeof(BuildingSkinMenuPatch), nameof(BuildingSkinMenuPatch.DrawPatch))
        );
    }

    private static void DrawPatch(BuildingSkinMenu __instance)
    {
        try
        {
            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
            if (__instance.GetParentMenu() != null)
                MouseUtils.SimulateMouseClicks((x, y) => __instance.receiveLeftClick(x, y), (x, y) => __instance.receiveRightClick(x, y));

            if (currentSkin != __instance.Skin || MainClass.Config.PrimaryInfoKey.JustPressed())
            {
                currentSkin = __instance.Skin;
                MainClass.ScreenReader.MenuPrefixNoQueryText = Translator.Instance.Translate("menu-building_skin-skin_info", translationCategory: TranslationCategory.Menu, tokens: new
                {
                    type = __instance.Building.buildingType.Value.ToLower().Trim().Replace(" ", "_"),
                    index = __instance.Skin.Index,
                    id = string.IsNullOrWhiteSpace(__instance.Skin.Id) ? __instance.Building.buildingType.Value : __instance.Skin.Id // Basically the english display name
                });
                MainClass.ScreenReader.PrevMenuQueryText = "";
            }

            if (__instance.NextSkinButton is { visible: true } && __instance.NextSkinButton.containsPoint(x, y))
            {
                MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-building_skin-next_skin_button", true);
            }
            else if (__instance.PreviousSkinButton is { visible: true } && __instance.PreviousSkinButton.containsPoint(x, y))
            {
                MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-building_skin-previous_skin_button", true);
            }
            else if (__instance.OkButton is { visible: true } && __instance.OkButton.containsPoint(x, y))
            {
                MainClass.ScreenReader.TranslateAndSayWithMenuChecker("common-ui-ok_button", true);
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in building skin menu patch:\n{e.Message}\n{e.StackTrace}");
        }
    }
}
