using HarmonyLib;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class ShippingMenuPatch : IPatch
{
    private static int previousTab = -1;

    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(ShippingMenu), "draw"),
            postfix: new HarmonyMethod(typeof(ShippingMenuPatch), nameof(ShippingMenuPatch.DrawPatch))
        );
    }

    private static void DrawPatch(ShippingMenu __instance, List<int> ___categoryTotals, List<List<Item>> ___categoryItems, Dictionary<Item, int> ___itemValues, Dictionary<Item, int> ___singleItemValues, bool ___outro, bool ___newDayPlaque)
    {
        try
        {
            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

            MouseUtils.SimulateMouseClicks((x, y) => __instance.receiveLeftClick(x, y),
                    (x, y) => __instance.receiveRightClick(x, y));

            if (___outro)
            {
                if (___newDayPlaque)
                    MainClass.ScreenReader.SayWithMenuChecker(Utility.getDateString(), true);
                previousTab = -1;
                return;
            }

            if (__instance.currentPage == -1)
            {
                previousTab = -1;

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
            else
            {
                // Speak category wise profit break down
                List<string> displayedItems = new();
                for (int i = __instance.currentTab * __instance.itemsPerCategoryPage; i < __instance.currentTab * __instance.itemsPerCategoryPage + __instance.itemsPerCategoryPage; i++)
                {
                    if (___categoryItems[__instance.currentPage].Count <= i)
                        continue;
                    Item item = ___categoryItems[__instance.currentPage][i];
                    string singleVal = Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", ___singleItemValues[item]);
                    string totalVal = Game1.content.LoadString("Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020", ___itemValues[item]);
                    displayedItems.Add($"{InventoryUtils.GetItemDetails(item)} x{singleVal} = {totalVal}");
                }

                if (MainClass.Config.PrimaryInfoKey.JustPressed())
                {
                    MainClass.ScreenReader.Say(string.Join(", ", displayedItems), true);
                    return;
                }

                string displayedItemsString = __instance.currentTab != previousTab ? string.Join(", ", displayedItems) + "\n" : "";
                string hoveredButton = "";

                if (__instance.backButton is { visible: true } && __instance.backButton.containsPoint(x, y))
                {
                    hoveredButton = Translator.Instance.Translate(__instance.currentTab is not 0 ? "common-ui-previous_page_button" : "common-ui-back_button", translationCategory: TranslationCategory.Menu);
                }
                else if (__instance.forwardButton is { visible: true } && __instance.showForwardButton() && __instance.forwardButton.containsPoint(x, y))
                {
                    hoveredButton = Translator.Instance.Translate("common-ui-next_page_button", translationCategory: TranslationCategory.Menu);
                }

                if (MainClass.ScreenReader.SayWithMenuChecker(displayedItemsString + hoveredButton, true, customQuery: hoveredButton))
                {
                    previousTab = __instance.currentTab;
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in shipping menu patch:\n{e.Message}\n{e.StackTrace}");
        }
    }
}
