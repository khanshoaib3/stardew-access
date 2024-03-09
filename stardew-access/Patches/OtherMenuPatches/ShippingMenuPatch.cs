using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    // TODO Speak per category money distribution info
    internal class ShippingMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(ShippingMenu), nameof(ShippingMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ShippingMenuPatch), nameof(ShippingMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(ShippingMenu __instance, List<int> ___categoryTotals)
        {
            try
            {
                if (__instance.currentPage != -1)
                    return;

                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                // Perform Left Click
                if (MainClass.Config.LeftClickMainKey.JustPressed() || MainClass.Config.LeftClickAlternateKey.JustPressed())
                {
                    Game1.activeClickableMenu.receiveLeftClick(x, y);
                }

                int total = ___categoryTotals[5];
                string toSpeak;
                object? translationTokens = null;
                if (__instance.okButton.containsPoint(x, y))
                {
                    toSpeak = "menu-shipping-total_money_received_info";
                    translationTokens = new
                    {
                        money = total
                    };
                    MainClass.ScreenReader.TranslateAndSayWithMenuChecker(toSpeak, true, translationTokens);
                    return;
                }

                for (int i = 0; i < __instance.categories.Count; i++)
                {
                    if (!__instance.categories[i].containsPoint(x, y))
                        continue;

                    toSpeak = "menu-shipping-money_received_from_category_info";
                    translationTokens = new
                    {
                        category_name = __instance.getCategoryName(i),
                        money = ___categoryTotals[i]
                    };
                    MainClass.ScreenReader.TranslateAndSayWithMenuChecker(toSpeak, true, translationTokens);
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in shipping menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
