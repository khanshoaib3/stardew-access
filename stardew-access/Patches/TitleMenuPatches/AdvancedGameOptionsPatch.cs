using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class AdvancedGameOptionsPatch : IPatch
    {
        private static bool firstTimeInMenu = true;
        
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

                if (!(__instance.okButton != null && __instance.okButton.containsPoint(x, y) && firstTimeInMenu))
                {
                    MouseUtils.SimulateMouseClicks(
                        (x, y) => __instance.receiveLeftClick(x, y),
                        (x, y) => __instance.receiveRightClick(x, y)
                    );
                } else if (__instance.okButton != null && !__instance.okButton.containsPoint(x, y) && firstTimeInMenu)
                {
                    firstTimeInMenu = false;
                }

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

        public static void Cleanup()
        {
            firstTimeInMenu = true;
        }
    }
}
