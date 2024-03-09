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
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.draw),
                    new Type[] { typeof(SpriteBatch) }),
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

                if (InventoryUtils.NarrateHoveredSlot(__instance.inventory, giveExtraDetails: true))
                {
                    return;
                }
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in inventory page patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void HandleKeyBinds()
        {
            if (!MainClass.Config.MoneyKey.JustPressed())
                return;

            string farmName = Game1.content.LoadString("Strings\\UI:Inventory_FarmName", Game1.player.farmName.Value);
            string currentFunds = Game1.content.LoadString(
                "Strings\\UI:Inventory_CurrentFunds" + (Game1.player.useSeparateWallets ? "_Separate" : ""),
                Utility.getNumberWithCommas(Game1.player.Money));
            string totalEarnings = Game1.content.LoadString(
                "Strings\\UI:Inventory_TotalEarnings" + (Game1.player.useSeparateWallets ? "_Separate" : ""),
                Utility.getNumberWithCommas((int)Game1.player.totalMoneyEarned));
            int festivalScore = Game1.player.festivalScore;
            int walnut = Game1.netWorldState.Value.GoldenWalnuts;
            int qiGems = Game1.player.QiGems;
            int qiCoins = Game1.player.clubCoins;


            MainClass.ScreenReader.TranslateAndSay("menu-inventory_page-money_info_key", true, new
                {
                    farm_name = farmName,
                    current_funds = currentFunds,
                    total_earnings = totalEarnings,
                    festival_score = festivalScore,
                    golden_walnut_count = walnut,
                    qi_gem_count = qiGems,
                    qi_club_coins = qiCoins
                },
                TranslationCategory.Menu);
        }

        private static bool NarrateHoveredButton(InventoryPage __instance, int x, int y)
        {
            string? translationKey = null;
            bool isDropItemButton = false;

            if (__instance.inventory.dropItemInvisibleButton != null &&
                __instance.inventory.dropItemInvisibleButton.containsPoint(x, y))
            {
                translationKey = "common-ui-drop_item_button";
                isDropItemButton = true;
            }
            else if (__instance.organizeButton != null && __instance.organizeButton.containsPoint(x, y))
            {
                translationKey = "common-ui-organize_inventory_button";
            }
            else if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
            {
                translationKey = "common-ui-trashcan_button";
            }
            else if (__instance.junimoNoteIcon != null && __instance.junimoNoteIcon.containsPoint(x, y))
            {
                translationKey = "common-ui-community_center_button";
            }
            else
            {
                return false;
            }

            if (!MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true)) return true;
            if (isDropItemButton) Game1.playSound("drop_item");

            return true;
        }

        private static bool NarrateHoveredEquipmentSlot(InventoryPage __instance, int mouseX, int mouseY)
        {
            for (int i = 0; i < __instance.equipmentIcons.Count; i++)
            {
                if (!__instance.equipmentIcons[i].containsPoint(mouseX, mouseY))
                    continue;

                MainClass.ScreenReader.SayWithMenuChecker(
                    GetNameAndDescriptionOfItem(__instance.equipmentIcons[i].name.ToLower().Replace(" ", "_")), true);

                return true;
            }

            return false;
        }

        private static string GetNameAndDescriptionOfItem(string slotName)
        {
            Item? item = slotName switch
            {
                "hat" => Game1.player.hat.Value,
                "left_ring" => Game1.player.leftRing.Value,
                "right_ring" => Game1.player.rightRing.Value,
                "boots" => Game1.player.boots.Value,
                "shirt" => Game1.player.shirtItem.Value,
                "pants" => Game1.player.pantsItem.Value,
                _ => null
            };

            return Translator.Instance.Translate(
                "common-ui-equipment_slots",
                new
                {
                    slot_name = slotName,
                    is_empty = (item == null) ? 1 : 0,
                    item_name = (item == null) ? "" : item.DisplayName,
                    item_description = (item == null) ? "" : item.getDescription()
                },
                TranslationCategory.Menu);
        }
    }
}