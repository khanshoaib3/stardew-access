using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class SocialPagePatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.Method(typeof(SocialPage), nameof(SocialPage.draw),
                new Type[] { typeof(SpriteBatch) }),
            postfix: new HarmonyMethod(typeof(SocialPagePatch), nameof(SocialPagePatch.DrawPatch))
        );
    }

    private static void DrawPatch(SocialPage __instance, List<ClickableTextureComponent> ___sprites,
        int ___slotPosition, List<string> ___kidsNames)
    {
        try
        {
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in social page patch:\n{e.Message}\n{e.StackTrace}");
        }
    }
}
