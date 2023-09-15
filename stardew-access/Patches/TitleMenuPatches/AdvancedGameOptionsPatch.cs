using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class AdvancedGameOptionsPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(AdvancedGameOptions), nameof(AdvancedGameOptions.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(AdvancedGameOptionsPatch), nameof(AdvancedGameOptionsPatch.DrawPatch))
            );
        }

        private static void DrawPatch(AdvancedGameOptions __instance)
        {
            try
            {
                int currentItemIndex = Math.Max(0, Math.Min(__instance.options.Count - 7, __instance.currentItemIndex));
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);

                // FIXME The menu auto focuses on ok button when it is opened which causes the menu to automatically close when it is opened. Possible solution: add a delay on menu open.
                MouseUtils.SimulateMouseClicks(
                    (x, y) => __instance.receiveLeftClick(x, y),
                    (x, y) => __instance.receiveRightClick(x, y)
                );

                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                {
                    MainClass.ScreenReader.TranslateAndSayWithMenuChecker("common-ui-ok_button", true);
                    return;
                }

                OptionsElementUtils.NarrateOptionsElementSlots(__instance.optionSlots, __instance.options, currentItemIndex);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in advanced game menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
