using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;
using System.Text;

namespace stardew_access.Patches
{
    // These patches are global, i.e. work on every menus
    internal class IClickableMenuPatch
    {
        private static readonly HashSet<Type> SkipMenuTypes = new()
        {
            typeof(AnimalQueryMenu),
            typeof(Billboard),
            typeof(CarpenterMenu),
            typeof(ConfirmationDialog),
            typeof(CraftingPage),
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
            typeof(TailoringMenu)
        };

        private static readonly HashSet<Type> SkipGameMenuPageTypes = new()
        {
            typeof(CraftingPage),
            typeof(ExitPage),
            typeof(InventoryPage),
            typeof(OptionsPage),
            typeof(SocialPage)
        };

        internal static void DrawHoverTextPatch(string? text, int moneyAmountToDisplayAtBottom = -1, string? boldTitleText = null, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1, string[]? buffIconsToDisplay = null, Item? hoveredItem = null, CraftingRecipe? craftingIngredients = null)
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

                StringBuilder toSpeak = new();

                #region Add item count before title
                if (hoveredItem != null && hoveredItem.HasBeenInInventory)
                {
                    int count = hoveredItem.Stack;
                    if (count > 1)
                        toSpeak.AppendFormat(" {0} ", count);
                }
                #endregion

                #region Add title if any
                if (boldTitleText != null)
                    toSpeak.AppendLine($" {boldTitleText}");
                #endregion

                #region Add quality of item
                if (hoveredItem is StardewValley.Object obj && obj.Quality > 0)
                {
                    switch (obj.Quality)
                    {
                        case 1:
                            toSpeak.Append(" Silver quality");
                            break;
                        case 2:
                        case 3:
                            toSpeak.Append(" Gold quality");
                            break;
                        // outer if conditional excluded values <= 0, so default is same as >= 4
                        default:
                            toSpeak.Append(" Iridium quality");
                            break;
                    }
                }
                #endregion

                #region Narrate hovered required ingredients
                if (extraItemToShowIndex != -1)
                {
                    string itemName = Game1.objectInformation[extraItemToShowIndex].Split('/')[0];

                    if (extraItemToShowAmount != -1)
                        toSpeak.AppendFormat(" Required: {0} {1}", extraItemToShowAmount, itemName);
                    else
                        toSpeak.AppendFormat(" Required: {0}", itemName);
                }
                #endregion

                #region Add money
                if (moneyAmountToDisplayAtBottom != -1)
                    toSpeak.AppendLine($"Cost: {moneyAmountToDisplayAtBottom}g");
                #endregion

                #region Add the base text
                if (text == "???")
                    toSpeak.Clear().Append("unknown");
                else
                    toSpeak.Append($" {text}");
                #endregion

                #region Add crafting ingredients
                if (craftingIngredients != null)
                {
                    toSpeak.AppendLine(craftingIngredients.description);
                    toSpeak.AppendLine("Ingredients");

                    foreach (var recipe in craftingIngredients.recipeList)
                    {
                        int count = recipe.Value;
                        int item = recipe.Key;
                        string name = craftingIngredients.getNameFromIndex(item);

                        toSpeak.AppendFormat(",{0} {1}", count, name);
                    }
                }
                #endregion

                #region Add health & stamina
                if (hoveredItem is StardewValley.Object edibleObj && edibleObj.Edibility != -300)
                {
                    int stamina_recovery = edibleObj.staminaRecoveredOnConsumption();
                    toSpeak.AppendLine($" {stamina_recovery} Energy");
                    if (stamina_recovery >= 0)
                    {
                        int health_recovery = edibleObj.healthRecoveredOnConsumption();
                        toSpeak.Append($" {health_recovery} Health");
                    }
                }
                #endregion

                #region Add buff items (effects like +1 walking speed)
                if (buffIconsToDisplay != null)
                {
                    for (int i = 0; i < buffIconsToDisplay.Length; i++)
                    {
                        if (Convert.ToInt32(buffIconsToDisplay[i]) > 0)
                        {
                            toSpeak.Append('+');
                        }
                        toSpeak.Append(buffIconsToDisplay[i]).Append(' ');
                        if (i <= 11)
                        {
                            toSpeak.Append(Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + i, buffIconsToDisplay[i]));
                        }
                        try
                        {
                            int count = int.Parse(buffIconsToDisplay[i]);
                            if (count != 0)
                            {
                                toSpeak.AppendLine();
                            }
                        }
                        catch (Exception) { }
                    }
                }
                #endregion

                #region Narrate toSpeak
                // To prevent it from getting conflicted by two hover texts at the same time, two seperate methods are used.
                // For example, sometimes `Welcome to Pierre's` and the items in seeds shop get conflicted causing it to speak infinitely.

                if (toSpeak.Length > 0)
                {
                    if (StardewModdingAPI.Context.IsPlayerFree)
                        MainClass.ScreenReader.SayWithChecker(toSpeak.ToString(), true); // Normal Checker
                    else
                        MainClass.ScreenReader.SayWithMenuChecker(toSpeak.ToString(), true); // Menu Checker
                }
                #endregion
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate dialog:\n{e.StackTrace}\n{e.Message}");
            }
        }

        internal static void ExitThisMenuPatch(IClickableMenu __instance)
        {
            try
            {
                MainClass.DebugLog($"Closed {__instance.GetType()} menu, performing cleanup...");
                Cleanup(__instance);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
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
                    LetterViwerMenuPatch.Cleanup();
                    break;
                case LevelUpMenu:
                    LevelUpMenuPatch.Cleanup();
                    break;
                case Billboard:
                    BillboardPatch.Cleanup();
                    break;
                case GameMenu:
                    SocialPagePatch.Cleanup();
                    InventoryPagePatch.Cleanup();
                    CraftingPagePatch.Cleanup();
                    break;
                case JunimoNoteMenu:
                    JunimoNoteMenuPatch.Cleanup();
                    break;
                case ShopMenu:
                    ShopMenuPatch.Cleanup();
                    break;
                case ItemGrabMenu:
                    ItemGrabMenuPatch.Cleanup();
                    break;
                case GeodeMenu:
                    GeodeMenuPatch.Cleanup();
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
                case JojaCDMenu:
                    JojaCDMenuPatch.Cleanup();
                    break;
                case QuestLog:
                    QuestLogPatch.Cleaup();
                    break;
                case TailoringMenu:
                    TailoringMenuPatch.Cleanup();
                    break;
                case ForgeMenu:
                    ForgeMenuPatch.Cleanup();
                    break;
                case ItemListMenu:
                    ItemListMenuPatch.Cleanup();
                    break;
                case FieldOfficeMenu:
                    FieldOfficeMenuPatch.Cleanup();
                    break;
                case PondQueryMenu:
                    PondQueryMenuPatch.Cleanup();
                    break;
                case SpecialOrdersBoard:
                    SpecialOrdersBoardPatch.Cleanup();
                    break;
            }

            InventoryUtils.Cleanup();
            TextBoxPatch.activeTextBoxes = "";
        }
    }
}
