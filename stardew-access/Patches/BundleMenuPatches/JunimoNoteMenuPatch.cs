using stardew_access.Utils;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using stardew_access.Translation;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

namespace stardew_access.Patches
{
    internal class JunimoNoteMenuPatch : IPatch
    {
        internal static bool firstTimeInMenu = true;
        internal static bool isUsingCustomKeyBinds = false;
        internal static int currentIngredientListItem = -1,
            currentIngredientInputSlot = -1,
            currentInventorySlot = -1;

        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(JunimoNoteMenu), nameof(JunimoNoteMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(JunimoNoteMenuPatch), nameof(JunimoNoteMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(
            JunimoNoteMenu __instance,
            bool ___specificBundlePage,
            int ___whichArea,
            Bundle ___currentPageBundle
        )
        {
            try
            {
                int x = Game1.getMouseX(true),
                    y = Game1.getMouseY(true); // Mouse x and y position

                if (NarrateJunimoArea(__instance, ___specificBundlePage, ___whichArea, x, y))
                {
                    return;
                }

                NarrateBundlePage(__instance, ___specificBundlePage, ___currentPageBundle, x, y);
            }
            catch (Exception e)
            {
                Log.Error( $"An error occurred in Junimo Note Menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static bool NarrateJunimoArea(
            JunimoNoteMenu __instance,
            bool ___specificBundlePage,
            int ___whichArea,
            int x,
            int y
        )
        {
            if (___specificBundlePage)
                return false;

            currentIngredientListItem = -1;
            isUsingCustomKeyBinds = false;

            string areaName = __instance.scrambledText
                ? CommunityCenter.getAreaEnglishDisplayNameFromNumber(___whichArea)
                : CommunityCenter.getAreaDisplayNameFromNumber(___whichArea);
            string reward = __instance.getRewardNameForArea(___whichArea);

            if (__instance.scrambledText)
            {
                string scrambledText = Translator.Instance.Translate("menu-junimo_note-scrambled_text");
                MainClass.ScreenReader.SayWithMenuChecker(scrambledText, true);
                return true;
            }

            if (firstTimeInMenu)
            {
                firstTimeInMenu = false;
                MainClass.ScreenReader.MenuPrefixNoQueryText = Translator.Instance.Translate(
                    "menu-junimo_note-current_area_info-prefix",
                    new { area_name = areaName, completion_reward = reward }
                );
            }

            string toSpeak = "";
            if (__instance.presentButton != null && __instance.presentButton.containsPoint(x, y))
            {
                toSpeak = Translator.Instance.Translate("menu-junimo_note-collect_rewards");
            }
            else if ( __instance.fromGameMenu && __instance.areaNextButton.visible
                && __instance.areaNextButton.containsPoint(x, y)
            )
            {
                toSpeak = Translator.Instance.Translate("menu-junimo_note-next_area_button");
            }
            else if ( __instance.fromGameMenu && __instance.areaBackButton.visible
                && __instance.areaBackButton.containsPoint(x, y)
            )
            {
                toSpeak = Translator.Instance.Translate("menu-junimo_note-previous_area_button");
            }
            else
            {
                for (int i = 0; i < __instance.bundles.Count; i++)
                {
                    if (!__instance.bundles[i].containsPoint(x, y))
                        continue;

                    toSpeak = Translator.Instance.Translate(
                        "menu-junimo_note-bundle_open_button",
                        new { bundle_name = __instance.bundles[i].name }
                    );
                    break;
                }
            }

            MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);

            return !string.IsNullOrWhiteSpace(toSpeak);
        }

        private static void NarrateBundlePage(
            JunimoNoteMenu __instance,
            bool ___specificBundlePage,
            Bundle ___currentPageBundle,
            int x,
            int y
        )
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
                CycleThroughIngredientList(__instance, ___currentPageBundle, isLeftShiftPressed);
                Task.Delay(200).ContinueWith(_ =>
                    {
                        isUsingCustomKeyBinds = false;
                    });
            }
            else if (isVPressed && !isUsingCustomKeyBinds)
            {
                isUsingCustomKeyBinds = true;
                CycleThroughInputSlots(__instance, ___currentPageBundle, isLeftShiftPressed);
                Task.Delay(200).ContinueWith(_ =>
                    {
                        isUsingCustomKeyBinds = false;
                    });
            }
            else if (isCPressed && !isUsingCustomKeyBinds)
            {
                isUsingCustomKeyBinds = true;
                CycleThroughInventorySlots(__instance, ___currentPageBundle, isLeftShiftPressed);
                Task.Delay(200).ContinueWith(_ =>
                    {
                        isUsingCustomKeyBinds = false;
                    });
            }
            else if ( isBackPressed && __instance.backButton != null
                && !__instance.backButton.containsPoint(x, y))
            {
                __instance.backButton.snapMouseCursorToCenter();
                MainClass.ScreenReader.Say(Translator.Instance.Translate("menu-junimo_note-back_button"), true);
            }
            else if ( isPPressed && __instance.purchaseButton != null
                && !__instance.purchaseButton.containsPoint(x, y))
            {
                __instance.purchaseButton.snapMouseCursorToCenter();
                MainClass.ScreenReader.Say(Translator.Instance.Translate("menu-junimo_note-purchase_button"), true);
            }
            return;
        }

        private static void CycleThroughIngredientList(
            JunimoNoteMenu __instance,
            Bundle ___currentPageBundle,
            bool isLeftShiftPressed = false
        )
        {
            if (___currentPageBundle.ingredients.Count < 0)
                return;

            currentIngredientListItem += (isLeftShiftPressed ? -1 : 1);
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
            BundleIngredientDescription ingredient = ___currentPageBundle.ingredients[
                currentIngredientListItem
            ];

            Item item = new StardewValley.Object( ingredient.index, ingredient.stack, isRecipe: false, -1, ingredient.quality); bool completed = false;
            if (___currentPageBundle != null && ___currentPageBundle.ingredients != null
                && currentIngredientListItem < ___currentPageBundle.ingredients.Count
                && ___currentPageBundle.ingredients[currentIngredientListItem].completed)
            {
                completed = true;
            }

            string toSpeak = item.DisplayName;

            if (completed)
            {
                toSpeak = Translator.Instance.Translate(
                    "menu-bundle-completed-prefix",
                    new { content = toSpeak }
                );
            }
            else
            {
                toSpeak = $"{InventoryUtils.GetPluralNameOfItem(item)}, {InventoryUtils.GetQualityFromIndex(ingredient.quality)}";
            }

            c.snapMouseCursorToCenter();
            MainClass.ScreenReader.Say(toSpeak, true);
        }

        private static void CycleThroughInputSlots(
            JunimoNoteMenu __instance,
            Bundle ___currentPageBundle,
            bool isLeftShiftPressed = false
        )
        {
            if (__instance.ingredientSlots.Count < 0)
                return;

            currentIngredientInputSlot += (isLeftShiftPressed ? -1 : 1);
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
                toSpeak = Translator.Instance.Translate(
                    "menu-junimo_note-input_slot",
                    new { index = currentIngredientInputSlot + 1 }
                );
            }
            else
            {
                toSpeak = item.DisplayName;
            }

            c.snapMouseCursorToCenter();
            MainClass.ScreenReader.Say(toSpeak, true);
        }

        private static void CycleThroughInventorySlots(
            JunimoNoteMenu __instance,
            Bundle ___currentPageBundle,
            bool isLeftShiftPressed = false
        )
        {
            if (__instance.inventory == null || __instance.inventory.actualInventory.Count < 0)
                return;

            int prevSlotIndex = currentInventorySlot;
            currentInventorySlot += (isLeftShiftPressed ? -1 : 1);
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
            InventoryUtils.NarrateHoveredSlot(
                __instance.inventory.inventory,
                __instance.inventory.actualInventory,
                __instance.inventory,
                hoverX: c.bounds.Center.X,
                hoverY: c.bounds.Center.Y
            );
            c.snapMouseCursorToCenter();
        }

        internal static void Cleanup()
        {
            currentIngredientListItem = -1;
            currentIngredientInputSlot = -1;
            currentInventorySlot = -1;
            firstTimeInMenu = true;
        }
    }
}
