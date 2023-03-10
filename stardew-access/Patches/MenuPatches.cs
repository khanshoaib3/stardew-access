using Microsoft.Xna.Framework;
using stardew_access.Features;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class MenuPatches
    {
        internal static string currentLevelUpTitle = " ";
        internal static bool firstTimeInNamingMenu = true;
        internal static bool isNarratingPondInfo = false;
        internal static string tailoringMenuQuery = " ";
        internal static string pondQueryMenuQuery = " ";
        internal static int prevSlotIndex = -999;

        internal static void PondQueryMenuPatch(PondQueryMenu __instance, StardewValley.Object ____fishItem, FishPond ____pond, string ____statusText, bool ___confirmingEmpty)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                bool isPrimaryInfoKeyPressed = MainClass.Config.PrimaryInfoKey.JustPressed();
                string toSpeak = " ", extra = "";

                if (___confirmingEmpty)
                {
                    if (__instance.yesButton != null && __instance.yesButton.containsPoint(x, y))
                        toSpeak = "Confirm button";
                    else if (__instance.noButton != null && __instance.noButton.containsPoint(x, y))
                        toSpeak = "Cancel button";
                }
                else
                {
                    if (isPrimaryInfoKeyPressed && !isNarratingPondInfo)
                    {
                        string pond_name_text = Game1.content.LoadString("Strings\\UI:PondQuery_Name", ____fishItem.DisplayName);
                        string population_text = Game1.content.LoadString("Strings\\UI:PondQuery_Population", string.Concat(____pond.FishCount), ____pond.maxOccupants.Value);
                        bool has_unresolved_needs = ____pond.neededItem.Value != null && ____pond.HasUnresolvedNeeds() && !____pond.hasCompletedRequest.Value;
                        string bring_text = "";

                        if (has_unresolved_needs && ____pond.neededItem.Value != null)
                            bring_text = Game1.content.LoadString("Strings\\UI:PondQuery_StatusRequest_Bring") + $": {____pond.neededItemCount} {____pond.neededItem.Value.DisplayName}";

                        extra = $"{pond_name_text} {population_text} {bring_text} Status: {____statusText}";
                        pondQueryMenuQuery = " ";

                        isNarratingPondInfo = true;
                        Task.Delay(200).ContinueWith(_ => { isNarratingPondInfo = false; });
                    }

                    if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                        toSpeak = "Ok button";
                    else if (__instance.changeNettingButton != null && __instance.changeNettingButton.containsPoint(x, y))
                        toSpeak = "Change netting button";
                    else if (__instance.emptyButton != null && __instance.emptyButton.containsPoint(x, y))
                        toSpeak = "Empty pond button";
                }

                if (pondQueryMenuQuery != toSpeak)
                {
                    pondQueryMenuQuery = toSpeak;
                    MainClass.ScreenReader.Say(extra + " \n\t" + toSpeak, true);
                }
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void TailoringMenuPatch(TailoringMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                string toSpeak = " ";

                if (__instance.leftIngredientSpot != null && __instance.leftIngredientSpot.containsPoint(x, y))
                {
                    if (__instance.leftIngredientSpot.item == null)
                    {
                        toSpeak = "Input cloth here";
                    }
                    else
                    {
                        Item item = __instance.leftIngredientSpot.item;
                        toSpeak = $"Cloth slot: {item.Stack} {item.DisplayName}";
                    }
                }
                else if (__instance.rightIngredientSpot != null && __instance.rightIngredientSpot.containsPoint(x, y))
                {
                    if (__instance.rightIngredientSpot.item == null)
                    {
                        toSpeak = "Input ingredient here";
                    }
                    else
                    {
                        Item item = __instance.rightIngredientSpot.item;
                        toSpeak = $"Ingredient slot: {item.Stack} {item.DisplayName}";
                    }
                }
                else if (__instance.startTailoringButton != null && __instance.startTailoringButton.containsPoint(x, y))
                {
                    toSpeak = "Star tailoring button";
                }
                else if (__instance.trashCan != null && __instance.trashCan.containsPoint(x, y))
                {
                    toSpeak = "Trashcan";
                }
                else if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                {
                    toSpeak = "ok button";
                }
                else if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
                {
                    toSpeak = "drop item";
                }
                else if (__instance.equipmentIcons.Count > 0 && __instance.equipmentIcons[0].containsPoint(x, y))
                {
                    toSpeak = "Hat Slot";

                    if (Game1.player.hat.Value != null)
                        toSpeak = $"{toSpeak}: {Game1.player.hat.Value.DisplayName}";
                }
                else if (__instance.equipmentIcons.Count > 0 && __instance.equipmentIcons[1].containsPoint(x, y))
                {
                    toSpeak = "Shirt Slot";

                    if (Game1.player.shirtItem.Value != null)
                        toSpeak = $"{toSpeak}: {Game1.player.shirtItem.Value.DisplayName}";
                }
                else if (__instance.equipmentIcons.Count > 0 && __instance.equipmentIcons[2].containsPoint(x, y))
                {
                    toSpeak = "Pants Slot";

                    if (Game1.player.pantsItem.Value != null)
                        toSpeak = $"{toSpeak}: {Game1.player.pantsItem.Value.DisplayName}";
                }

                if (InventoryUtils.narrateHoveredSlot(__instance.inventory, __instance.inventory.inventory, __instance.inventory.actualInventory, x, y))
                    return;


                if (tailoringMenuQuery != toSpeak)
                {
                    tailoringMenuQuery = toSpeak;
                    MainClass.ScreenReader.Say(toSpeak, true);

                    if (__instance.dropItemInvisibleButton != null && __instance.dropItemInvisibleButton.containsPoint(x, y))
                        Game1.playSound("drop_item");
                }
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

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
                if (firstTimeInNamingMenu)
                {
                    firstTimeInNamingMenu = false;
                    ___textBox.Selected = false;
                }

                if (TextBoxPatch.isAnyTextBoxActive) return;

                string toSpeak = "";
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                bool isEscPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape); // For escaping/unselecting from the animal name text box

                if (__instance.textBoxCC != null && __instance.textBoxCC.containsPoint(x, y))
                    toSpeak = $"{___title} text box";
                else if (__instance.doneNamingButton != null && __instance.doneNamingButton.containsPoint(x, y))
                    toSpeak = $"Done naming button";
                else if (__instance.randomButton != null && __instance.randomButton.containsPoint(x, y))
                    toSpeak = $"Random button";

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
    }
}
