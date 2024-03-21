using System.Text;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

// These patches are global, i.e. work on every menus
internal class IClickableMenuPatch : IPatch
{
    private static readonly HashSet<Type> SkipMenuTypes =
    [
        typeof(AdvancedGameOptions),
        typeof(AnimalQueryMenu),
        typeof(Billboard),
        typeof(CarpenterMenu),
        typeof(CharacterCustomization),
        typeof(ConfirmationDialog),
        typeof(CoopMenu),
        typeof(FarmhandMenu),
        typeof(FieldOfficeMenu),
        typeof(ForgeMenu),
        typeof(GeodeMenu),
        typeof(ItemGrabMenu),
        typeof(ItemListMenu),
        typeof(JojaCDMenu),
        typeof(JunimoNoteMenu),
        typeof(LetterViewerMenu),
        typeof(MuseumMenu),
        typeof(NumberSelectionMenu),
        typeof(PondQueryMenu),
        typeof(PurchaseAnimalsMenu),
        typeof(QuestContainerMenu),
        typeof(QuestLog),
        typeof(ReadyCheckDialog),
        typeof(ShopMenu),
        typeof(SpecialOrdersBoard),
        typeof(TailoringMenu),
        typeof(TitleMenu)
    ];

    private static readonly HashSet<Type> SkipGameMenuPageTypes =
    [
        typeof(CollectionsPage),
        typeof(CraftingPage),
        typeof(ExitPage),
        typeof(InventoryPage),
        typeof(OptionsPage),
        typeof(SkillsPage),
        typeof(SocialPage)
    ];

    internal static HashSet<string> ManuallyPatchedCustomMenus = [];

    public void Apply(Harmony harmony)
    {
        harmony.Patch(
                original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.exitThisMenu)),
                postfix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(IClickableMenuPatch.ExitThisMenuPatch))
        );

        harmony.Patch(
            original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.drawHoverText), new Type[] { typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont), typeof(int), typeof(int), typeof(int), typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(string), typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe), typeof(IList<Item>) ,typeof(Texture2D), typeof(Rectangle?), typeof(Color?), typeof(Color?), typeof(float), typeof(int), typeof(int)}),
            postfix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(IClickableMenuPatch.DrawHoverTextPatch))
        );
    }

    private static void DrawHoverTextPatch(string? text,
                                           int moneyAmountToDisplayAtBottom = -1,
                                           string? boldTitleText = null,
                                           string? extraItemToShowIndex = null,
                                           int extraItemToShowAmount = -1,
                                           string[]? buffIconsToDisplay = null,
                                           Item? hoveredItem = null,
                                           CraftingRecipe? craftingIngredients = null)
    {
        try
        {
            #region Skip narrating hover info for manually patched custom menus

            if (Game1.activeClickableMenu != null)
            {
                foreach (var fullNameOfCustomMenu in ManuallyPatchedCustomMenus)
                {
                    if (Game1.activeClickableMenu.GetType().FullName == fullNameOfCustomMenu)
                        return;
                }
            }

            #endregion
            
            #region Skip narrating hover text for certain menus
            var activeClickableMenu = Game1.activeClickableMenu?.GetType();
            var activeGameMenuPage = Game1.activeClickableMenu is GameMenu gameMenu ? gameMenu.GetCurrentPage().GetType() : null;
            // Check both sets as game menu pages can sometimes be stand alone menus
            // E.G. CraftingPage is stand alone menu at stove.
            if (activeClickableMenu is not null && (SkipMenuTypes.Contains(activeClickableMenu) || SkipGameMenuPageTypes.Contains(activeClickableMenu)))
            {
                return;
            }

            if (Game1.activeClickableMenu is TitleMenu titleMenu && TitleMenu.subMenu is not CharacterCustomization)
            {
                return;
            }

            if (activeGameMenuPage != null && (SkipGameMenuPageTypes.Contains(activeGameMenuPage)))
            {
                return;
            }
            #endregion

            string toSpeak = "";

            if (hoveredItem != null)
            {
                toSpeak = InventoryUtils.GetItemDetails(hoveredItem,
                                                        hoverPrice: moneyAmountToDisplayAtBottom,
                                                        extraItemToShowIndex: extraItemToShowIndex,
                                                        extraItemToShowAmount: extraItemToShowAmount,
                                                        customBuffs: buffIconsToDisplay);
                toSpeak += (craftingIngredients is not null)
                    ? $", {InventoryUtils.GetIngredientsFromRecipe(craftingIngredients)}"
                    : "";
            }
            else
            {
                if (!string.IsNullOrEmpty(boldTitleText))
                    toSpeak = $"{boldTitleText}, ";

                if (text == "???")
                    toSpeak = Translator.Instance.Translate("common-unknown");
                else if (!string.IsNullOrEmpty(text))
                    toSpeak += text;
            }

            // To prevent it from getting conflicted by two hover texts at the same time, two separate methods are used.
            // For example, sometimes `Welcome to Pierre's` and the items in seeds shop get conflicted causing it to speak infinitely.

            if (toSpeak.Length > 0)
            {
                if (Game1.activeClickableMenu is not null)
                    MainClass.ScreenReader.SayWithMenuChecker(toSpeak.ToString(), true); // Menu Checker
                else
                    MainClass.ScreenReader.SayWithChecker(toSpeak.ToString(), true); // Normal Checker
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in draw hover text patch:\n{e.StackTrace}\n{e.Message}");
        }
    }

    private static void ExitThisMenuPatch(IClickableMenu __instance)
    {
        try
        {
            Log.Debug($"Closed {__instance.GetType()} menu, performing cleanup...");
            Cleanup(__instance);
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in exit this menu patch:\n{e.Message}\n{e.StackTrace}");
        }
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
        }

        MainClass.ScreenReader.Cleanup();
        InventoryUtils.Cleanup();
        TextBoxPatch.activeTextBoxes = "";
    }
}
