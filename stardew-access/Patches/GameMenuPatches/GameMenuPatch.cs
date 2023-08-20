using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class GameMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameMenu), nameof(GameMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(GameMenuPatch), nameof(GameMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(GameMenu __instance)
        {
            try
            {
                // Skip if in map page
                if (__instance.currentTab == 3)
                    return;

                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                for (int i = 0; i < __instance.tabs.Count; i++)
                {
                    if (!__instance.tabs[i].containsPoint(x, y))
                        continue;

                    string toSpeak = Translator.Instance.Translate("menu-game_menu-tab_names",
                            new {
                                tab_name = GameMenu.getLabelOfTabFromIndex(i),
                                is_active = (i == __instance.currentTab) ? 1 : 0
                            });
                    MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in game menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
