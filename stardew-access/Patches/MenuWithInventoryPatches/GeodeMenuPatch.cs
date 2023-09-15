using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class GeodeMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(GeodeMenu), "draw"),
                postfix: new HarmonyMethod(typeof(GeodeMenuPatch), nameof(GeodeMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(GeodeMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (NarrateReceivedTreasure(__instance)) return;
                if (NarrateHoveredButton(__instance, x, y)) return;

                InventoryUtils.NarrateHoveredSlot(__instance.inventory.inventory, __instance.inventory.actualInventory,
                    __instance.inventory);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in geode menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static bool NarrateReceivedTreasure(GeodeMenu __instance)
        {
            // Narrates the treasure received on breaking the geode
            if (__instance.geodeTreasure == null) return false;

            string pluralizedName = InventoryUtils.GetPluralNameOfItem(__instance.geodeTreasure);

            MainClass.ScreenReader.TranslateAndSayWithMenuChecker(
                "menu-geode-received_treasure_info",
                true,
                new { treasure_name = pluralizedName });
            return true;
        }

        private static bool NarrateHoveredButton(GeodeMenu __instance, int x, int y)
        {
            string translationKey = "";
            bool isDropItemButton = false;

            if (__instance.geodeSpot != null && __instance.geodeSpot.containsPoint(x, y))
            {
                translationKey = "menu-geode-geode_input_slot";
            }
            else if (__instance.dropItemInvisibleButton != null &&
                     __instance.dropItemInvisibleButton.containsPoint(x, y))
            {
                translationKey = "common-ui-drop_item_button";
                isDropItemButton = true;
            }
            else if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
            {
                translationKey = "common-ui-trashcan_button";
            }
            else if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
            {
                translationKey = "common-ui-ok_button";
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
    }
}