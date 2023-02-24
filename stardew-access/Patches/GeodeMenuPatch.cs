using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class GeodeMenuPatch
    {
        internal static string geodeMenuQueryKey = "";

        internal static void DrawPatch(GeodeMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                #region Narrate the treasure recieved on breaking the geode
                if (__instance.geodeTreasure != null)
                {
                    string name = __instance.geodeTreasure.DisplayName;
                    int stack = __instance.geodeTreasure.Stack;

                    string toSpeak = $"Recieved {stack} {name}";

                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                    return;
                }
                #endregion

                #region Narrate hovered buttons in the menu
                if (__instance.geodeSpot != null && __instance.geodeSpot.containsPoint(x, y))
                {
                    string toSpeak = "Place geode here";
                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
                {
                    string toSpeak = "Drop item here";

                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        MainClass.ScreenReader.Say(toSpeak, true);
                        Game1.playSound("drop_item");
                    }
                    return;
                }

                if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
                {
                    string toSpeak = "Trash can";

                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                    return;
                }

                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                {
                    string toSpeak = "Ok button";

                    if (geodeMenuQueryKey != toSpeak)
                    {
                        geodeMenuQueryKey = toSpeak;
                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                    return;
                }
                #endregion

                #region Narrate hovered item
                if (InventoryUtils.narrateHoveredSlot(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y))
                    geodeMenuQueryKey = "";
                #endregion
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void Cleanup()
        {
            geodeMenuQueryKey = "";
        }
    }
}
