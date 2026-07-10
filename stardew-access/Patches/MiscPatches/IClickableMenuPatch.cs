using System.Diagnostics.CodeAnalysis;
using System.Text;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using stardew_access.Features;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

// These patches are global, i.e. work on every menu
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal class IClickableMenuPatch : IPatch
{
    private static readonly HashSet<Type> SkipMenuTypes =
    [
        /*********
        ** Bundle Menus
        *********/
        typeof(JojaCDMenu),
        typeof(JunimoNoteMenu),

        /*********
        ** Donation Menus
        *********/
        typeof(FieldOfficeMenu),
        typeof(MuseumMenu),

        /*********
        ** Game Menu Pages
        *********/
        typeof(AnimalPage),
        typeof(CollectionsPage),
        typeof(CraftingPage),
        typeof(ExitPage),
        typeof(InventoryPage),
        typeof(MapPage),
        typeof(PowersTab),
        typeof(SkillsPage),
        typeof(SocialPage),

        /*********
        ** Menus with Inventory
        *********/
        typeof(ForgeMenu),
        typeof(GeodeMenu),
        typeof(ItemGrabMenu),
        typeof(QuestContainerMenu),
        typeof(ShopMenu),
        typeof(StorageContainer),
        typeof(TailoringMenu),

        /*********
        ** Quest Menus
        *********/
        typeof(Billboard),
        typeof(QuestLog),
        typeof(SpecialOrdersBoard),

        /*********
        ** Title Menus
        *********/
        typeof(TitleMenu),
        // typeof(AdvancedGameOptions),
        typeof(CharacterCustomization),
        typeof(CoopMenu),
        typeof(FarmhandMenu),
        typeof(LoadGameMenu),
  
        /*********
        ** Other Menus
        *********/
        typeof(AnimalQueryMenu),
        typeof(BuildingSkinMenu),
        typeof(CarpenterMenu),
        typeof(ChooseFromIconsMenu),
        typeof(ChooseFromListMenu),
        typeof(ConfirmationDialog),
        typeof(DialogueBox),
        typeof(ItemListMenu),
        typeof(LetterViewerMenu),
        typeof(LevelUpMenu),
        typeof(MasteryTrackerMenu),
        typeof(NamingMenu),
        typeof(NumberSelectionMenu),
        typeof(PondQueryMenu),
        typeof(PrizeTicketMenu),
        typeof(PurchaseAnimalsMenu),
        typeof(RenovateMenu),
        typeof(ShippingMenu),
        typeof(TitleTextInputMenu),
        typeof(ReadyCheckDialog),

        /*********
        ** Custom Menus
        *********/
        typeof(TileDataEntryMenu),
    ];

    private static readonly HashSet<Type> AllowFallbackInMenuTypes =
    [
        typeof(OptionsPage),
        typeof(AdvancedGameOptions),
        typeof(LanguageSelectionMenu),
        typeof(MineElevatorMenu)
    ];
    
    private static bool _tryHoverPatch = false;

    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    internal static HashSet<string> ManuallyPatchedCustomMenus = [];
    
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    internal static HashSet<string> IgnoreHoverTextInMenus = [];
    
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    internal static HashSet<string> IgnoreClickableComponentsInMenus = [];

    internal static string? CurrentMenu;
    internal static bool ManuallyCallingDrawPatch = false;
    
    private static readonly Dictionary<string, Func<bool>> DrawHandlers = new();
    private static readonly Dictionary<string, Func<int, int, int, bool>> DrawWithParamsHandlers = new();
    private static readonly Dictionary<string, Func<IStardewAccessApi.IDrawHoverTextData, bool>> DrawHoverTextHandlers = new();

    #if DEBUG
    private static bool _justOpened = true;
    #endif

    internal static IClickableMenu? ActiveMenuOrSubMenu
    {
        get
        {
            var activeMenu = Game1.activeClickableMenu;

            if (activeMenu == null) return null;

            if (activeMenu.GetParentMenu() != null)
            {
                // To let the parent menu's draw() call set `activeMenu` to the child menu, in the next if condition.
                return null;
            }

            if (Game1.textEntry != null)
            {
                return Game1.textEntry;
            }

            while (activeMenu.GetChildMenu() != null)
            {
                activeMenu = activeMenu.GetChildMenu();
            }

            if (activeMenu is TitleMenu titleMenu && TitleMenu.subMenu != null)
            {
                return TitleMenu.subMenu;
            }

            if (activeMenu is GameMenu gameMenu)
            {
                return gameMenu.GetCurrentPage();
            }

            return activeMenu;
        }
    }


    public void Apply(Harmony harmony)
    {
        harmony.Patch(
                original: AccessTools.Method(typeof(IClickableMenu), "exitThisMenu"),
                postfix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(ExitThisMenuPatch))
        );

        harmony.Patch(
            original: AccessTools.Method(typeof(IClickableMenu), "draw", [typeof(SpriteBatch)]),
            postfix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(DrawPatch))
        );

        harmony.Patch(
            original: AccessTools.Method(typeof(IClickableMenu), "draw", [typeof(SpriteBatch), typeof(int), typeof(int), typeof(int)]),
            postfix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(DrawWithParamsPatch))
        );

        harmony.Patch(
            original: AccessTools.Method(typeof(IClickableMenu), "drawHoverText", [typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont), typeof(int), typeof(int), typeof(int), typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(string), typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe), typeof(IList<Item>), typeof(Texture2D), typeof(Rectangle?), typeof(Color?), typeof(Color?), typeof(float), typeof(int), typeof(int)]),
            postfix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(DrawHoverTextPatch))
        );
        harmony.Patch(
            original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.receiveKeyPress), [typeof(Keys)]),
            prefix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(ReceiveKeyPressPatch))
        );
    }

    public static void DrawPatch()
    {
        try
        {
            foreach (var (modId, handler) in DrawHandlers)
            {
                Log.Trace($"[IClickableMenuPatch::DrawPatch] Executing handler by mod {modId}", once: true);
                bool canContinue = handler.Invoke();

                if (!canContinue)
                {
#if DEBUG
                    Log.Trace( $"[IClickableMenuPatch::DrawPatch] a handler by mod {modId} returned false, cancelling further execution", true);
#endif
                    return;
                }
            }
        
            DrawPatchDefaultHandler();
        }
        catch (Exception e)
        {
            Log.Error($"[IClickableMenuPatch.DrawPatch]: {e.Message}\n{e.StackTrace}");
        }
    }

    public static void DrawWithParamsPatch(int red, int green, int blue)
    {
        try
        {
            foreach (var (modId, handler) in DrawWithParamsHandlers)
            {
                Log.Trace($"[IClickableMenuPatch::DrawWithParamsPatch] Executing handler by mod {modId}", once: true);
                bool canContinue = handler.Invoke(red, green, blue);

                if (!canContinue)
                {
#if DEBUG
                    Log.Trace( $"[IClickableMenuPatch::DrawWithParamsPatch] a handler by mod {modId} returned false, cancelling further execution", true);
#endif
                    return;
                }
            }
        
            DrawPatchDefaultHandler();
        }
        catch (Exception e)
        {
            Log.Error($"[IClickableMenuPatch.DrawWithParamsPatch]: {e.Message}\n{e.StackTrace}");
        }       
    }

    private static void DrawPatchDefaultHandler()
    {
            // The only case when the active menu is null (in vanilla stardew) is when a hud message with no icon is displayed.
            if (Game1.activeClickableMenu is null) return;

            var activeMenu = ActiveMenuOrSubMenu;
            if (activeMenu is null) return;

            // FIXME For some reason custom mod menus don't trigger this patch, even though they implement IClickableMenu,
            // the following method essentially detects if the active menu hasn't triggered this method
            // and then manually calls it from ModEntry::OnUpdateTicked
            if (!ManuallyCallingDrawPatch) CurrentMenu = activeMenu.GetType()?.FullName;
            else ManuallyCallingDrawPatch = false;

            var activeMenuType = activeMenu.GetType();
            if (SkipMenuTypes.Contains(activeMenuType)
                || ManuallyPatchedCustomMenus.Contains(activeMenuType.FullName ?? "")
                || IgnoreClickableComponentsInMenus.Contains(activeMenuType.FullName ?? ""))
            {
                Log.Debug($"[IClickableMenuPatch::DrawPatch] Skipping menu {activeMenuType.FullName}", once: true);
                return;
            }
            
            bool allowFallback = AllowFallbackInMenuTypes.Contains(activeMenuType);
            Log.Debug($"[IClickableMenuPatch::DrawPatch] allowFallback: {allowFallback} ActiveMenuType: {activeMenuType}", once: true);

            #if DEBUG
            if (_justOpened)
            {
                _justOpened = false;
                Log.Debug($"[IClickableMenuPatch.DrawPatch] Attempting to patch menu {{ManuallyCalled:{ManuallyCallingDrawPatch}}} {{AllowFallback:{allowFallback}}}: {activeMenuType?.FullName}");
            }
            #endif

            if (activeMenu.currentlySnappedComponent == null || string.IsNullOrWhiteSpace(activeMenu!.currentlySnappedComponent.ScreenReaderText))
            {
                if (OptionsElementUtils.NarrateOptionSlotsInMenuUsingReflection(activeMenu, allowFallback: allowFallback))
                    return;
            }
            else
            {
                if (ClickableComponentUtils.NarrateComponent(activeMenu.currentlySnappedComponent, allowFallback: allowFallback))
                    return;
            }

            if (ClickableComponentUtils.NarrateHoveredComponentUsingReflectionInMenu(activeMenu, allowFallback: allowFallback))
            {
                return;
            }

            if (ClickableComponentUtils.NarrateHoveredComponentFromList(activeMenu.allClickableComponents, allowFallback: allowFallback))
            {
                return;
            }

            _tryHoverPatch = true;
    }

    private static void DrawHoverTextPatch(StringBuilder text,
        SpriteFont font,
        int xOffset = 0,
        int yOffset = 0,
        int moneyAmountToDisplayAtBottom = -1,
        string? boldTitleText = null,
        int healAmountToDisplay = -1,
        string[]? buffIconsToDisplay = null,
        Item? hoveredItem = null,
        int currencySymbol = 0,
        string? extraItemToShowIndex = null,
        int extraItemToShowAmount = -1,
        int overrideX = -1,
        int overrideY = -1,
        float alpha = 1f,
        CraftingRecipe? craftingIngredients = null,
        IList<Item>? additional_craft_materials = null,
        Texture2D? boxTexture = null,
        Rectangle? boxSourceRect = null,
        Color? textColor = null,
        Color? textShadowColor = null,
        float boxScale = 1f,
        int boxWidthOverride = -1,
        int boxHeightOverride = -1)
    {
        foreach (var (modId, handler) in DrawHoverTextHandlers)
        {
            Log.Trace($"[IClickableMenuPatch::DrawHoverTextPatch] Executing handler by mod {modId}", once: true);
            bool canContinue = handler.Invoke(
                new DrawHoverTextData(
                    text,
                    font,
                    xOffset,
                    yOffset,
                    moneyAmountToDisplayAtBottom,
                    boldTitleText,
                    healAmountToDisplay,
                    buffIconsToDisplay,
                    hoveredItem,
                    currencySymbol,
                    extraItemToShowIndex,
                    extraItemToShowAmount,
                    overrideX,
                    overrideY,
                    alpha,
                    craftingIngredients,
                    additional_craft_materials,
                    boxTexture,
                    boxSourceRect,
                    textColor,
                    textShadowColor,
                    boxScale,
                    boxWidthOverride,
                    boxHeightOverride
                )
            );

            if (!canContinue)
            {
#if DEBUG
                Log.Trace($"[IClickableMenuPatch::DrawHoverTextPatch] a handler by mod {modId} returned false, cancelling further execution", true);
#endif
                return;
            }
        }
        
        if (!_tryHoverPatch && ActiveMenuOrSubMenu != null && !IgnoreClickableComponentsInMenus.Contains(ActiveMenuOrSubMenu.GetType().FullName ?? "")) return;
        if (ActiveMenuOrSubMenu != null && IgnoreHoverTextInMenus.Contains(ActiveMenuOrSubMenu.GetType().FullName ?? ""))
        {
            Log.Debug($"[IClickableMenuPatch::DrawHoverTextPatch] Skipping menu {ActiveMenuOrSubMenu.GetType().FullName}", once: true);
            return;
        }

        try
        {
            string toSpeak = "";

            if (hoveredItem != null)
            {
                toSpeak = InventoryUtils.GetItemDetails(
                    hoveredItem,
                    hoverPrice: moneyAmountToDisplayAtBottom,
                    extraItemToShowIndex: extraItemToShowIndex,
                    extraItemToShowAmount: extraItemToShowAmount,
                    customBuffs: buffIconsToDisplay,
                    alternativeDisplayName: boldTitleText,
                    alternativeDescription: text.ToString() == "???"
                        ? Translator.Instance.Translate("common-unknown")
                        : text.ToString()
                );
                toSpeak += (craftingIngredients is not null)
                    ? $", {InventoryUtils.GetIngredientsFromRecipe(craftingIngredients)}"
                    : "";
            }
            else
            {
                if (!string.IsNullOrEmpty(boldTitleText))
                    toSpeak = $"{boldTitleText}, ";

                if (text.ToString() == "???")
                    toSpeak = Translator.Instance.Translate("common-unknown");
                else
                    toSpeak += text;
            }

            _tryHoverPatch = false;

            // To prevent it from getting conflicted by two hover texts at the same time, two separate methods are used.
            // For example, sometimes `Welcome to Pierre's` and the items in seeds shop get conflicted causing it to speak infinitely.

            if (string.IsNullOrWhiteSpace(toSpeak)) return;

            MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true); // Menu Checker
        }
        catch (Exception e)
        {
            Log.Error($"[IClickableMenuPatch.DrawHoverTextPatch]: {e.Message}\n{e.StackTrace}");
        }
    }

    internal static bool HandleMenuMovementKeyPress(IClickableMenu activeMenu, ref Keys key)
    {
        var pressedInput = new InputButton(key);
        if (Game1.options.moveUpButton.Contains(pressedInput))
        {
            activeMenu.applyMovementKey(0);
            return false;
        }
        else if (Game1.options.moveRightButton.Contains(pressedInput))
        {
            activeMenu.applyMovementKey(1);
            return false;
        }
        else if (Game1.options.moveDownButton.Contains(pressedInput))
        {
            activeMenu.applyMovementKey(2);
            return false;
        }
        else if (Game1.options.moveLeftButton.Contains(pressedInput))
        {
            activeMenu.applyMovementKey(3);
            return false;
        }

        // For any other key, let the original method process it
        return true;
    }

    private static bool ReceiveKeyPressPatch(IClickableMenu __instance, ref Keys key)
    {
        if (__instance == null) return true;
        var activeMenu = ActiveMenuOrSubMenu;
                return activeMenu switch
        {
            LoadGameMenu loadGameMenu => LoadGameMenuPatch.ReceiveKeyPressPatch(loadGameMenu, ref key),
            _ => true // Returns true for null and any other unmatched type; handle normally.
        };
    }

    private static void ExitThisMenuPatch(IClickableMenu __instance)
    {
        try
        {
            Log.Verbose($"[IClickableMenuPatch.ExitThisMenuPatch] Closed {__instance.GetType().FullName} menu, performing cleanup...");
            Cleanup(__instance);
        }
        catch (Exception e)
        {
            Log.Error($"[IClickableMenuPatch.ExitThisMenuPatch]: {e.Message}\n{e.StackTrace}");
        }
    }

    internal static void RegisterDrawHandler(Func<bool> handler, string modId)
    {
        Log.Debug($"[IClickableMenuPatch] Registering draw handler for mod {modId}");
        DrawHandlers[modId] = handler;
    }

    internal static void RegisterDrawWithParamsHandler(Func<int, int, int, bool> handler, string modId)
    {
        Log.Debug($"[IClickableMenuPatch] Registering draw with params handler for mod {modId}");
        DrawWithParamsHandlers[modId] = handler;
    }

    internal static void RegisterDrawHoverTextHandler(Func<IStardewAccessApi.IDrawHoverTextData, bool> handler, string modId)
    {
        Log.Debug($"[IClickableMenuPatch] Registering draw hover text handler for mod {modId}");
        DrawHoverTextHandlers[modId] = handler;
    }

    internal static void Cleanup(IClickableMenu menu)
    {
        switch (menu)
        {
            case LetterViewerMenu:
                LetterViewerMenuPatch.Cleanup();
                break;
            case GameMenu:
                CraftingPagePatch.Cleanup();
                MapPagePatch.Cleanup();
                break;
            case JunimoNoteMenu:
                JunimoNoteMenuPatch.Cleanup();
                break;
            case CarpenterMenu:
                CarpenterMenuPatch.Cleanup();
                break;
            case PurchaseAnimalsMenu:
                PurchaseAnimalsMenuPatch.Cleanup();
                break;
            case AnimalQueryMenu:
                AnimalQueryMenuPatch.Cleanup();
                break;
            case DialogueBox:
                DialogueBoxPatch.Cleanup();
                break;
            case QuestLog:
                QuestLogPatch.Cleanup();
                break;
            case PondQueryMenu:
                PondQueryMenuPatch.Cleanup();
                break;
            case NumberSelectionMenu:
                NumberSelectionMenuPatch.Cleanup();
                break;
            case NamingMenu:
                NamingMenuPatch.Cleanup();
                break;
            case RenovateMenu:
                RenovateMenuPatch.Cleanup();
                break;
        }

        MainClass.ScreenReader.Cleanup();
        InventoryUtils.Cleanup();
        TextBoxPatch.activeTextBoxes = "";
        CurrentMenu = null;
        ManuallyCallingDrawPatch = false;
        _tryHoverPatch = false;

        #if DEBUG
        _justOpened = true;
        #endif
    }
}
