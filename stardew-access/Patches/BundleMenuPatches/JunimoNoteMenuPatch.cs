using stardew_access.Utils;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.TokenizableStrings;
using stardew_access.Translation;
using HarmonyLib;

namespace stardew_access.Patches;

internal class JunimoNoteMenuPatch : IPatch
{
    internal static bool firstTimeInMenu = true;
    internal static bool isUsingCustomKeyBinds = false;
    private static bool _announcedBundlePage = false;

    internal static int currentIngredientListItem = -1,
        currentIngredientInputSlot = -1,
        currentInventorySlot = -1;

    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(JunimoNoteMenu), "draw"),
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
            Log.Error($"An error occurred in Junimo Note Menu patch:\n{e.Message}\n{e.StackTrace}");
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
        _announcedBundlePage = false;

        string areaName = __instance.scrambledText
            ? CommunityCenter.getAreaEnglishDisplayNameFromNumber(___whichArea)
            : CommunityCenter.getAreaDisplayNameFromNumber(___whichArea);
        string reward = __instance.getRewardNameForArea(___whichArea);

        if (__instance.scrambledText)
        {
            MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-junimo_note-scrambled_text", true);
            return true;
        }

        if (firstTimeInMenu)
        {
            firstTimeInMenu = false;
            MainClass.ScreenReader.MenuPrefixNoQueryText = Translator.Instance.Translate(
                "menu-junimo_note-current_area_info-prefix",
                new { area_name = areaName, completion_reward = reward },
                TranslationCategory.Menu
            );
        }

        string translationKey = "";
        object? translationToken = null;

        if (__instance.presentButton != null && __instance.presentButton.containsPoint(x, y))
        {
            translationKey = "menu-junimo_note-collect_rewards";
        }
        else if (__instance.fromGameMenu && __instance.areaNextButton.visible
                                         && __instance.areaNextButton.containsPoint(x, y)
                )
        {
            translationKey = "menu-junimo_note-next_area_button";
        }
        else if (__instance.fromGameMenu && __instance.areaBackButton.visible
                                         && __instance.areaBackButton.containsPoint(x, y)
                )
        {
            translationKey = "menu-junimo_note-previous_area_button";
        }
        else
        {
            for (int i = 0; i < __instance.bundles.Count; i++)
            {
                if (!__instance.bundles[i].containsPoint(x, y))
                    continue;

                translationKey = "menu-junimo_note-bundle_open_button";
                // Use localized display name (label), not the internal English name.
                translationToken = new { bundle_name = GetBundleDisplayName(__instance.bundles[i]) };
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(translationKey))
            return false;

        MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationToken);
        return true;
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
        bool isVPressed =
            MainClass.Config.BundleMenuIngredientsInputSlotKey.JustPressed(); // For the ingredient input slots
        bool isBackPressed = MainClass.Config.BundleMenuBackButtonKey.JustPressed(); // For the back button
        bool isLeftShiftPressed =
            Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift);

        if (!_announcedBundlePage)
        {
            _announcedBundlePage = true;
            MainClass.ScreenReader.MenuPrefixNoQueryText = Translator.Instance.Translate(
                "menu-junimo_note-bundle_open_button",
                new { bundle_name = GetBundleDisplayName(___currentPageBundle) },
                TranslationCategory.Menu
            ) + ", ";
        }

        if (isIPressed && !isUsingCustomKeyBinds)
        {
            isUsingCustomKeyBinds = true;
            CycleThroughIngredientList(__instance, ___currentPageBundle, isLeftShiftPressed);
            Task.Delay(200).ContinueWith(_ => { isUsingCustomKeyBinds = false; });
            return;
        }

        if (isVPressed && !isUsingCustomKeyBinds)
        {
            isUsingCustomKeyBinds = true;
            CycleThroughInputSlots(__instance, ___currentPageBundle, isLeftShiftPressed);
            Task.Delay(200).ContinueWith(_ => { isUsingCustomKeyBinds = false; });
            return;
        }

        if (isCPressed && !isUsingCustomKeyBinds)
        {
            isUsingCustomKeyBinds = true;
            CycleThroughInventorySlots(__instance, ___currentPageBundle, isLeftShiftPressed);
            Task.Delay(200).ContinueWith(_ => { isUsingCustomKeyBinds = false; });
            return;
        }

        if (isBackPressed && __instance.backButton != null
                               && !__instance.backButton.containsPoint(x, y))
        {
            __instance.backButton.snapMouseCursorToCenter();
            MainClass.ScreenReader.Say(
                Translator.Instance.Translate("menu-junimo_note-back_button", TranslationCategory.Menu), true);
            return;
        }

        if (isPPressed && __instance.purchaseButton != null
                            && !__instance.purchaseButton.containsPoint(x, y))
        {
            __instance.purchaseButton.snapMouseCursorToCenter();
            MainClass.ScreenReader.Say(
                Translator.Instance.Translate("menu-junimo_note-purchase_button", TranslationCategory.Menu), true);
            return;
        }

        NarrateBundlePageHover(__instance, ___currentPageBundle, x, y);
    }

    private static void NarrateBundlePageHover(
        JunimoNoteMenu __instance,
        Bundle ___currentPageBundle,
        int x,
        int y
    )
    {
        if (__instance.backButton != null && __instance.backButton.containsPoint(x, y))
        {
            MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-junimo_note-back_button", true);
            return;
        }

        if (__instance.purchaseButton != null && __instance.purchaseButton.containsPoint(x, y))
        {
            MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-junimo_note-purchase_button", true);
            return;
        }

        for (int i = 0; i < __instance.ingredientList.Count; i++)
        {
            ClickableTextureComponent ingredient = __instance.ingredientList[i];
            if (!ingredient.containsPoint(x, y))
                continue;

            string toSpeak = !string.IsNullOrWhiteSpace(ingredient.hoverText)
                ? ingredient.hoverText
                : ingredient.item?.DisplayName ?? "";

            if (i < ___currentPageBundle.ingredients.Count)
            {
                BundleIngredientDescription description = ___currentPageBundle.ingredients[i];
                if (description.completed)
                {
                    toSpeak = Translator.Instance.Translate(
                        "menu-bundle-completed-prefix",
                        new { content = toSpeak },
                        TranslationCategory.Menu
                    );
                }
                else if (ingredient.item != null)
                {
                    toSpeak =
                        $"{InventoryUtils.GetPluralNameOfItem(ingredient.item)}, {InventoryUtils.GetQualityFromIndex(description.quality)}";
                }
            }

            if (!string.IsNullOrWhiteSpace(toSpeak))
                MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
            return;
        }

        for (int i = 0; i < __instance.ingredientSlots.Count; i++)
        {
            ClickableTextureComponent slot = __instance.ingredientSlots[i];
            if (!slot.containsPoint(x, y))
                continue;

            string toSpeak = slot.item == null
                ? Translator.Instance.Translate(
                    "menu-junimo_note-input_slot",
                    new { index = i + 1 },
                    TranslationCategory.Menu
                )
                : slot.item.DisplayName;

            MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
            return;
        }

        if (__instance.inventory != null
            && !___currentPageBundle.complete
            && ___currentPageBundle.completionTimer <= 0)
        {
            InventoryUtils.NarrateHoveredSlot(__instance.inventory);
        }
    }

    private static string GetBundleDisplayName(Bundle bundle)
    {
        string displayName = string.IsNullOrWhiteSpace(bundle.label) ? bundle.name : bundle.label;
        return TokenParser.ParseText(displayName) ?? displayName;
    }

    private static void CycleThroughIngredientList(
        JunimoNoteMenu __instance,
        Bundle ___currentPageBundle,
        bool isLeftShiftPressed = false
    )
    {
        if (___currentPageBundle.ingredients.Count <= 0)
            return;

        if (__instance.ingredientList.Count <= 0)
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
        BundleIngredientDescription ingredient = ___currentPageBundle.ingredients[currentIngredientListItem];
        string representativeItemId = JunimoNoteMenu.GetRepresentativeItemId(ingredient);
        Item item = ingredient.preservesId == null
            ? ItemRegistry.Create(representativeItemId, ingredient.stack, ingredient.quality)
            : Utility.CreateFlavoredItem(representativeItemId, ingredient.preservesId, ingredient.quality, ingredient.stack);

        bool completed = ___currentPageBundle != null && ___currentPageBundle.ingredients != null
                                                      && currentIngredientListItem < ___currentPageBundle.ingredients.Count
                                                      && ___currentPageBundle.ingredients[currentIngredientListItem].completed;

        string toSpeak = item.DisplayName;

        if (completed)
        {
            toSpeak = Translator.Instance.Translate(
                "menu-bundle-completed-prefix",
                new { content = toSpeak },
                TranslationCategory.Menu
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
        if (__instance.ingredientSlots.Count <= 0)
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
                new { index = currentIngredientInputSlot + 1 },
                TranslationCategory.Menu
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
        if (__instance.inventory == null || __instance.inventory.actualInventory.Count <= 0)
            return;

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

        ClickableComponent c = __instance.inventory.inventory[currentInventorySlot];
        InventoryUtils.NarrateHoveredSlot(__instance.inventory,
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
        _announcedBundlePage = false;
    }
}
