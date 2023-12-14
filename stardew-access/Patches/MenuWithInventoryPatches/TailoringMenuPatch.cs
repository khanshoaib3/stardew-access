using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class TailoringMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(TailoringMenu), nameof(TailoringMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(TailoringMenuPatch), nameof(TailoringMenuPatch.DrawPatch))
            );
        }

        internal static void DrawPatch(TailoringMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (InventoryUtils.NarrateHoveredSlot(__instance.inventory))
                    return;

                NarrateHoveredButton(__instance, x, y);
            }
            catch (System.Exception e)
            {
                Log.Error($"An error occurred in tailoring menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static bool NarrateHoveredButton(TailoringMenu __instance, int x, int y)
        {
            string translationKey = "";
            object? translationTokens = null;
            bool isDropItemButton = false;

            if (__instance.leftIngredientSpot != null && __instance.leftIngredientSpot.containsPoint(x, y))
            {
                translationKey = "menu-tailoring-cloth_input_slot";
                Item? item = __instance.leftIngredientSpot.item;
                translationTokens = new
                {
                    is_empty = (item == null) ? 1 : 0,
                    item_name = (item == null) ? "" : InventoryUtils.GetPluralNameOfItem(item)
                };
            }
            else if (__instance.rightIngredientSpot != null && __instance.rightIngredientSpot.containsPoint(x, y))
            {
                translationKey = "menu-tailoring-spool_slot";
                Item? item = __instance.rightIngredientSpot.item;
                translationTokens = new
                {
                    is_empty = (item == null) ? 1 : 0,
                    item_name = (item == null) ? "" : InventoryUtils.GetPluralNameOfItem(item)
                };
            }
            else if (__instance.startTailoringButton != null && __instance.startTailoringButton.containsPoint(x, y))
            {
                translationKey = "menu-tailoring-start_tailoring_button";
            }
            else if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
            {
                translationKey = "common-ui-trashcan_button";
            }
            else if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
            {
                translationKey = "common-ui-ok_button";
            }
            else if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
            {
                translationKey = "common-ui-drop_item_button";
                isDropItemButton = true;
            }
            else if (__instance.equipmentIcons.Count > 0 && __instance.equipmentIcons[0].containsPoint(x, y))
            {
                translationKey = "common-ui-equipment_slots";
                Item? item = Game1.player.hat.Value;
                translationTokens = new
                {
                    slot_name = "hat",
                    is_empty = (item == null) ? 1 : 0,
                    item_name = (item == null) ? "" : InventoryUtils.GetPluralNameOfItem(item),
                    item_description = ""
                };
            }
            else if (__instance.equipmentIcons.Count > 0 && __instance.equipmentIcons[1].containsPoint(x, y))
            {
                translationKey = "common-ui-equipment_slots";
                Item? item = Game1.player.shirtItem.Value;
                translationTokens = new
                {
                    slot_name = "shirt",
                    is_empty = (item == null) ? 1 : 0,
                    item_name = (item == null) ? "" : InventoryUtils.GetPluralNameOfItem(item),
                    item_description = ""
                };
            }
            else if (__instance.equipmentIcons.Count > 0 && __instance.equipmentIcons[2].containsPoint(x, y))
            {
                translationKey = "common-ui-equipment_slots";
                Item? item = Game1.player.pantsItem.Value;
                translationTokens = new
                {
                    slot_name = "pants",
                    is_empty = (item == null) ? 1 : 0,
                    item_name = (item == null) ? "" : InventoryUtils.GetPluralNameOfItem(item),
                    item_description = ""
                };
            }
            else {
                return false;
            }

            if (MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens))
                if (isDropItemButton) Game1.playSound("drop_item");

            return true;
        }
    }
}
