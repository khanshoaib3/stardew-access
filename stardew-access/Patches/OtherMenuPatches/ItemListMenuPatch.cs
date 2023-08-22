using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ItemListMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(ItemListMenu), nameof(ItemListMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ItemListMenuPatch), nameof(ItemListMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(ItemListMenu __instance, string ___title, int ___currentTab, int ___totalValueOfItems, List<Item> ___itemsToList)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                string translationKey = "";
                object? translationTokens = null;

                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                {
                    List<string> currentList = new();

                    for (int i = ___currentTab * __instance.itemsPerCategoryPage; i < ___currentTab * __instance.itemsPerCategoryPage + __instance.itemsPerCategoryPage; i++)
                    {
                        if (___itemsToList.Count <= i) continue;

                        currentList.Add((___itemsToList[i] == null)
                                ? Game1.content.LoadString("Strings\\UI:ItemList_ItemsLostValue", ___totalValueOfItems)
                                : InventoryUtils.GetPluralNameOfItem(___itemsToList[i]));
                    }

                    translationKey = "menu-item_list-ok_button";
                    translationTokens = new
                    {
                        title = ___title,
                        active_tab = ___currentTab + 1,
                        total_tabs = ((int)___itemsToList.Count / __instance.itemsPerCategoryPage) + 1,
                        item_list = string.Join(", ", currentList)
                    };
                }
                else if (__instance.forwardButton != null && __instance.forwardButton.containsPoint(x, y))
                {
                    translationKey = "common-ui-next_page_button";
                }
                else if (__instance.backButton != null && __instance.backButton.containsPoint(x, y))
                {
                    translationKey = "common-ui-previous_page_button";
                }

                MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens);
            }
            catch (System.Exception e)
            {
                Log.Error($"An error occurred in item list menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
