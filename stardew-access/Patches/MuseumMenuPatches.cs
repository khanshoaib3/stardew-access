using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class MuseumMenuPatches
    {
        private static string museumQueryKey = " ";
        private static bool isMoving = false;

        internal static bool MuseumMenuKeyPressPatch()
        {
            try
            {
                if (isMoving)
                    return false;

                if (!isMoving)
                {
                    isMoving = true;
                    Task.Delay(200).ContinueWith(_ => { isMoving = false; });
                }

            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }

            return true;
        }

        internal static void MuseumMenuPatch(MuseumMenu __instance, bool ___holdingMuseumPiece)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (__instance.heldItem != null)
                {
                    // Museum Inventory
                    string toSpeak = " ";
                    int tileX = (int)(Utility.ModifyCoordinateFromUIScale(x) + (float)Game1.viewport.X) / 64;
                    int tileY = (int)(Utility.ModifyCoordinateFromUIScale(y) + (float)Game1.viewport.Y) / 64;
                    LibraryMuseum libraryMuseum = (LibraryMuseum)Game1.currentLocation;

                    if (libraryMuseum.isTileSuitableForMuseumPiece(tileX, tileY))
                        toSpeak = $"slot {tileX}x {tileY}y";

                    if (museumQueryKey != toSpeak)
                    {
                        museumQueryKey = toSpeak;
                        MainClass.GetScreenReader().Say(toSpeak, true);
                    }
                }
                else
                {
                    // Player Inventory
                    if (!narrateHoveredItemInInventory(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y))
                    {
                        if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                        {
                            if (museumQueryKey != $"ok button")
                            {
                                museumQueryKey = $"ok button";
                                MainClass.GetScreenReader().Say("ok button", true);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static bool narrateHoveredItemInInventory(InventoryMenu inventoryMenu, List<ClickableComponent> inventory, IList<Item> actualInventory, int x, int y)
        {
            #region Narrate hovered item
            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i].containsPoint(x, y))
                {
                    string toSpeak = "";
                    if ((i + 1) <= actualInventory.Count)
                    {
                        if (actualInventory[i] != null)
                        {
                            string name = actualInventory[i].DisplayName;
                            int stack = actualInventory[i].Stack;
                            string quality = "";

                            #region Add quality of item
                            if (actualInventory[i] is StardewValley.Object && ((StardewValley.Object)actualInventory[i]).quality > 0)
                            {
                                int qualityIndex = ((StardewValley.Object)actualInventory[i]).quality;
                                if (qualityIndex == 1)
                                {
                                    quality = "Silver quality";
                                }
                                else if (qualityIndex == 2 || qualityIndex == 3)
                                {
                                    quality = "Gold quality";
                                }
                                else if (qualityIndex >= 4)
                                {
                                    quality = "Iridium quality";
                                }
                            }
                            #endregion

                            if (inventoryMenu.highlightMethod(inventoryMenu.actualInventory[i]))
                                name = $"Donatable {name}";

                            if (stack > 1)
                                toSpeak = $"{stack} {name} {quality}";
                            else
                                toSpeak = $"{name} {quality}";
                        }
                        else
                        {
                            // For empty slot
                            toSpeak = "Empty Slot";
                        }
                    }
                    else
                    {
                        // For empty slot
                        toSpeak = "Empty Slot";
                    }

                    if (museumQueryKey != $"{toSpeak}:{i}")
                    {
                        museumQueryKey = $"{toSpeak}:{i}";
                        MainClass.GetScreenReader().Say(toSpeak, true);
                    }
                    return true;
                }
            }
            #endregion
            return false;
        }
    }
}