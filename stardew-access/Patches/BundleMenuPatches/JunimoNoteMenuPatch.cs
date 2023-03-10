using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class JunimoNoteMenuPatch
    {
        internal static string junimoNoteMenuQuery = "";
        internal static string currentJunimoArea = "";
        internal static bool isUsingCustomKeyBinds = false;
        internal static int currentIngredientListItem = -1, currentIngredientInputSlot = -1, currentInventorySlot = -1;

        internal static void DrawPatch(JunimoNoteMenu __instance, bool ___specificBundlePage, int ___whichArea, Bundle ___currentPageBundle)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (narrateJunimoArea(__instance, ___specificBundlePage, ___whichArea, x, y))
                {
                    return;
                }

                narrateBundlePage(__instance, ___specificBundlePage, ___currentPageBundle, x, y);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static bool narrateJunimoArea(JunimoNoteMenu __instance, bool ___specificBundlePage, int ___whichArea, int x, int y)
        {
            if (___specificBundlePage)
                return false;

            currentIngredientListItem = -1;
            isUsingCustomKeyBinds = false;

            string areaName = __instance.scrambledText ? CommunityCenter.getAreaEnglishDisplayNameFromNumber(___whichArea) : CommunityCenter.getAreaDisplayNameFromNumber(___whichArea);
            string reward = __instance.getRewardNameForArea(___whichArea);

            if (__instance.scrambledText)
            {
                string scrambledText = "Scrambled Text";
                if (junimoNoteMenuQuery != scrambledText)
                {
                    junimoNoteMenuQuery = scrambledText;
                    MainClass.ScreenReader.Say(scrambledText, true);
                }
                return true;
            }

            if (currentJunimoArea != areaName)
            {
                currentJunimoArea = areaName;
                MainClass.ScreenReader.Say($"Area {areaName}, {reward}", true);
                return true;
            }

            string toSpeak = "";
            if (__instance.presentButton != null && __instance.presentButton.containsPoint(x, y))
            {
                toSpeak = "Present Button";
            }
            else if (__instance.fromGameMenu && __instance.areaNextButton.visible && __instance.areaNextButton.containsPoint(x, y))
            {
                toSpeak = "Next Area Button";
            }
            else if (__instance.fromGameMenu && __instance.areaBackButton.visible && __instance.areaBackButton.containsPoint(x, y))
            {
                toSpeak = "Previous Area Button";
            }
            else
            {
                for (int i = 0; i < __instance.bundles.Count; i++)
                {
                    if (!__instance.bundles[i].containsPoint(x, y))
                        continue;

                    toSpeak = $"{__instance.bundles[i].name} bundle";
                    break;
                }
            }

            if (junimoNoteMenuQuery != toSpeak)
            {
                junimoNoteMenuQuery = toSpeak;
                MainClass.ScreenReader.Say(toSpeak, true);
                return true;
            }

            return false;
        }

        private static void narrateBundlePage(JunimoNoteMenu __instance, bool ___specificBundlePage, Bundle ___currentPageBundle, int x, int y)
        {
            if (!___specificBundlePage)
                return;

            bool isIPressed = MainClass.Config.BundleMenuIngredientsKey.JustPressed(); // For the ingredients
            bool isCPressed = MainClass.Config.BundleMenuInventoryItemsKey.JustPressed(); // For the items in inventory
            bool isPPressed = MainClass.Config.BundleMenuPurchaseButtonKey.JustPressed(); // For the Purchase Button
            bool isVPressed = MainClass.Config.BundleMenuIngredientsInputSlotKey.JustPressed(); // For the ingredient input slots
            bool isBackPressed = MainClass.Config.BundleMenuBackButtonKey.JustPressed(); // For the back button
            bool isLeftShiftPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);

            if (isIPressed && !isUsingCustomKeyBinds)
            {
                isUsingCustomKeyBinds = true;
                cycleThroughIngredientList(__instance, ___currentPageBundle, isLeftShiftPressed);
                Task.Delay(200).ContinueWith(_ => { isUsingCustomKeyBinds = false; });
            }
            else if (isVPressed && !isUsingCustomKeyBinds)
            {
                isUsingCustomKeyBinds = true;
                cycleThroughInputSlots(__instance, ___currentPageBundle, isLeftShiftPressed);
                Task.Delay(200).ContinueWith(_ => { isUsingCustomKeyBinds = false; });
            }
            else if (isCPressed && !isUsingCustomKeyBinds)
            {
                isUsingCustomKeyBinds = true;
                cycleThroughInventorySlots(__instance, ___currentPageBundle, isLeftShiftPressed);
                Task.Delay(200).ContinueWith(_ => { isUsingCustomKeyBinds = false; });
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
            return;
        }

        private static void cycleThroughIngredientList(JunimoNoteMenu __instance, Bundle ___currentPageBundle, bool isLeftShiftPressed = false)
        {
            if (___currentPageBundle.ingredients.Count < 0)
                return;

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

            if (completed)
            {
                toSpeak = $"Completed {toSpeak}";
            }
            else
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

            c.snapMouseCursorToCenter();
            MainClass.ScreenReader.Say(toSpeak, true);
        }

        private static void cycleThroughInputSlots(JunimoNoteMenu __instance, Bundle ___currentPageBundle, bool isLeftShiftPressed = false)
        {
            if (__instance.ingredientSlots.Count < 0)
                return;

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

        private static void cycleThroughInventorySlots(JunimoNoteMenu __instance, Bundle ___currentPageBundle, bool isLeftShiftPressed = false)
        {
            if (__instance.inventory == null || __instance.inventory.actualInventory.Count < 0)
                return;

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

                if (!__instance.inventory.highlightMethod(__instance.inventory.actualInventory[currentInventorySlot]))
                {
                    toSpeak = $"{toSpeak} not usable here";
                }
            }
            else
            {
                toSpeak = "Empty Slot";
            }
            c.snapMouseCursorToCenter();
            MainClass.ScreenReader.Say(toSpeak, true);
        }

        internal static void Cleanup()
        {
            JunimoNoteMenuPatch.currentIngredientListItem = -1;
            JunimoNoteMenuPatch.currentIngredientInputSlot = -1;
            JunimoNoteMenuPatch.currentInventorySlot = -1;
            JunimoNoteMenuPatch.junimoNoteMenuQuery = "";
        }
    }
}
