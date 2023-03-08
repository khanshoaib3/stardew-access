using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    // These patches are global, i.e. work on every menus
    internal class IClickableMenuPatch
    {
        internal static void DrawHoverTextPatch(string? text, int moneyAmountToDisplayAtBottom = -1, string? boldTitleText = null, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1, string[]? buffIconsToDisplay = null, Item? hoveredItem = null, CraftingRecipe? craftingIngredients = null)
        {
            try
            {
                #region Skip narrating hover text for certain menus
                if (Game1.activeClickableMenu is TitleMenu && !(((TitleMenu)Game1.activeClickableMenu).GetChildMenu() is CharacterCustomization))
                    return;
                else if (Game1.activeClickableMenu is LetterViewerMenu || Game1.activeClickableMenu is QuestLog)
                    return;
                else if (Game1.activeClickableMenu is Billboard)
                    return;
                else if (Game1.activeClickableMenu is GeodeMenu)
                    return;
                else if (Game1.activeClickableMenu is GameMenu && ((GameMenu)Game1.activeClickableMenu).GetCurrentPage() is InventoryPage)
                    return;
                else if (Game1.activeClickableMenu is GameMenu && ((GameMenu)Game1.activeClickableMenu).GetCurrentPage() is CraftingPage)
                    return;
                else if (Game1.activeClickableMenu is GameMenu && ((GameMenu)Game1.activeClickableMenu).GetCurrentPage() is OptionsPage)
                    return;
                else if (Game1.activeClickableMenu is GameMenu && ((GameMenu)Game1.activeClickableMenu).GetCurrentPage() is ExitPage)
                    return;
                else if (Game1.activeClickableMenu is GameMenu && ((GameMenu)Game1.activeClickableMenu).GetCurrentPage() is SocialPage)
                    return;
                else if (Game1.activeClickableMenu is ItemGrabMenu)
                    return;
                else if (Game1.activeClickableMenu is ShopMenu)
                    return;
                else if (Game1.activeClickableMenu is ConfirmationDialog)
                    return;
                else if (Game1.activeClickableMenu is JunimoNoteMenu)
                    return;
                else if (Game1.activeClickableMenu is CarpenterMenu)
                    return;
                else if (Game1.activeClickableMenu is PurchaseAnimalsMenu)
                    return;
                else if (Game1.activeClickableMenu is CraftingPage)
                    return;
                else if (Game1.activeClickableMenu is AnimalQueryMenu)
                    return;
                else if (Game1.activeClickableMenu is ConfirmationDialog)
                    return;
                else if (Game1.activeClickableMenu is ReadyCheckDialog)
                    return;
                else if (Game1.activeClickableMenu is JojaCDMenu)
                    return;
                else if (Game1.activeClickableMenu is TailoringMenu)
                    return;
                else if (Game1.activeClickableMenu is PondQueryMenu)
                    return;
                else if (Game1.activeClickableMenu is ForgeMenu)
                    return;
                else if (Game1.activeClickableMenu is ItemListMenu)
                    return;
                else if (Game1.activeClickableMenu is FieldOfficeMenu)
                    return;
                else if (Game1.activeClickableMenu is MuseumMenu)
                    return;
                #endregion

                string toSpeak = " ";

                #region Add item count before title
                if (hoveredItem != null && hoveredItem.HasBeenInInventory)
                {
                    int count = hoveredItem.Stack;
                    if (count > 1)
                        toSpeak = $"{toSpeak} {count} ";
                }
                #endregion

                #region Add title if any
                if (boldTitleText != null)
                    toSpeak = $"{toSpeak} {boldTitleText}\n";
                #endregion

                #region Add quality of item
                if (hoveredItem is StardewValley.Object && ((StardewValley.Object)hoveredItem).Quality > 0)
                {
                    int quality = ((StardewValley.Object)hoveredItem).Quality;
                    if (quality == 1)
                    {
                        toSpeak = $"{toSpeak} Silver quality";
                    }
                    else if (quality == 2 || quality == 3)
                    {
                        toSpeak = $"{toSpeak} Gold quality";
                    }
                    else if (quality >= 4)
                    {
                        toSpeak = $"{toSpeak} Iridium quality";
                    }
                }
                #endregion

                #region Narrate hovered required ingredients
                if (extraItemToShowIndex != -1)
                {
                    string itemName = Game1.objectInformation[extraItemToShowIndex].Split('/')[0];

                    if (extraItemToShowAmount != -1)
                        toSpeak = $"{toSpeak} Required: {extraItemToShowAmount} {itemName}";
                    else
                        toSpeak = $"{toSpeak} Required: {itemName}";
                }
                #endregion

                #region Add money
                if (moneyAmountToDisplayAtBottom != -1)
                    toSpeak = $"{toSpeak} \nCost: {moneyAmountToDisplayAtBottom}g\n";
                #endregion

                #region Add the base text
                if (text == "???")
                    toSpeak = "unknown";
                else
                    toSpeak = $"{toSpeak} {text}";
                #endregion

                #region Add crafting ingredients
                if (craftingIngredients != null)
                {
                    toSpeak = $"{toSpeak} \n{craftingIngredients.description}";
                    toSpeak = $"{toSpeak} \nIngredients\n";

                    craftingIngredients.recipeList.ToList().ForEach(recipe =>
                    {
                        int count = recipe.Value;
                        int item = recipe.Key;
                        string name = craftingIngredients.getNameFromIndex(item);

                        toSpeak = $"{toSpeak} ,{count} {name}";
                    });
                }
                #endregion

                #region Add health & stamina
                if (hoveredItem is StardewValley.Object && ((StardewValley.Object)hoveredItem).Edibility != -300)
                {
                    int stamina_recovery = ((StardewValley.Object)hoveredItem).staminaRecoveredOnConsumption();
                    toSpeak = $"{toSpeak} {stamina_recovery} Energy\n";
                    if (stamina_recovery >= 0)
                    {
                        int health_recovery = ((StardewValley.Object)hoveredItem).healthRecoveredOnConsumption();
                        toSpeak = $"{toSpeak} {health_recovery} Health";
                    }
                }
                #endregion

                #region Add buff items (effects like +1 walking speed)
                if (buffIconsToDisplay != null)
                {
                    for (int i = 0; i < buffIconsToDisplay.Length; i++)
                    {
                        string buffName = ((Convert.ToInt32(buffIconsToDisplay[i]) > 0) ? "+" : "") + buffIconsToDisplay[i] + " ";
                        if (i <= 11)
                        {
                            buffName = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + i, buffName);
                        }
                        try
                        {
                            int count = int.Parse(buffName.Substring(0, buffName.IndexOf(' ')));
                            if (count != 0)
                                toSpeak = $"{toSpeak} {buffName}\n";
                        }
                        catch (Exception) { }
                    }
                }
                #endregion

                #region Narrate toSpeak
                // To prevent it from getting conflicted by two hover texts at the same time, two seperate methods are used.
                // For example, sometimes `Welcome to Pierre's` and the items in seeds shop get conflicted causing it to speak infinitely.

                if (toSpeak.ToString() != " ")
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
                Cleanup(__instance);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void Cleanup(IClickableMenu menu)
        {
            if (menu is TitleMenu)
            {
                TitleMenuPatch.Cleanup();
            }
            else if (menu is CoopMenu)
            {
                CoopMenuPatch.Cleanup();
            }
            else if (menu is LoadGameMenu)
            {
                LoadGameMenuPatch.Cleanup();
            }
            else if (menu is AdvancedGameOptions)
            {
                AdvancedGameOptionsPatch.Cleanup();
            }
            else if (menu is LetterViewerMenu)
            {
                LetterViwerMenuPatch.Cleanup();
            }
            else if (menu is LevelUpMenu)
            {
                MenuPatches.currentLevelUpTitle = " ";
            }
            else if (menu is Billboard)
            {
                QuestPatches.currentDailyQuestText = " ";
            }
            else if (menu is GameMenu)
            {
                GameMenuPatch.Cleanup();
                ExitPagePatch.Cleanup();
                OptionsPagePatch.Cleanup();
                SocialPagePatch.Cleanup();
                InventoryPagePatch.Cleanup();
                CraftingPagePatch.Cleanup();
            }
            else if (menu is JunimoNoteMenu)
            {
                JunimoNoteMenuPatch.Cleanup();
            }
            else if (menu is ShopMenu)
            {
                ShopMenuPatch.Cleanup();
            }
            else if (menu is ItemGrabMenu)
            {
                ItemGrabMenuPatch.Cleanup();
            }
            else if (menu is GeodeMenu)
            {
                GeodeMenuPatch.Cleanup();
            }
            else if (menu is CarpenterMenu)
            {
                CarpenterMenuPatch.carpenterMenuQuery = "";
                CarpenterMenuPatch.isUpgrading = false;
                CarpenterMenuPatch.isDemolishing = false;
                CarpenterMenuPatch.isPainting = false;
                CarpenterMenuPatch.isMoving = false;
                CarpenterMenuPatch.isConstructing = false;
                CarpenterMenuPatch.carpenterMenu = null;
            }
            else if (menu is PurchaseAnimalsMenu)
            {
                PurchaseAnimalsMenuPatch.Cleanup();
            }
            else if (menu is AnimalQueryMenu)
            {
                AnimalQueryMenuPatch.Cleanup();
            }
            else if (menu is DialogueBox)
            {
                DialogueBoxPatch.Cleanup();
            }
            else if (menu is JojaCDMenu)
            {
                JojaCDMenuPatch.Cleanup();
            }
            else if (menu is QuestLog)
            {
                QuestPatches.questLogQuery = " ";
            }
            else if (menu is TailoringMenu)
            {
                MenuPatches.tailoringMenuQuery = " ";
            }
            else if (menu is ForgeMenu)
            {
                MenuPatches.forgeMenuQuery = " ";
            }
            else if (menu is ItemListMenu)
            {
                MenuPatches.itemListMenuQuery = " ";
            }
            else if (menu is FieldOfficeMenu)
            {
                DonationMenuPatches.fieldOfficeMenuQuery = " ";
            }
            else if (menu is MuseumMenu)
            {
                DonationMenuPatches.museumQueryKey = " ";
            }
            else if (menu is PondQueryMenu)
            {
                MenuPatches.pondQueryMenuQuery = " ";
            }

            InventoryUtils.Cleanup();
            TextBoxPatch.activeTextBoxes = "";
        }
    }
}
