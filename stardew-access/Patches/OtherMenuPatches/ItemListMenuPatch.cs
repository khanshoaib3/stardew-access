using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ItemListMenuPatch
    {
        private static string itemListMenuQuery = "";

        internal static void DrawPatch(ItemListMenu __instance, string ___title, int ___currentTab, int ___totalValueOfItems, List<Item> ___itemsToList)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                string toSpeak = "", currentList = "";

                for (int i = ___currentTab * __instance.itemsPerCategoryPage; i < ___currentTab * __instance.itemsPerCategoryPage + __instance.itemsPerCategoryPage; i++)
                {
                    if (i == 0) currentList = ___title;
                    if (___itemsToList.Count <= i) continue;
                    
                    if (___itemsToList[i] == null)
                    {
                        currentList = $"{currentList}, \n" + Game1.content.LoadString("Strings\\UI:ItemList_ItemsLostValue", ___totalValueOfItems);
                        continue;
                    }

                    currentList = $"{currentList}, \n {___itemsToList[i].Stack} {___itemsToList[i].DisplayName}";
                }

                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                    toSpeak = $"Page {___currentTab + 1} of {((int)___itemsToList.Count / __instance.itemsPerCategoryPage) + 1} \n {currentList} \n ok button";
                else if (__instance.forwardButton != null && __instance.forwardButton.containsPoint(x, y))
                    toSpeak = "Next page button";
                else if (__instance.backButton != null && __instance.backButton.containsPoint(x, y))
                    toSpeak = "Previous page button";

                if (itemListMenuQuery != toSpeak)
                {
                    itemListMenuQuery = toSpeak;
                    MainClass.ScreenReader.Say(toSpeak, true);
                }
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void Cleanup()
        {
            itemListMenuQuery = "";
        }
    }
}
