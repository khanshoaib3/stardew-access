using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class InventoryPagePatch
    {
        internal static string inventoryPageQueryKey = "";
        internal static string hoveredItemQueryKey = "";

        internal static void DrawPatch(InventoryPage __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                HandleKeyBinds();

                if (NarrateHoveredButton(__instance, x, y))
                {
                    return;
                }

                if (NarrateHoveredEquipmentSlot(__instance, x, y))
                {
                    return;
                }

                if (InventoryUtils.NarrateHoveredSlot(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y, true))
                {
                    inventoryPageQueryKey = "";
                    return;
                }

                // If no slot or button is hovered
                Cleanup();
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"An error occurred in InventoryPagePatch()->DrawPatch():\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void HandleKeyBinds()
        {
            if (!MainClass.Config.MoneyKey.JustPressed())
                return;

            string farmName = Game1.content.LoadString("Strings\\UI:Inventory_FarmName", Game1.player.farmName.Value);
            string currentFunds = Game1.content.LoadString("Strings\\UI:Inventory_CurrentFunds" + (Game1.player.useSeparateWallets ? "_Separate" : ""), Utility.getNumberWithCommas(Game1.player.Money));
            string totalEarnings = Game1.content.LoadString("Strings\\UI:Inventory_TotalEarnings" + (Game1.player.useSeparateWallets ? "_Separate" : ""), Utility.getNumberWithCommas((int)Game1.player.totalMoneyEarned));
            int festivalScore = Game1.player.festivalScore;
            int walnut = Game1.netWorldState.Value.GoldenWalnuts.Value;
            int qiGems = Game1.player.QiGems;
            int qiCoins = Game1.player.clubCoins;

            string toSpeak = $"{farmName}\n{currentFunds}\n{totalEarnings}";

            if (festivalScore > 0)
                toSpeak = $"{toSpeak}\nFestival Score: {festivalScore}";

            if (walnut > 0)
                toSpeak = $"{toSpeak}\nGolden Walnut: {walnut}";

            if (qiGems > 0)
                toSpeak = $"{toSpeak}\nQi Gems: {qiGems}";

            if (qiCoins > 0)
                toSpeak = $"{toSpeak}\nQi Club Coins: {qiCoins}";

            MainClass.ScreenReader.Say(toSpeak, true);
        }

        private static bool NarrateHoveredButton(InventoryPage __instance, int x, int y)
        {
            string? toSpeak = null;
            bool isDropItemButton = false;

            if (__instance.inventory.dropItemInvisibleButton != null && __instance.inventory.dropItemInvisibleButton.containsPoint(x, y))
            {
                toSpeak = "Drop Item";
                isDropItemButton = true;
            }
            else if (__instance.organizeButton != null && __instance.organizeButton.containsPoint(x, y))
            {
                toSpeak = "Organize Inventory Button";
            }
            else if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
            {
                toSpeak = "Trash Can";
            }
            else if (__instance.organizeButton != null && __instance.organizeButton.containsPoint(x, y))
            {
                toSpeak = "Organize Button";
            }
            else if (__instance.junimoNoteIcon != null && __instance.junimoNoteIcon.containsPoint(x, y))
            {
                toSpeak = "Community Center Button";
            }
            else
            {
                return false;
            }

            if (toSpeak != null && inventoryPageQueryKey != toSpeak)
            {
                inventoryPageQueryKey = toSpeak;
                hoveredItemQueryKey = "";
                MainClass.ScreenReader.Say(toSpeak, true);
                if (isDropItemButton) Game1.playSound("drop_item");
            }

            return true;
        }

        private static bool NarrateHoveredEquipmentSlot(InventoryPage __instance, int mouseX, int mouseY)
        {
            for (int i = 0; i < __instance.equipmentIcons.Count; i++)
            {
                if (!__instance.equipmentIcons[i].containsPoint(mouseX, mouseY))
                    continue;

                string toSpeak = GetNameAndDescriptionOfItem(__instance.equipmentIcons[i].name);

                if (inventoryPageQueryKey != toSpeak)
                {
                    inventoryPageQueryKey = toSpeak;
                    hoveredItemQueryKey = "";
                    MainClass.ScreenReader.Say(toSpeak, true);
                }

                return true;
            }

            return false;
        }

        private static string GetNameAndDescriptionOfItem(string slotName) => slotName switch
        {
            "Hat" => (Game1.player.hat.Value != null) ? $"{Game1.player.hat.Value.DisplayName}, {Game1.player.hat.Value.getDescription()}" : "Hat slot",
            "Left Ring" => (Game1.player.leftRing.Value != null) ? $"{Game1.player.leftRing.Value.DisplayName}, {Game1.player.leftRing.Value.getDescription()}" : "Left Ring slot",
            "Right Ring" => (Game1.player.rightRing.Value != null) ? $"{Game1.player.rightRing.Value.DisplayName}, {Game1.player.rightRing.Value.getDescription()}" : "Right ring slot",
            "Boots" => (Game1.player.boots.Value != null) ? $"{Game1.player.boots.Value.DisplayName}, {Game1.player.boots.Value.getDescription()}" : "Boots slot",
            "Shirt" => (Game1.player.shirtItem.Value != null) ? $"{Game1.player.shirtItem.Value.DisplayName}, {Game1.player.shirtItem.Value.getDescription()}" : "Shirt slot",
            "Pants" => (Game1.player.pantsItem.Value != null) ? $"{Game1.player.pantsItem.Value.DisplayName}, {Game1.player.pantsItem.Value.getDescription()}" : "Pants slot",
            _ => "unkown slot"
        };

        internal static void Cleanup()
        {
            InventoryUtils.Cleanup();
            inventoryPageQueryKey = "";
            hoveredItemQueryKey = "";
        }
    }
}
