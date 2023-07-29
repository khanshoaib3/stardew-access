namespace stardew_access.Patches
{
    internal class CollectionsPagePatch
    {
        internal static void DrawPatch(StardewValley.Menus.CollectionsPage __instance)
        {
            try
            {
                int x = StardewValley.Game1.getMousePosition().X, y = StardewValley.Game1.getMousePosition().Y;
                if (__instance.letterviewerSubMenu != null)
                {
                    LetterViwerMenuPatch.NarrateLetterContent(__instance.letterviewerSubMenu);
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
