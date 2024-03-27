using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ItemGrabMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(ItemGrabMenu), nameof(ItemGrabMenu.draw),
                    new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ItemGrabMenuPatch), nameof(ItemGrabMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(ItemGrabMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (MainClass.Config.SnapToFirstSecondaryInventorySlotKey.JustPressed() &&
                    __instance.ItemsToGrabMenu.inventory.Count > 0 && !__instance.shippingBin)
                {
                    __instance.setCurrentlySnappedComponentTo(__instance.ItemsToGrabMenu.inventory[0].myID);
                    __instance.ItemsToGrabMenu.inventory[0].snapMouseCursorToCenter();
                }
                else if (MainClass.Config.SnapToFirstInventorySlotKey.JustPressed() &&
                         __instance.inventory.inventory.Count > 0)
                {
                    __instance.setCurrentlySnappedComponentTo(__instance.inventory.inventory[0].myID);
                    __instance.inventory.inventory[0].snapMouseCursorToCenter();
                }

                if (NarrateHoveredButton(__instance, x, y))
                {
                    return;
                }

                if (NarrateLastShippedItem(__instance, x, y))
                {
                    return;
                }

                // Player inventory
                if (InventoryUtils.NarrateHoveredSlot(__instance.inventory, giveExtraDetails: true))
                {
                    return;
                }

                // Other inventory
                InventoryUtils.NarrateHoveredSlot(__instance.ItemsToGrabMenu, giveExtraDetails: true);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in item grab menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static bool NarrateHoveredButton(ItemGrabMenu __instance, int x, int y)
        {
            string translationKey = "";
            object? translationTokens = null;
            bool isDropItemButton = false;

            if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
            {
                translationKey = "common-ui-ok_button";
            }
            else if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
            {
                translationKey = "common-ui-trashcan_button";
            }
            else if (__instance.organizeButton != null && __instance.organizeButton.containsPoint(x, y))
            {
                translationKey = "common-ui-organize_inventory_button";
            }
            else if (__instance.fillStacksButton != null && __instance.fillStacksButton.containsPoint(x, y))
            {
                translationKey = "menu-item_grab-add_to_existing_stack_button";
            }
            else if (__instance.specialButton != null && __instance.specialButton.containsPoint(x, y))
            {
                translationKey = "menu-item_grab-special_button";
            }
            else if (__instance.colorPickerToggleButton != null &&
                     __instance.colorPickerToggleButton.containsPoint(x, y))
            {
                translationKey = "menu-item_grab-color_picker_button";
                translationTokens = new
                {
                    is_enabled = __instance.chestColorPicker.visible ? 1 : 0
                };
            }
            else if (__instance.junimoNoteIcon != null && __instance.junimoNoteIcon.containsPoint(x, y))
            {
                translationKey = "common-ui-community_center_button";
            }
            else if (__instance.dropItemInvisibleButton != null &&
                     __instance.dropItemInvisibleButton.containsPoint(x, y))
            {
                translationKey = "common-ui-drop_item_button";
                isDropItemButton = true;
            }
            else
            {
                for (int i = 0;
                     __instance.discreteColorPickerCC != null && i < __instance.discreteColorPickerCC.Count;
                     i++)
                {
                    if (!__instance.discreteColorPickerCC[i].containsPoint(x, y))
                        continue;

                    MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-item_grab-chest_colors", true, new
                    {
                        index = i,
                        is_selected = i == __instance.chestColorPicker.colorSelection ? 1 : 0
                    });

                    return true;
                }

                return false;
            }

            if (!MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens)) return true;
            if (isDropItemButton) Game1.playSound("drop_item");

            return true;
        }

        private static bool NarrateLastShippedItem(ItemGrabMenu __instance, int x, int y)
        {
            if (!__instance.shippingBin || Game1.getFarm().lastItemShipped == null ||
                !__instance.lastShippedHolder.containsPoint(x, y))
                return false;

            Item lastShippedItem = Game1.getFarm().lastItemShipped;
            string pluralizedName = InventoryUtils.GetPluralNameOfItem(lastShippedItem);

            MainClass.ScreenReader.TranslateAndSayWithMenuChecker(
                "menu-item_grab-last_shipped_info",
                true,
                new { shipped_item_name = pluralizedName });
            return true;
        }
    }
}
