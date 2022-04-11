using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class BundleMenuPatches
    {
        internal static string junimoNoteMenuQuery = "";
        internal static string jojaCDMenuQuery = "";
        internal static bool isUsingCustomButtons = false;
        internal static int currentIngredientListItem = -1, currentIngredientInputSlot = -1, currentInventorySlot = -1;

        #region Joja Mart Bundle/Quests
        internal static void JojaCDMenuPatch(JojaCDMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                string toSpeak = "";

                for (int i = 0; i < __instance.checkboxes.Count; i++)
                {
                    ClickableComponent c = __instance.checkboxes[i];
                    if (!c.containsPoint(x, y))
                        continue;

                    if (c.name.Equals("complete"))
                    {
                        toSpeak = $"Completed {getNameFromIndex(i)}";
                    }
                    else
                    {
                        toSpeak = $"{getNameFromIndex(i)} Cost: {__instance.getPriceFromButtonNumber(i)}g Description: {__instance.getDescriptionFromButtonNumber(i)}";
                    }

                    break;
                }

                if (jojaCDMenuQuery != toSpeak)
                {
                    jojaCDMenuQuery = toSpeak;
                    MainClass.ScreenReader.Say(toSpeak, true);
                }
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static string getNameFromIndex(int i)
        {
            string name = i switch
            {
                0 => "Bus",
                1 => "Minecarts",
                2 => "Bridge",
                3 => "Greenhouse",
                4 => "Panning",
                _ => "",
            };

            if (name != "")
                return $"{name} Project";
            else
                return "unkown";
        }
        #endregion

        #region Community Center Bundles
        internal static void JunimoNoteMenuPatch(JunimoNoteMenu __instance, bool ___specificBundlePage, int ___whichArea, Bundle ___currentPageBundle)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                if (!___specificBundlePage)
                {
                    currentIngredientListItem = -1;
                    isUsingCustomButtons = false;

                    string areaName = __instance.scrambledText ? CommunityCenter.getAreaEnglishDisplayNameFromNumber(___whichArea) : CommunityCenter.getAreaDisplayNameFromNumber(___whichArea);
                    if (__instance.scrambledText)
                    {
                        string toSpeak = "Scrambled Text";
                        if (junimoNoteMenuQuery != toSpeak)
                        {
                            junimoNoteMenuQuery = toSpeak;
                            MainClass.ScreenReader.Say(toSpeak, true);
                        }
                        return;
                    }
                    for (int i = 0; i < __instance.bundles.Count; i++)
                    {
                        if (__instance.bundles[i].containsPoint(x, y))
                        {
                            string toSpeak = $"{__instance.bundles[i].name} bundle";
                            if (junimoNoteMenuQuery != toSpeak)
                            {
                                junimoNoteMenuQuery = toSpeak;
                                MainClass.ScreenReader.Say(toSpeak, true);
                            }
                            return;
                        }
                    }
                    if (__instance.presentButton != null && __instance.presentButton.containsPoint(x, y))
                    {
                        string toSpeak = "Present Button";
                        if (junimoNoteMenuQuery != toSpeak)
                        {
                            junimoNoteMenuQuery = toSpeak;
                            MainClass.ScreenReader.Say(toSpeak, true);
                        }
                        return;
                    }
                    if (__instance.fromGameMenu)
                    {
                        if (__instance.areaNextButton.visible && __instance.areaNextButton.containsPoint(x, y))
                        {
                            string toSpeak = "Next Area Button";
                            if (junimoNoteMenuQuery != toSpeak)
                            {
                                junimoNoteMenuQuery = toSpeak;
                                MainClass.ScreenReader.Say(toSpeak, true);
                            }
                            return;
                        }
                        if (__instance.areaBackButton.visible && __instance.areaBackButton.containsPoint(x, y))
                        {
                            string toSpeak = "Previous Area Button";
                            if (junimoNoteMenuQuery != toSpeak)
                            {
                                junimoNoteMenuQuery = toSpeak;
                                MainClass.ScreenReader.Say(toSpeak, true);
                            }
                            return;
                        }
                    }
                }
                else
                {
                    bool isIPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.I); // For the ingredients
                    bool isCPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.C); // For the items in inventory
                    bool isPPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.P); // For the Purchase Button
                    bool isVPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.V); // For the ingredient input slots
                    bool isBackPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Back); // For the back button
                    bool isLeftShiftPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);

                    if (isIPressed && !isUsingCustomButtons)
                    {
                        isUsingCustomButtons = true;
                        JunimoNoteCustomButtons(__instance, ___currentPageBundle, 0, isLeftShiftPressed);
                        Task.Delay(200).ContinueWith(_ => { isUsingCustomButtons = false; });
                    }
                    else if (isVPressed && !isUsingCustomButtons)
                    {
                        isUsingCustomButtons = true;
                        JunimoNoteCustomButtons(__instance, ___currentPageBundle, 1, isLeftShiftPressed);
                        Task.Delay(200).ContinueWith(_ => { isUsingCustomButtons = false; });
                    }
                    else if (isCPressed && !isUsingCustomButtons)
                    {
                        isUsingCustomButtons = true;
                        JunimoNoteCustomButtons(__instance, ___currentPageBundle, 2, isLeftShiftPressed);
                        Task.Delay(200).ContinueWith(_ => { isUsingCustomButtons = false; });
                    }
                    else if (isBackPressed && __instance.backButton != null && !__instance.backButton.containsPoint(x, y))
                    {
                        __instance.backButton.snapMouseCursorToCenter();
                        MainClass.ScreenReader.Say("Back Button", true);
                    }
                    else if (isPPressed && __instance.purchaseButton != null && !__instance.purchaseButton.containsPoint(x, y))
                    {
                        __instance.purchaseButton.snapMouseCursorToCenter();
                        MainClass.ScreenReader.Say("Purchase Button", true);
                    }
                }
                string reward = __instance.getRewardNameForArea(___whichArea);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void JunimoNoteCustomButtons(JunimoNoteMenu __instance, Bundle ___currentPageBundle, int signal, bool isLeftShiftPressed = false)
        {
            try
            {

                switch (signal)
                {
                    case 0: // For ingredient list
                        {
                            if (___currentPageBundle.ingredients.Count >= 0)
                            {
                                currentIngredientListItem = currentIngredientListItem + (isLeftShiftPressed ? -1 : 1);
                                if (currentIngredientListItem >= ___currentPageBundle.ingredients.Count)
                                    if (isLeftShiftPressed)
                                        currentIngredientListItem = ___currentPageBundle.ingredients.Count - 1;
                                    else
                                        currentIngredientListItem = 0;

                                if (currentIngredientListItem < 0)
                                    if (isLeftShiftPressed)
                                        currentIngredientListItem = ___currentPageBundle.ingredients.Count - 1;
                                    else
                                        currentIngredientListItem = 0;

                                ClickableTextureComponent c = __instance.ingredientList[currentIngredientListItem];
                                BundleIngredientDescription ingredient = ___currentPageBundle.ingredients[currentIngredientListItem];

                                Item item = new StardewValley.Object(ingredient.index, ingredient.stack, isRecipe: false, -1, ingredient.quality);
                                bool completed = false;
                                if (___currentPageBundle != null && ___currentPageBundle.ingredients != null && currentIngredientListItem < ___currentPageBundle.ingredients.Count && ___currentPageBundle.ingredients[currentIngredientListItem].completed)
                                {
                                    completed = true;
                                }

                                string toSpeak = item.DisplayName;

                                if (!completed)
                                {
                                    int quality = ingredient.quality;
                                    if (quality == 1)
                                    {
                                        toSpeak = $"Silver quality {toSpeak}";
                                    }
                                    else if (quality == 2 || quality == 3)
                                    {
                                        toSpeak = $"Gold quality {toSpeak}";
                                    }
                                    else if (quality >= 4)
                                    {
                                        toSpeak = $"Iridium quality {toSpeak}";
                                    }

                                    toSpeak = $"{ingredient.stack} {toSpeak}";
                                }

                                if (completed)
                                    toSpeak = $"Completed {toSpeak}";

                                c.snapMouseCursorToCenter();
                                MainClass.ScreenReader.Say(toSpeak, true);
                            }
                        }
                        break;
                    case 1: // For input slot list
                        {
                            if (__instance.ingredientSlots.Count >= 0)
                            {
                                currentIngredientInputSlot = currentIngredientInputSlot + (isLeftShiftPressed ? -1 : 1);
                                if (currentIngredientInputSlot >= __instance.ingredientSlots.Count)
                                    if (isLeftShiftPressed)
                                        currentIngredientInputSlot = __instance.ingredientSlots.Count - 1;
                                    else
                                        currentIngredientInputSlot = 0;

                                if (currentIngredientInputSlot < 0)
                                    if (isLeftShiftPressed)
                                        currentIngredientInputSlot = __instance.ingredientSlots.Count - 1;
                                    else
                                        currentIngredientInputSlot = 0;

                                ClickableTextureComponent c = __instance.ingredientSlots[currentIngredientInputSlot];
                                Item item = c.item;
                                string toSpeak;

                                if (item == null)
                                {
                                    toSpeak = $"Input Slot {currentIngredientInputSlot + 1}";
                                }
                                else
                                {
                                    toSpeak = item.DisplayName;
                                }

                                c.snapMouseCursorToCenter();
                                MainClass.ScreenReader.Say(toSpeak, true);
                            }
                        }
                        break;
                    case 2: // For inventory slots
                        {
                            if (__instance.inventory != null && __instance.inventory.actualInventory.Count >= 0)
                            {
                                int prevSlotIndex = currentInventorySlot;
                                currentInventorySlot = currentInventorySlot + (isLeftShiftPressed ? -1 : 1);
                                if (currentInventorySlot >= __instance.inventory.actualInventory.Count)
                                    if (isLeftShiftPressed)
                                        currentInventorySlot = __instance.inventory.actualInventory.Count - 1;
                                    else
                                        currentInventorySlot = 0;

                                if (currentInventorySlot < 0)
                                    if (isLeftShiftPressed)
                                        currentInventorySlot = __instance.inventory.actualInventory.Count - 1;
                                    else
                                        currentInventorySlot = 0;

                                Item item = __instance.inventory.actualInventory[currentInventorySlot];
                                ClickableComponent c = __instance.inventory.inventory[currentInventorySlot];
                                string toSpeak;
                                if (item != null)
                                {
                                    toSpeak = item.DisplayName;

                                    if ((item as StardewValley.Object) != null)
                                    {
                                        int quality = ((StardewValley.Object)item).Quality;
                                        if (quality == 1)
                                        {
                                            toSpeak = $"Silver quality {toSpeak}";
                                        }
                                        else if (quality == 2 || quality == 3)
                                        {
                                            toSpeak = $"Gold quality {toSpeak}";
                                        }
                                        else if (quality >= 4)
                                        {
                                            toSpeak = $"Iridium quality {toSpeak}";
                                        }
                                    }
                                    toSpeak = $"{item.Stack} {toSpeak}";

                                }
                                else
                                {
                                    toSpeak = "Empty Slot";
                                }
                                c.snapMouseCursorToCenter();
                                MainClass.ScreenReader.Say(toSpeak, true);
                            }
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
        #endregion
    }
}