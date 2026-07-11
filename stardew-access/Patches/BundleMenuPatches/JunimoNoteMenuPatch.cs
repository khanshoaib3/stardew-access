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
    private enum BundleSection
    {
        None,
        Inventory,
        Required,
        Deposit
    }

    internal static bool firstTimeInMenu = true;
    internal static bool isUsingCustomKeyBinds = false;
    private static bool _announcedBundlePage = false;
    private static BundleSection _lastSection = BundleSection.None;

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
        _lastSection = BundleSection.None;

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
            string bundleName = Translator.Instance.Translate(
                "menu-junimo_note-bundle_open_button",
                new { bundle_name = GetBundleDisplayName(___currentPageBundle) },
                TranslationCategory.Menu
            );
            string layoutInfo = Translator.Instance.Translate(
                "menu-junimo_note-bundle_layout_info",
                TranslationCategory.Menu
            );
            MainClass.ScreenReader.MenuPrefixNoQueryText = $"{bundleName}, {layoutInfo}, ";
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

        // Legacy/vanilla hover navigation: no repeating section labels.
        NarrateBundlePageHover(__instance, ___currentPageBundle, x, y, announceSections: false);
    }

    private static void NarrateBundlePageHover(
        JunimoNoteMenu __instance,
        Bundle ___currentPageBundle,
        int x,
        int y,
        bool announceSections
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

            NarrateRequiredIngredient(___currentPageBundle, ingredient, i, announceSections);
            return;
        }

        for (int i = 0; i < __instance.ingredientSlots.Count; i++)
        {
            ClickableTextureComponent slot = __instance.ingredientSlots[i];
            if (!slot.containsPoint(x, y))
                continue;

            NarrateDepositSlot(slot, i, useMenuChecker: true, announceSections);
            return;
        }

        if (__instance.inventory != null
            && !___currentPageBundle.complete
            && ___currentPageBundle.completionTimer <= 0)
        {
            NarrateInventorySlot(__instance.inventory, x, y, useMenuChecker: true, announceSections);
        }
    }

    private static void NarrateRequiredIngredient(
        Bundle currentPageBundle,
        ClickableTextureComponent ingredient,
        int index,
        bool announceSections
    )
    {
        string itemDetails = !string.IsNullOrWhiteSpace(ingredient.hoverText)
            ? ingredient.hoverText
            : ingredient.item?.DisplayName ?? "";

        bool completed = false;
        if (index < currentPageBundle.ingredients.Count)
        {
            BundleIngredientDescription description = currentPageBundle.ingredients[index];
            completed = description.completed;

            if (!completed)
            {
                Item? item = ingredient.item;
                if (item == null)
                {
                    string representativeItemId = JunimoNoteMenu.GetRepresentativeItemId(description);
                    item = description.preservesId == null
                        ? ItemRegistry.Create(representativeItemId, description.stack, description.quality)
                        : Utility.CreateFlavoredItem(
                            representativeItemId,
                            description.preservesId,
                            description.quality,
                            description.stack
                        );
                }

                itemDetails =
                    $"{InventoryUtils.GetPluralNameOfItem(item)}, {InventoryUtils.GetQualityFromIndex(description.quality)}";
            }
            else if (string.IsNullOrWhiteSpace(itemDetails) && ingredient.item != null)
            {
                itemDetails = ingredient.item.DisplayName;
            }
        }

        if (string.IsNullOrWhiteSpace(itemDetails))
            return;

        string translationKey = completed
            ? "menu-junimo_note-required_ingredient_completed"
            : "menu-junimo_note-required_ingredient";

        string toSpeak = Translator.Instance.Translate(
            translationKey,
            new { content = itemDetails },
            TranslationCategory.Menu
        );
        toSpeak = WithSectionPrefix(BundleSection.Required, toSpeak, announceSections);

        MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
    }

    private static void NarrateDepositSlot(
        ClickableTextureComponent slot,
        int index,
        bool useMenuChecker,
        bool announceSections
    )
    {
        string toSpeak = slot.item == null
            ? Translator.Instance.Translate(
                "menu-junimo_note-deposit_slot_empty",
                new { index = index + 1 },
                TranslationCategory.Menu
            )
            : Translator.Instance.Translate(
                "menu-junimo_note-deposit_slot_filled",
                new { index = index + 1, item_name = slot.item.DisplayName },
                TranslationCategory.Menu
            );

        toSpeak = WithSectionPrefix(BundleSection.Deposit, toSpeak, announceSections);

        if (useMenuChecker)
            MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
        else
            MainClass.ScreenReader.Say(toSpeak, true);
    }

    private static void NarrateInventorySlot(
        InventoryMenu inventoryMenu,
        int x,
        int y,
        bool useMenuChecker,
        bool announceSections
    )
    {
        List<ClickableComponent> inventory = inventoryMenu.inventory;
        IList<Item> actualInventory = inventoryMenu.actualInventory;

        for (int i = 0; i < inventory.Count; i++)
        {
            if (!inventory[i].containsPoint(x, y))
                continue;

            // Skip invisible trailing slots.
            if (!inventory[i].visible)
                continue;

            string toSpeak;
            string customQuery;

            if ((inventoryMenu.playerInventory || inventoryMenu.showGrayedOutSlots) && i >= actualInventory.Count)
            {
                toSpeak = Translator.Instance.Translate(
                    "menu-junimo_note-inventory_item",
                    new { content = Translator.Instance.Translate("inventory_util-locked_slot") },
                    TranslationCategory.Menu
                );
                customQuery = $"junimo-inventory-locked:{i}";
            }
            else if (i >= actualInventory.Count || actualInventory[i] == null)
            {
                toSpeak = Translator.Instance.Translate(
                    "menu-junimo_note-inventory_empty_slot",
                    TranslationCategory.Menu
                );
                customQuery = $"junimo-inventory-empty:{i}";
            }
            else
            {
                // Pass isHighlighted: null so InventoryUtils does not play invalid-selection
                // every draw frame (prevSlotIndex is only updated by NarrateHoveredSlot).
                bool canDonate = inventoryMenu.highlightMethod(actualInventory[i]);
                string itemDetails = InventoryUtils.GetItemDetails(
                    actualInventory[i],
                    i,
                    isHighlighted: null,
                    giveExtraDetails: !MainClass.Config.DisableInventoryVerbosity
                );

                string translationKey = canDonate
                    ? "menu-junimo_note-inventory_donatable"
                    : "menu-junimo_note-inventory_not_donatable";

                toSpeak = Translator.Instance.Translate(
                    translationKey,
                    new { content = itemDetails },
                    TranslationCategory.Menu
                );
                customQuery = $"junimo-inventory-item:{i}:{canDonate}:{itemDetails}";
            }

            toSpeak = WithSectionPrefix(BundleSection.Inventory, toSpeak, announceSections);

            if (useMenuChecker)
                MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true, customQuery);
            else
                MainClass.ScreenReader.Say(toSpeak, true);
            return;
        }
    }

    /// <summary>
    /// For custom keybinds only: speak the section name once when switching between
    /// inventory / required items / deposit slots. Legacy hover skips this entirely.
    /// </summary>
    private static string WithSectionPrefix(BundleSection section, string content, bool announceSections)
    {
        if (!announceSections)
            return content;

        if (_lastSection == section)
            return content;

        _lastSection = section;
        string sectionKey = section switch
        {
            BundleSection.Inventory => "menu-junimo_note-section_inventory",
            BundleSection.Required => "menu-junimo_note-section_required",
            BundleSection.Deposit => "menu-junimo_note-section_deposit",
            _ => ""
        };

        if (string.IsNullOrWhiteSpace(sectionKey))
            return content;

        string sectionName = Translator.Instance.Translate(sectionKey, TranslationCategory.Menu);
        return $"{sectionName}, {content}";
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
        c.snapMouseCursorToCenter();
        NarrateRequiredIngredient(___currentPageBundle, c, currentIngredientListItem, announceSections: true);
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
        c.snapMouseCursorToCenter();
        NarrateDepositSlot(c, currentIngredientInputSlot, useMenuChecker: false, announceSections: true);
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
        c.snapMouseCursorToCenter();
        NarrateInventorySlot(
            __instance.inventory,
            c.bounds.Center.X,
            c.bounds.Center.Y,
            useMenuChecker: false,
            announceSections: true
        );
    }

    internal static void Cleanup()
    {
        currentIngredientListItem = -1;
        currentIngredientInputSlot = -1;
        currentInventorySlot = -1;
        firstTimeInMenu = true;
        _announcedBundlePage = false;
        _lastSection = BundleSection.None;
    }
}
