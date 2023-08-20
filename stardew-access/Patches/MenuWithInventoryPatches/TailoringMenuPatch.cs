using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class TailoringMenuPatch
    {
        internal static string tailoringMenuQuery = "";

        internal static void DrawPatch(TailoringMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (InventoryUtils.NarrateHoveredSlot(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y))
                    return;


            }
            catch (System.Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static bool NarrateHoveredButton(TailoringMenu __instance, int x, int y)
        {
            string toSpeak = "";
            bool isDropItemButton = false;

            if (__instance.leftIngredientSpot != null && __instance.leftIngredientSpot.containsPoint(x, y))
            {
                if (__instance.leftIngredientSpot.item == null)
                {
                    toSpeak = "Input cloth here";
                }
                else
                {
                    Item item = __instance.leftIngredientSpot.item;
                    toSpeak = $"Cloth slot: {Translator.Instance.Translate("common-util-pluralize_name", new {item_count = item.Stack, name = item.DisplayName})}";
                }
            }
            else if (__instance.rightIngredientSpot != null && __instance.rightIngredientSpot.containsPoint(x, y))
            {
                if (__instance.rightIngredientSpot.item == null)
                {
                    toSpeak = "Input ingredient here";
                }
                else
                {
                    Item item = __instance.rightIngredientSpot.item;
                    toSpeak = $"Ingredient slot: {Translator.Instance.Translate("common-util-pluralize_name", new {item_count = item.Stack, name = item.DisplayName})}";
                }
            }
            else if (__instance.startTailoringButton != null && __instance.startTailoringButton.containsPoint(x, y))
            {
                toSpeak = "Star tailoring button";
            }
            else if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
            {
                toSpeak = "Trashcan";
            }
            else if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
            {
                toSpeak = "ok button";
            }
            else if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
            {
                toSpeak = "drop item";
                isDropItemButton = true;
            }
            else if (__instance.equipmentIcons.Count > 0 && __instance.equipmentIcons[0].containsPoint(x, y))
            {
                toSpeak = "Hat Slot";

                if (Game1.player.hat.Value != null)
                    toSpeak = $"{toSpeak}: {Game1.player.hat.Value.DisplayName}";
            }
            else if (__instance.equipmentIcons.Count > 0 && __instance.equipmentIcons[1].containsPoint(x, y))
            {
                toSpeak = "Shirt Slot";

                if (Game1.player.shirtItem.Value != null)
                    toSpeak = $"{toSpeak}: {Game1.player.shirtItem.Value.DisplayName}";
            }
            else if (__instance.equipmentIcons.Count > 0 && __instance.equipmentIcons[2].containsPoint(x, y))
            {
                toSpeak = "Pants Slot";

                if (Game1.player.pantsItem.Value != null)
                    toSpeak = $"{toSpeak}: {Game1.player.pantsItem.Value.DisplayName}";
            }
            else {
                return false;
            }

            if (tailoringMenuQuery != toSpeak)
            {
                tailoringMenuQuery = toSpeak;
                MainClass.ScreenReader.Say(toSpeak, true);

                if (isDropItemButton) Game1.playSound("drop_item");
            }

            return true;
        }

        internal static void Cleanup()
        {
            tailoringMenuQuery = "";
        }
    }
}
