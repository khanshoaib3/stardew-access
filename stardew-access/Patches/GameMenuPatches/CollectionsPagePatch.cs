using HarmonyLib;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class CollectionsPagePatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(CollectionsPage), "draw"),
            postfix: new HarmonyMethod(typeof(CollectionsPagePatch), nameof(CollectionsPagePatch.DrawPatch))
        );
    }

    internal static void DrawPatch(CollectionsPage __instance)
    {
        try
        {
            int x = StardewValley.Game1.getMousePosition().X, y = StardewValley.Game1.getMousePosition().Y;
            if (__instance.letterviewerSubMenu != null)
            {
                LetterViewerMenuPatch.NarrateMenu(__instance.letterviewerSubMenu);
            }
        }
        catch (System.Exception e)
        {
            Log.Error($"An error occurred in collections page patch:\n{e.Message}\n{e.StackTrace}");
        }
    }
}
