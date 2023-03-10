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
        internal static int prevSlotIndex = -999;




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
