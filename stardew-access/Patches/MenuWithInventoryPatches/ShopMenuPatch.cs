using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ShopMenuPatch
    {
        internal static string shopMenuQueryKey = "";
        internal static string hoveredItemQueryKey = "";

        internal static void DrawPatch(ShopMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (MainClass.Config.SnapToFirstSecondaryInventorySlotKey.JustPressed() && __instance.forSaleButtons.Count > 0)
                {
                    __instance.forSaleButtons[0].snapMouseCursorToCenter();
                    __instance.setCurrentlySnappedComponentTo(__instance.forSaleButtons[0].myID);
                }
                else if (MainClass.Config.SnapToFirstInventorySlotKey.JustPressed() && __instance.inventory.inventory.Count > 0)
                {
                    __instance.inventory.inventory[0].snapMouseCursorToCenter();
                    __instance.setCurrentlySnappedComponentTo(__instance.inventory.inventory[0].myID);
                }

                if (NarrateHoveredButton(__instance, x, y)) return;

                if (InventoryUtils.NarrateHoveredSlot(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y, hoverPrice: __instance.hoverPrice))
                {
                    shopMenuQueryKey = "";
                    return;
                }

                NarrateHoveredSellingItem(__instance);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static bool NarrateHoveredButton(ShopMenu __instance, int x, int y)
        {
            string toSpeak = "";
            bool isDropItemButton = false;

            if (__instance.inventory.dropItemInvisibleButton != null && __instance.inventory.dropItemInvisibleButton.containsPoint(x, y))
            {
                toSpeak = "Drop Item";
                isDropItemButton = true;
            }
            else if (__instance.upArrow != null && __instance.upArrow.containsPoint(x, y))
            {
                toSpeak = "Up Arrow Button";
            }
            else if (__instance.downArrow != null && __instance.downArrow.containsPoint(x, y))
            {
                toSpeak = "Down Arrow Button";
            }
            else
            {
                return false;
            }

            if (shopMenuQueryKey == toSpeak) return true;

            shopMenuQueryKey = toSpeak;
            hoveredItemQueryKey = "";
            MainClass.ScreenReader.Say(toSpeak, true);
            if (isDropItemButton) Game1.playSound("drop_item");

            return true;
        }

        private static void NarrateHoveredSellingItem(ShopMenu __instance)
        {
            if (__instance.hoveredItem == null) return;

            string name = __instance.hoveredItem.DisplayName;
            string price = $"Buy Price: {__instance.hoverPrice} g";
            string description = __instance.hoveredItem.getDescription();
            string requirements = "";

            #region get required items for item
            int itemIndex = -1, itemAmount = 5;

            if (__instance.itemPriceAndStock[__instance.hoveredItem].Length > 2)
                itemIndex = __instance.itemPriceAndStock[__instance.hoveredItem][2];

            if (__instance.itemPriceAndStock[__instance.hoveredItem].Length > 3)
                itemAmount = __instance.itemPriceAndStock[__instance.hoveredItem][3];

            if (itemIndex != -1)
            {
                string itemName = Game1.objectInformation[itemIndex].Split('/')[0];

                if (itemAmount != -1)
                    requirements = $"Required: {itemAmount} {itemName}";
                else
                    requirements = $"Required: {itemName}";
            }
            #endregion

            string toSpeak = $"{name}, {requirements}, {price}, \n\t{description}";
            if (shopMenuQueryKey == toSpeak) return;

            shopMenuQueryKey = toSpeak;
            hoveredItemQueryKey = "";
            MainClass.ScreenReader.Say(toSpeak, true);
        }

        internal static void Cleanup()
        {
            shopMenuQueryKey = "";
            hoveredItemQueryKey = "";
        }
    }
}
