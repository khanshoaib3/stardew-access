using HarmonyLib;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class ShopMenuPatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(ShopMenu), "draw"),
            postfix: new HarmonyMethod(typeof(ShopMenuPatch), nameof(ShopMenuPatch.DrawPatch))
        );
    }

    private static void DrawPatch(ShopMenu __instance)
    {
        try
        {
            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

            if (MainClass.Config.SnapToFirstSecondaryInventorySlotKey.JustPressed() &&
                __instance.forSaleButtons.Count > 0)
            {
                __instance.forSaleButtons[0].snapMouseCursorToCenter();
                __instance.setCurrentlySnappedComponentTo(__instance.forSaleButtons[0].myID);
            }
            else if (MainClass.Config.SnapToFirstInventorySlotKey.JustPressed() &&
                     __instance.inventory.inventory.Count > 0)
            {
                __instance.inventory.inventory[0].snapMouseCursorToCenter();
                __instance.setCurrentlySnappedComponentTo(__instance.inventory.inventory[0].myID);
            }

            if (NarrateHoveredButton(__instance, x, y)) return;

            if (InventoryUtils.NarrateHoveredSlot(__instance.inventory, hoverPrice: __instance.hoverPrice))
            {
                return;
            }

            NarrateHoveredSellingItem(__instance);
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in shop menu patch:\n{e.Message}\n{e.StackTrace}");
        }
    }

    private static bool NarrateHoveredButton(ShopMenu __instance, int x, int y)
    {
        string translationKey = "";
        bool isDropItemButton = false;

        if (__instance.inventory.dropItemInvisibleButton != null &&
            __instance.inventory.dropItemInvisibleButton.containsPoint(x, y))
        {
            translationKey = "common-ui-drop_item_button";
            isDropItemButton = true;
        }
        else if (__instance.upArrow != null && __instance.upArrow.containsPoint(x, y))
        {
            translationKey = "common-ui-scroll_up_button";
        }
        else if (__instance.downArrow != null && __instance.downArrow.containsPoint(x, y))
        {
            translationKey = "common-ui-scroll_down_button";
        }
        else
        {
            return false;
        }

        if (MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true))
            if (isDropItemButton)
                Game1.playSound("drop_item");

        return true;
    }

    private static void NarrateHoveredSellingItem(ShopMenu __instance)
    {
        if (__instance.hoveredItem == null) return;

        string name = __instance.hoveredItem.DisplayName;
        string price = __instance.hoverPrice <= 0 ? ""
            : Translator.Instance.Translate("menu-shop-buy_price_info", new { price = __instance.hoverPrice }, TranslationCategory.Menu);
        string description = __instance.hoveredItem.IsRecipe
            ? new CraftingRecipe(__instance.hoveredItem.Name.Replace(" Recipe", "")).description
            : __instance.hoveredItem.getDescription();

        if (__instance.ShopId is Game1.shop_petAdoption)
        {
            string petType = __instance.hoveredItem.Name.Split("_")[0].ToLower();
            int breed = int.Parse(__instance.hoveredItem.Name.Split("_")[1]);
            breed++;

            name = Translator.Instance.Translate($"menu-character_creation-description-{petType}", translationCategory: TranslationCategory.CharacterCreationMenu, tokens: new
            {
                breed,
                less_info = 0
            });

            name = Translator.Instance.Translate($"menu-shop-pet_license-suffix", translationCategory: TranslationCategory.Menu, tokens: new
            {
                content = name
            });
        }

        string itemId = __instance.itemPriceAndStock[__instance.hoveredItem].TradeItem;
        int? itemCount = __instance.itemPriceAndStock[__instance.hoveredItem].TradeItemCount;

        string requirements = InventoryUtils.GetExtraItemInfo(itemId, itemCount);
        string healthAndStamina = InventoryUtils.GetHealthNStaminaFromItem(__instance.hoveredItem as Item);
        string buffs = InventoryUtils.GetBuffsFromItem(__instance.hoveredItem.QualifiedItemId);

        string ingredients = !__instance.hoveredItem.IsRecipe ? ""
            : InventoryUtils.GetIngredientsFromRecipe(new CraftingRecipe(__instance.hoveredItem.Name.Replace(" Recipe", "")));
        ingredients = string.IsNullOrWhiteSpace(ingredients) ? ""
            : Translator.Instance.Translate("menu-shop-recipe_ingredients_info", translationCategory: TranslationCategory.Menu, tokens: new { ingredients_list = ingredients });

        string toSpeak = string.Join(", ",
            new string[] { name, requirements, price, description, healthAndStamina, buffs, ingredients }.Where(c => !string.IsNullOrEmpty(c)));

        MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true, customQuery: $"{__instance.currentItemIndex}:{__instance.currentlySnappedComponent.myID}:{toSpeak}");
    }
}
