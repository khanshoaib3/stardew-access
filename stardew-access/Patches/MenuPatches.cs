using Microsoft.Xna.Framework;
using stardew_access.Features;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class MenuPatches
    {
        private static string currentLetterText = " ";
        private static string currentLevelUpTitle = " ";
        public static Vector2? prevTile = null;

        internal static bool PlaySoundPatch(string cueName)
        {
            try
            {
                if (!Context.IsPlayerFree)
                    return true;

                if (!Game1.player.isMoving())
                    return true;

                if (cueName == "grassyStep" || cueName == "sandyStep" || cueName == "snowyStep" || cueName == "stoneStep" || cueName == "thudStep" || cueName == "woodyStep")
                {
                    Vector2 nextTile = CurrentPlayer.getNextTile();
                    if (ReadTile.isCollidingAtTile((int)nextTile.X, (int)nextTile.Y))
                    {
                        if (prevTile != nextTile)
                        {
                            prevTile = nextTile;
                            //Game1.playSound("colliding");
                        }
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }

            return true;
        }

        internal static void LanguageSelectionMenuPatch(LanguageSelectionMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(), y = Game1.getMouseY(); // Mouse x and y position

                if (__instance.nextPageButton != null && __instance.nextPageButton.containsPoint(x, y))
                {
                    MainClass.GetScreenReader().SayWithMenuChecker($"Next Page Button", true);
                    return;
                }

                if (__instance.previousPageButton != null && __instance.previousPageButton.containsPoint(x, y))
                {
                    MainClass.GetScreenReader().SayWithMenuChecker($"Previous Page Button", true);
                    return;
                }

                for (int i = 0; i < __instance.languages.Count; i++)
                {
                    if (__instance.languages[i].containsPoint(x, y))
                    {
                        MainClass.GetScreenReader().SayWithMenuChecker($"{__instance.languageList[i]} Button", true);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void MineElevatorMenuPatch(List<ClickableComponent> ___elevators)
        {
            try
            {
                int x = Game1.getMouseX(), y = Game1.getMouseY(); // Mouse x and y position
                for (int i = 0; i < ___elevators.Count; i++)
                {
                    if (___elevators[i].containsPoint(x, y))
                    {
                        MainClass.GetScreenReader().SayWithMenuChecker($"{___elevators[i].name} level", true);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void NamingMenuPatch(NamingMenu __instance, string title, TextBox ___textBox)
        {
            try
            {
                __instance.textBoxCC.snapMouseCursor();
                ___textBox.SelectMe();
                string toSpeak = $"{title}";

                MainClass.GetScreenReader().SayWithChecker(toSpeak, true);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void ConfirmationDialogPatch(ConfirmationDialog __instance, string ___message)
        {
            try
            {
                int x = Game1.getMouseX(), y = Game1.getMouseY();

                MainClass.GetScreenReader().SayWithMenuChecker(___message, true);
                if (__instance.okButton.containsPoint(x, y))
                {
                    MainClass.GetScreenReader().SayWithMenuChecker("Ok Button", false);
                }
                else if (__instance.cancelButton.containsPoint(x, y))
                {
                    MainClass.GetScreenReader().SayWithMenuChecker("Cancel Button", false);
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void LevelUpMenuPatch(LevelUpMenu __instance, List<int> ___professionsToChoose, List<string> ___leftProfessionDescription, List<string> ___rightProfessionDescription, List<string> ___extraInfoForLevel, List<CraftingRecipe> ___newCraftingRecipes, string ___title, bool ___isActive, bool ___isProfessionChooser)
        {
            try
            {
                int x = Game1.getMouseX(), y = Game1.getMouseY();
                string leftProfession = " ", rightProfession = " ", extraInfo = " ", newCraftingRecipe = " ", toSpeak = " ";

                bool isOpenBracketPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.OemOpenBrackets); // for left click
                bool isLeftCtrlPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl);
                bool isEnterPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter);

                if (!__instance.informationUp)
                {
                    return;
                }
                if (__instance.isProfessionChooser)
                {
                    if (___professionsToChoose.Count() == 0)
                    {
                        return;
                    }
                    for (int j = 0; j < ___leftProfessionDescription.Count; j++)
                    {
                        leftProfession += ___leftProfessionDescription[j] + ", ";
                    }
                    for (int i = 0; i < ___rightProfessionDescription.Count; i++)
                    {
                        rightProfession += ___rightProfessionDescription[i] + ", ";
                    }

                    if (__instance.leftProfession.containsPoint(x, y))
                    {
                        if (isOpenBracketPressed || (isLeftCtrlPressed && isEnterPressed && __instance.readyToClose()))
                        {
                            Game1.player.professions.Add(___professionsToChoose[0]);
                            __instance.getImmediateProfessionPerk(___professionsToChoose[0]);
                            ___isActive = false;
                            __instance.informationUp = false;
                            ___isProfessionChooser = false;
                            __instance.RemoveLevelFromLevelList();
                            __instance.exitThisMenu();
                            return;
                        }

                        toSpeak = $"Selected: {leftProfession} Left click to choose.";
                    }

                    if (__instance.rightProfession.containsPoint(x, y))
                    {
                        if (isOpenBracketPressed || (isLeftCtrlPressed && isEnterPressed && __instance.readyToClose()))
                        {
                            Game1.player.professions.Add(___professionsToChoose[1]);
                            __instance.getImmediateProfessionPerk(___professionsToChoose[1]);
                            ___isActive = false;
                            __instance.informationUp = false;
                            ___isProfessionChooser = false;
                            __instance.RemoveLevelFromLevelList();
                            __instance.exitThisMenu();
                            return;
                        }

                        toSpeak = $"Selected: {rightProfession} Left click to choose.";
                    }
                }
                else
                {
                    foreach (string s2 in ___extraInfoForLevel)
                    {
                        extraInfo += s2 + ", ";
                    }
                    foreach (CraftingRecipe s in ___newCraftingRecipes)
                    {
                        string cookingOrCrafting = Game1.content.LoadString("Strings\\UI:LearnedRecipe_" + (s.isCookingRecipe ? "cooking" : "crafting"));
                        string message = Game1.content.LoadString("Strings\\UI:LevelUp_NewRecipe", cookingOrCrafting, s.DisplayName);

                        newCraftingRecipe += $"{message}, ";
                    }
                }

                if (__instance.okButton.containsPoint(x, y))
                {
                    if (isOpenBracketPressed || (isLeftCtrlPressed && isEnterPressed))
                        __instance.okButtonClicked();

                    toSpeak = $"{___title} {extraInfo} {newCraftingRecipe}. Left click to close.";
                }

                if (toSpeak != " ")
                    MainClass.GetScreenReader().SayWithMenuChecker(toSpeak, true);
                else if (__instance.isProfessionChooser && currentLevelUpTitle != $"{___title}. Select a new profession.")
                {
                    MainClass.GetScreenReader().SayWithMenuChecker($"{___title}. Select a new profession.", true);
                    currentLevelUpTitle = $"{___title}. Select a new profession.";
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void ShippingMenuPatch(ShippingMenu __instance, List<int> ___categoryTotals)
        {
            try
            {
                bool isLeftControlPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl);
                bool isOpenBracketPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.OemOpenBrackets); // for left click
                bool isEnterPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Enter);

                if (__instance.currentPage == -1)
                {
                    int total = ___categoryTotals[5];
                    string toSpeak;
                    if (__instance.okButton.containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                    {
                        // Perform Left Click
                        if (isOpenBracketPressed || (isLeftControlPressed && isEnterPressed))
                        {
                            Game1.activeClickableMenu.receiveLeftClick(Game1.getMouseX(true), Game1.getMouseY(true));
                        }
                        toSpeak = $"{total}g in total. Press left mouse button to save.";
                        MainClass.GetScreenReader().SayWithChecker(toSpeak, true);
                    }
                    for (int i = 0; i < __instance.categories.Count; i++)
                    {
                        if (__instance.categories[i].containsPoint(Game1.getMouseX(), Game1.getMouseY()))
                        {
                            toSpeak = $"Money recieved from {__instance.getCategoryName(i)}: {___categoryTotals[i]}g.";
                            MainClass.GetScreenReader().SayWithChecker(toSpeak, true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void LetterViewerMenuPatch(LetterViewerMenu __instance)
        {
            try
            {
                if (!__instance.IsActive())
                    return;

                int x = Game1.getMousePosition().X, y = Game1.getMousePosition().Y;
                #region Texts in the letter
                string message = __instance.mailMessage[__instance.page];

                string toSpeak = $"{message}";

                if (__instance.ShouldShowInteractable())
                {
                    if (__instance.moneyIncluded > 0)
                    {
                        string moneyText = Game1.content.LoadString("Strings\\UI:LetterViewer_MoneyIncluded", __instance.moneyIncluded);
                        toSpeak += $"\t\n\t ,Included money: {moneyText}";
                    }
                    else if (__instance.learnedRecipe != null && __instance.learnedRecipe.Length > 0)
                    {
                        string recipeText = Game1.content.LoadString("Strings\\UI:LetterViewer_LearnedRecipe", __instance.cookingOrCrafting);
                        toSpeak += $"\t\n\t ,Learned Recipe: {recipeText}";
                    }
                }

                if (currentLetterText != toSpeak)
                {
                    currentLetterText = toSpeak;

                    // snap mouse to accept quest button
                    if (__instance.acceptQuestButton != null && __instance.questID != -1)
                    {
                        toSpeak += "\t\n Left click to accept quest.";
                        __instance.acceptQuestButton.snapMouseCursorToCenter();
                    }
                    if (__instance.mailMessage.Count > 1)
                        toSpeak = $"Page {__instance.page + 1} of {__instance.mailMessage.Count}:\n\t{toSpeak}";

                    MainClass.GetScreenReader().Say(toSpeak, false);
                }
                #endregion

                #region Narrate items given in the mail
                if (__instance.ShouldShowInteractable())
                {
                    foreach (ClickableComponent c in __instance.itemsToGrab)
                    {
                        string name = c.name;
                        string label = c.label;

                        if (c.containsPoint(x, y))
                            MainClass.GetScreenReader().SayWithChecker($"Grab: {name} \t\n {label}", false);
                    }
                }
                #endregion

                #region Narrate buttons
                if (__instance.backButton != null && __instance.backButton.visible && __instance.backButton.containsPoint(x, y))
                    MainClass.GetScreenReader().SayWithChecker($"Previous page button", false);

                if (__instance.forwardButton != null && __instance.forwardButton.visible && __instance.forwardButton.containsPoint(x, y))
                    MainClass.GetScreenReader().SayWithChecker($"Next page button", false);

                #endregion
            }
            catch (Exception e)
            {

                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void Game1ExitActiveMenuPatch()
        {
            try
            {
                if (Game1.activeClickableMenu is GameMenu)
                {
                    GameMenuPatches.gameMenuQueryKey = "";
                    GameMenuPatches.craftingPageQueryKey = "";
                    GameMenuPatches.inventoryPageQueryKey = "";
                    GameMenuPatches.exitPageQueryKey = "";
                    GameMenuPatches.optionsPageQueryKey = "";
                    GameMenuPatches.socialPageQuery = "";
                    GameMenuPatches.currentSelectedCraftingRecipe = -1;
                    GameMenuPatches.isSelectingRecipe = false;
                }

                if (Game1.activeClickableMenu is JunimoNoteMenu)
                {
                    GameMenuPatches.currentIngredientListItem = -1;
                    GameMenuPatches.currentIngredientInputSlot = -1;
                    GameMenuPatches.currentInventorySlot = -1;
                    GameMenuPatches.junimoNoteMenuQuery = "";
                }

                if (Game1.activeClickableMenu is ShopMenu)
                {
                    GameMenuPatches.shopMenuQueryKey = "";
                }

                if (Game1.activeClickableMenu is ItemGrabMenu)
                {
                    GameMenuPatches.itemGrabMenuQueryKey = "";
                }

                GameMenuPatches.hoveredItemQueryKey = "";
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void IClickableMenuOnExitPatch(IClickableMenu __instance)
        {
            try
            {
                if (__instance is GeodeMenu)
                {
                    GameMenuPatches.geodeMenuQueryKey = "";
                }

                if (__instance is ItemGrabMenu)
                {
                    GameMenuPatches.itemGrabMenuQueryKey = "";
                }

                if (__instance is ShopMenu)
                {
                    GameMenuPatches.shopMenuQueryKey = "";
                }

                if (__instance is CarpenterMenu)
                {
                    BuildingNAnimalMenuPatches.carpenterMenuQuery = "";
                    BuildingNAnimalMenuPatches.isUpgrading = false;
                    BuildingNAnimalMenuPatches.isDemolishing = false;
                    BuildingNAnimalMenuPatches.isPainting = false;
                    BuildingNAnimalMenuPatches.isMoving = false;
                    BuildingNAnimalMenuPatches.isConstructing = false;
                    BuildingNAnimalMenuPatches.carpenterMenu = null;
                }

                if (__instance is PurchaseAnimalsMenu)
                {
                    BuildingNAnimalMenuPatches.purchaseAnimalMenuQuery = "";
                    BuildingNAnimalMenuPatches.firstTimeInNamingMenu = true;
                    BuildingNAnimalMenuPatches.purchaseAnimalsMenu = null;
                }

                if (__instance is DialogueBox)
                {
                    DialoguePatches.isDialogueAppearingFirstTime = true;
                    DialoguePatches.currentDialogue = " ";
                }

                GameMenuPatches.hoveredItemQueryKey = "";
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void ExitEventPatch()
        {
            if (MainClass.GetScreenReader() != null)
                MainClass.GetScreenReader().CloseScreenReader();
        }
        internal static void resetGlobalVars()
        {
            currentLetterText = " ";
            currentLevelUpTitle = " ";
        }
    }
}
