using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    // These patches are global, i.e. work on every menus
    internal class IClickableMenuPatch : IPatch
    {
        private static readonly HashSet<Type> SkipMenuTypes = new()
        {
            typeof(AnimalQueryMenu),
            typeof(Billboard),
            typeof(CarpenterMenu),
            typeof(ConfirmationDialog),
            typeof(FieldOfficeMenu),
            typeof(ForgeMenu),
            typeof(GeodeMenu),
            typeof(ItemGrabMenu),
            typeof(ItemListMenu),
            typeof(JojaCDMenu),
            typeof(JunimoNoteMenu),
            typeof(LetterViewerMenu),
            typeof(MuseumMenu),
            typeof(PondQueryMenu),
            typeof(PurchaseAnimalsMenu),
            typeof(QuestLog),
            typeof(ReadyCheckDialog),
            typeof(ShopMenu),
            typeof(TailoringMenu),
            typeof(SpecialOrdersBoard),
            typeof(NumberSelectionMenu)
        };

        private static readonly HashSet<Type> SkipGameMenuPageTypes = new()
        {
            typeof(CraftingPage),
            typeof(ExitPage),
            typeof(InventoryPage),
            typeof(OptionsPage),
            typeof(SocialPage)
        };

        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                    original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.exitThisMenu)),
                    postfix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(IClickableMenuPatch.ExitThisMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.drawHoverText), new Type[] { typeof(SpriteBatch), typeof(string), typeof(SpriteFont), typeof(int), typeof(int), typeof(int), typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe), typeof(IList<Item>) }),
                postfix: new HarmonyMethod(typeof(IClickableMenuPatch), nameof(IClickableMenuPatch.DrawHoverTextPatch))
            );
        }

        private static void DrawHoverTextPatch(string? text,
                                               int moneyAmountToDisplayAtBottom = -1,
                                               string? boldTitleText = null,
                                               int extraItemToShowIndex = -1,
                                               int extraItemToShowAmount = -1,
                                               string[]? buffIconsToDisplay = null,
                                               Item? hoveredItem = null,
                                               CraftingRecipe? craftingIngredients = null)
        {
            try
            {
                #region Skip narrating hover text for certain menus
                if (Game1.activeClickableMenu != null && SkipMenuTypes.Contains(Game1.activeClickableMenu.GetType()))
                {
                    return;
                }

                if (Game1.activeClickableMenu is TitleMenu titleMenu && titleMenu.GetChildMenu() is not CharacterCustomization)
                {
                    return;
                }

                if (Game1.activeClickableMenu is GameMenu gameMenu && SkipGameMenuPageTypes.Contains(gameMenu.GetCurrentPage().GetType()))
                {
                    return;
                }
                #endregion

                // TODO Use InventoryUtils.cs
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
                case TitleMenu:
                    TitleMenuPatch.Cleanup();
                    break;
                case CoopMenu:
                    CoopMenuPatch.Cleanup();
                    break;
                case LoadGameMenu:
                    LoadGameMenuPatch.Cleanup();
                    break;
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
                case SpecialOrdersBoard:
                    SpecialOrdersBoardPatch.Cleanup();
                    break;
                case NumberSelectionMenu:
                    NumberSelectionMenuPatch.Cleanup();
                    break;
            }

            MainClass.ScreenReader.PrevMenuQueryText = "";
            MainClass.ScreenReader.MenuPrefixText = "";
            MainClass.ScreenReader.MenuSuffixText = "";
            InventoryUtils.Cleanup();
            TextBoxPatch.activeTextBoxes = "";
        }
    }
}
