using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class InventoryPagePatch : IPatch
    {
        internal static string inventoryPageQueryKey = "";
        internal static string hoveredItemQueryKey = "";

        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(InventoryPagePatch), nameof(InventoryPagePatch.DrawPatch))
            );
        }

        private static void DrawPatch(InventoryPage __instance)
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
                MainClass.ErrorLog($"An error occurred in inventory page patch:\n{e.Message}\n{e.StackTrace}");
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

            string toSpeak = Translator.Instance.Translate("menu-inventory_page-money_info_key",
                    new {
                        farm_name = farmName,
                        current_funds = currentFunds,
                        total_earnings = totalEarnings,
                        festival_score = festivalScore,
                        golden_walnut_count = walnut,
                        qi_gem_count = qiGems,
                        qi_club_coins = qiCoins
                    });


            MainClass.ScreenReader.Say(toSpeak, true);
        }

        private static bool NarrateHoveredButton(InventoryPage __instance, int x, int y)
        {
            string? toSpeak = null;
            bool isDropItemButton = false;

            if (__instance.inventory.dropItemInvisibleButton != null && __instance.inventory.dropItemInvisibleButton.containsPoint(x, y))
            {
                toSpeak = Translator.Instance.Translate("common-ui-drop_item_button");
                isDropItemButton = true;
            }
            else if (__instance.organizeButton != null && __instance.organizeButton.containsPoint(x, y))
            {
                toSpeak = Translator.Instance.Translate("common-ui-organize_inventory_button");
            }
            else if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
            {
                toSpeak = Translator.Instance.Translate("common-ui-trashcan_button");
            }
            else if (__instance.junimoNoteIcon != null && __instance.junimoNoteIcon.containsPoint(x, y))
            {
                toSpeak = Translator.Instance.Translate("common-ui-community_center_button");
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

                string toSpeak = Translator.Instance.Translate(GetNameAndDescriptionOfItem(__instance.equipmentIcons[i].name), true);

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
            "Hat" => (Game1.player.hat.Value != null) ? $"{Game1.player.hat.Value.DisplayName}, {Game1.player.hat.Value.getDescription()}" : "menu-inventory_page-hat_slot",
            "Left Ring" => (Game1.player.leftRing.Value != null) ? $"{Game1.player.leftRing.Value.DisplayName}, {Game1.player.leftRing.Value.getDescription()}" : "menu-inventory_page-left_ring_slot",
            "Right Ring" => (Game1.player.rightRing.Value != null) ? $"{Game1.player.rightRing.Value.DisplayName}, {Game1.player.rightRing.Value.getDescription()}" : "menu-inventory_page-right_ring_slot",
            "Boots" => (Game1.player.boots.Value != null) ? $"{Game1.player.boots.Value.DisplayName}, {Game1.player.boots.Value.getDescription()}" : "menu-inventory_page-boots_slot",
            "Shirt" => (Game1.player.shirtItem.Value != null) ? $"{Game1.player.shirtItem.Value.DisplayName}, {Game1.player.shirtItem.Value.getDescription()}" : "menu-inventory_page-shirt_slot",
            "Pants" => (Game1.player.pantsItem.Value != null) ? $"{Game1.player.pantsItem.Value.DisplayName}, {Game1.player.pantsItem.Value.getDescription()}" : "menu-inventory_page-pants_slot",
            _ => "common-unknown"
        };

        internal static void Cleanup()
        {
            InventoryUtils.Cleanup();
            inventoryPageQueryKey = "";
            hoveredItemQueryKey = "";
        }
    }
}
