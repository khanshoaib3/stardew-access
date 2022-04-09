using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using stardew_access.Features;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class MenuPatches
    {
        private static string currentLevelUpTitle = " ";
        internal static bool firstTimeInNamingMenu = true;
        private static string animalQueryMenuQuery = " ";
        public static Vector2? prevTile = null;

        internal static void ChooseFromListMenuPatch(ChooseFromListMenu __instance, List<string> ___options, int ___index, bool ___isJukebox)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                string toSpeak = "";

                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                    toSpeak = "Select " + (___isJukebox ? Utility.getSongTitleFromCueName(___options[___index]) : ___options[___index]) + " button";
                else if (__instance.cancelButton != null && __instance.cancelButton.containsPoint(x, y))
                    toSpeak = "Cancel button";
                else if (__instance.backButton != null && __instance.backButton.containsPoint(x, y))
                    toSpeak = "Previous option: " + (___isJukebox ? Utility.getSongTitleFromCueName(___options[Math.Max(0, ___index - 1)]) : ___options[Math.Max(0, ___index - 1)]) + " button";
                else if (__instance.forwardButton != null && __instance.forwardButton.containsPoint(x, y))
                    toSpeak = "Next option: " + (___isJukebox ? Utility.getSongTitleFromCueName(___options[Math.Min(___options.Count, ___index + 1)]) : ___options[Math.Min(___options.Count, ___index + 1)]) + " button";

                MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void AnimalQueryMenuPatch(AnimalQueryMenu __instance, bool ___confirmingSell, FarmAnimal ___animal, TextBox ___textBox, string ___parentName)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                bool isCPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.C); // For narrating animal details
                bool isEscPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape); // For escaping/unselecting from the animal name text box
                string toSpeak = " ", details = " ";

                if (___textBox.Selected)
                {
                    toSpeak = ___textBox.Text;

                    if (isEscPressed)
                    {
                        ___textBox.Selected = false;
                    }
                }
                else
                {
                    if (isCPressed)
                    {
                        string name = ___animal.displayName;
                        string type = ___animal.displayType;
                        int age = (___animal.GetDaysOwned() + 1) / 28 + 1;
                        string ageText = (age <= 1) ? Game1.content.LoadString("Strings\\UI:AnimalQuery_Age1") : Game1.content.LoadString("Strings\\UI:AnimalQuery_AgeN", age);
                        string parent = "";
                        if ((int)___animal.age.Value < (byte)___animal.ageWhenMature.Value)
                        {
                            ageText += Game1.content.LoadString("Strings\\UI:AnimalQuery_AgeBaby");
                        }
                        if (___parentName != null)
                        {
                            parent = Game1.content.LoadString("Strings\\UI:AnimalQuery_Parent", ___parentName);
                        }

                        details = $"Name: {name} Type: {type} \n\t Age: {ageText} {parent}";
                        animalQueryMenuQuery = " ";
                    }

                    if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                        toSpeak = "OK button";
                    else if (__instance.sellButton != null && __instance.sellButton.containsPoint(x, y))
                        toSpeak = $"Sell for {___animal.getSellPrice()}g button";
                    else if (___confirmingSell && __instance.yesButton != null && __instance.yesButton.containsPoint(x, y))
                        toSpeak = "Confirm selling animal";
                    else if (___confirmingSell && __instance.noButton != null && __instance.noButton.containsPoint(x, y))
                        toSpeak = "Cancel selling animal";
                    else if (__instance.moveHomeButton != null && __instance.moveHomeButton.containsPoint(x, y))
                        toSpeak = "Change home building button";
                    else if (__instance.allowReproductionButton != null && __instance.allowReproductionButton.containsPoint(x, y))
                        toSpeak = ((___animal.allowReproduction.Value) ? "Enabled" : "Disabled") + " allow reproduction button";
                    else if (__instance.textBoxCC != null && __instance.textBoxCC.containsPoint(x, y))
                        toSpeak = "Animal name text box";
                }

                if (animalQueryMenuQuery != toSpeak)
                {
                    animalQueryMenuQuery = toSpeak;
                    MainClass.ScreenReader.Say($"{details} {toSpeak}", true);
                }
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

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
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (__instance.nextPageButton != null && __instance.nextPageButton.containsPoint(x, y))
                {
                    MainClass.ScreenReader.SayWithMenuChecker($"Next Page Button", true);
                    return;
                }

                if (__instance.previousPageButton != null && __instance.previousPageButton.containsPoint(x, y))
                {
                    MainClass.ScreenReader.SayWithMenuChecker($"Previous Page Button", true);
                    return;
                }

                for (int i = 0; i < __instance.languages.Count; i++)
                {
                    if (__instance.languages[i].containsPoint(x, y))
                    {
                        MainClass.ScreenReader.SayWithMenuChecker($"{__instance.languageList[i]} Button", true);
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
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                for (int i = 0; i < ___elevators.Count; i++)
                {
                    if (___elevators[i].containsPoint(x, y))
                    {
                        MainClass.ScreenReader.SayWithMenuChecker($"{___elevators[i].name} level", true);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void TitleTextInputMenuPatch(TitleTextInputMenu __instance)
        {
            try
            {
                string toSpeak = "";
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                if (__instance.pasteButton != null && __instance.pasteButton.containsPoint(x, y))
                    toSpeak = $"Paste button";

                if (toSpeak != "")
                    MainClass.ScreenReader.SayWithChecker(toSpeak, true);
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void NamingMenuPatch(NamingMenu __instance, TextBox ___textBox, string ___title)
        {
            try
            {
                string toSpeak = "";
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                bool isEscPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape); // For escaping/unselecting from the animal name text box

                if (firstTimeInNamingMenu)
                {
                    firstTimeInNamingMenu = false;
                    ___textBox.Selected = false;
                }

                if (___textBox.Selected)
                {
                    ___textBox.Update();
                    toSpeak = ___textBox.Text;

                    if (isEscPressed)
                    {
                        ___textBox.Selected = false;
                    }
                }
                else
                {
                    if (__instance.textBoxCC != null && __instance.textBoxCC.containsPoint(x, y))
                        toSpeak = $"{___title} text box";
                    else if (__instance.doneNamingButton != null && __instance.doneNamingButton.containsPoint(x, y))
                        toSpeak = $"Done naming button";
                    else if (__instance.randomButton != null && __instance.randomButton.containsPoint(x, y))
                        toSpeak = $"Random button";
                }

                if (toSpeak != "")
                    MainClass.ScreenReader.SayWithChecker(toSpeak, true);
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
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
                string toSpeak = ___message;

                if (__instance.okButton.containsPoint(x, y))
                {
                    toSpeak += "\n\tOk Button";
                }
                else if (__instance.cancelButton.containsPoint(x, y))
                {
                    toSpeak += "\n\tCancel Button";
                }

                MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
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
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
                string leftProfession = " ", rightProfession = " ", extraInfo = " ", newCraftingRecipe = " ", toSpeak = " ";

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
                        if ((MainClass.Config.LeftClickMainKey.JustPressed() || MainClass.Config.LeftClickAlternateKey.JustPressed()) && __instance.readyToClose())
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
                        if ((MainClass.Config.LeftClickMainKey.JustPressed() || MainClass.Config.LeftClickAlternateKey.JustPressed()) && __instance.readyToClose())
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
                    if (MainClass.Config.LeftClickMainKey.JustPressed() || MainClass.Config.LeftClickAlternateKey.JustPressed())
                        __instance.okButtonClicked();

                    toSpeak = $"{___title} {extraInfo} {newCraftingRecipe}. Left click to close.";
                }

                if (toSpeak != " ")
                    MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
                else if (__instance.isProfessionChooser && currentLevelUpTitle != $"{___title}. Select a new profession.")
                {
                    MainClass.ScreenReader.SayWithMenuChecker($"{___title}. Select a new profession.", true);
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

                if (__instance.currentPage == -1)
                {
                    int total = ___categoryTotals[5];
                    string toSpeak;
                    if (__instance.okButton.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                    {
                        // Perform Left Click
                        if (MainClass.Config.LeftClickMainKey.JustPressed() || MainClass.Config.LeftClickAlternateKey.JustPressed())
                        {
                            Game1.activeClickableMenu.receiveLeftClick(Game1.getMouseX(true), Game1.getMouseY(true));
                        }
                        toSpeak = $"{total}g in total. Press left mouse button to save.";
                        MainClass.ScreenReader.SayWithChecker(toSpeak, true);
                    }
                    for (int i = 0; i < __instance.categories.Count; i++)
                    {
                        if (__instance.categories[i].containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                        {
                            toSpeak = $"Money recieved from {__instance.getCategoryName(i)}: {___categoryTotals[i]}g.";
                            MainClass.ScreenReader.SayWithChecker(toSpeak, true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        #region Cleanup on exitting a menu
        internal static void Game1ExitActiveMenuPatch()
        {
            try
            {
                Cleanup(Game1.activeClickableMenu);
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
                Cleanup(__instance);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void Cleanup(IClickableMenu menu)
        {
            if (menu is LetterViewerMenu)
            {
                DialoguePatches.currentLetterText = " ";
            }

            if (menu is LevelUpMenu)
            {
                currentLevelUpTitle = " ";
            }

            if (menu is Billboard)
            {
                QuestPatches.currentDailyQuestText = " ";
            }

            if (menu is GameMenu)
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

            if (menu is JunimoNoteMenu)
            {
                GameMenuPatches.currentIngredientListItem = -1;
                GameMenuPatches.currentIngredientInputSlot = -1;
                GameMenuPatches.currentInventorySlot = -1;
                GameMenuPatches.junimoNoteMenuQuery = "";
            }

            if (menu is ShopMenu)
            {
                GameMenuPatches.shopMenuQueryKey = "";
            }

            if (menu is ItemGrabMenu)
            {
                GameMenuPatches.itemGrabMenuQueryKey = "";
            }

            if (menu is GeodeMenu)
            {
                GameMenuPatches.geodeMenuQueryKey = "";
            }

            if (menu is CarpenterMenu)
            {
                BuildingNAnimalMenuPatches.carpenterMenuQuery = "";
                BuildingNAnimalMenuPatches.isUpgrading = false;
                BuildingNAnimalMenuPatches.isDemolishing = false;
                BuildingNAnimalMenuPatches.isPainting = false;
                BuildingNAnimalMenuPatches.isMoving = false;
                BuildingNAnimalMenuPatches.isConstructing = false;
                BuildingNAnimalMenuPatches.carpenterMenu = null;
            }

            if (menu is PurchaseAnimalsMenu)
            {
                BuildingNAnimalMenuPatches.purchaseAnimalMenuQuery = "";
                BuildingNAnimalMenuPatches.firstTimeInNamingMenu = true;
                BuildingNAnimalMenuPatches.purchaseAnimalsMenu = null;
            }

            if (menu is DialogueBox)
            {
                DialoguePatches.isDialogueAppearingFirstTime = true;
                DialoguePatches.currentDialogue = " ";
            }

            GameMenuPatches.hoveredItemQueryKey = "";
        }
        #endregion

        internal static void ExitEventPatch()
        {
            if (MainClass.ScreenReader != null)
                MainClass.ScreenReader.CloseScreenReader();
        }
    }
}
