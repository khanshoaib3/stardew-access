using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ForgeMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(ForgeMenuPatch), nameof(ForgeMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(ForgeMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (NarrateHoveredButton(__instance, x, y)) return;

                InventoryUtils.NarrateHoveredSlot(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y);

            }
            catch (System.Exception e)
            {
                Log.Error($"An error occurred in forge menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static bool NarrateHoveredButton(ForgeMenu __instance, int x, int y)
        {
            string translationKey = "";
            object? translationTokens = null;
            bool isDropItemButton = false;

            if (__instance.leftIngredientSpot != null && __instance.leftIngredientSpot.containsPoint(x, y))
            {
                translationKey = "menu-forge-weapon_input_slot";
                Item? item = __instance.leftIngredientSpot.item;
                translationTokens = new
                {
                    is_empty = (item == null) ? 1 : 0,
                    item_name = (item == null) ? "" : Translator.Instance.Translate("common-util-pluralize_name",
                            new
                            {
                                item_count = item.Stack,
                                name = item.DisplayName
                            })
                };
            }
            else if (__instance.rightIngredientSpot != null && __instance.rightIngredientSpot.containsPoint(x, y))
            {
                translationKey = "menu-forge-gemstone_input_slot";
                Item? item = __instance.rightIngredientSpot.item;
                translationTokens = new
                {
                    is_empty = (item == null) ? 1 : 0,
                    item_name = (item == null) ? "" : Translator.Instance.Translate("common-util-pluralize_name",
                            new
                            {
                                item_count = item.Stack,
                                name = item.DisplayName
                            })
                };
            }
            else if (__instance.startTailoringButton != null && __instance.startTailoringButton.containsPoint(x, y))
            {
                translationKey = "menu-forge-start_forging_button";
            }
            else if (__instance.unforgeButton != null && __instance.unforgeButton.containsPoint(x, y))
            {
                translationKey = "menu-forge-unforge_button";
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
                Item? item = Game1.player.leftRing.Value;
                translationTokens = new
                {
                    slot_name = "left_ring",
                    is_empty = (item == null) ? 1 : 0,
                    item_name = (item == null) ? "" : Translator.Instance.Translate( "common-util-pluralize_name",
                            new
                            {
                                item_count = item.Stack,
                                name = item.DisplayName
                            }),
                    item_description = ""
                };
            }
            else if (__instance.equipmentIcons.Count > 0 && __instance.equipmentIcons[1].containsPoint(x, y))
            {
                translationKey = "common-ui-equipment_slots";
                Item? item = Game1.player.rightRing.Value;
                translationTokens = new
                {
                    slot_name = "right_ring",
                    is_empty = (item == null) ? 1 : 0,
                    item_name = (item == null) ? "" : Translator.Instance.Translate( "common-util-pluralize_name",
                            new
                            {
                                item_count = item.Stack,
                                name = item.DisplayName
                            }),
                    item_description = ""
                };
            }
            else
            {
                return false;
            }

            if (MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens))
                if (isDropItemButton) Game1.playSound("drop_item");

            return true;
        }
    }
}
