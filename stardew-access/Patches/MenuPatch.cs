using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class MenuPatch
    {
        private static int saveGameIndex = -1;
        private static bool isRunning = false;
        private static string currentLetterText = " ";

        internal static void LetterViewerMenuPatch(LetterViewerMenu __instance)
        {
            try
            {
                if (!__instance.IsActive())
                    return;

                #region Texts in the letter
                string title = __instance.mailTitle;
                string message = __instance.mailMessage[__instance.page];

                string toSpeak = $"{title} \t\n\t {message}.";

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

                if (__instance.ShouldShowInteractable() && __instance.questID != -1)
                {
                    toSpeak += "\t\n\t ,Close this menu to accept or press left click button";
                }

                if (currentLetterText != toSpeak)
                {
                    currentLetterText = toSpeak;
                    ScreenReader.say(toSpeak, false);
                }
                #endregion

                #region Narrate items given in the mail
                if (__instance.ShouldShowInteractable())
                {
                    foreach (ClickableComponent c in __instance.itemsToGrab)
                    {
                        string name = c.name;
                        string label = c.label;

                        if (c.containsPoint(Game1.getMousePosition().X, Game1.getMousePosition().Y))
                            ScreenReader.sayWithChecker($"Grab: {name} \t\n {label}", false);
                    }
                } 
                #endregion
            }
            catch (Exception e)
            {

                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void TitleMenuPatch(TitleMenu __instance)
        {
            try
            {
                string toSpeak = "";

                __instance.buttons.ForEach(component =>
                {
                    if (component.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                    {
                        string name = component.name;
                        string label = component.label;
                        toSpeak = $"{name} {label} Button";
                    }
                });

                if(__instance.muteMusicButton.containsPoint(Game1.getMousePosition(true).X,Game1.getMousePosition(true).Y))
                {
                    toSpeak = "Mute Music Button";
                }

                if (__instance.aboutButton.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    toSpeak = "About Button";
                }

                if (__instance.languageButton.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    toSpeak = "Language Button";
                }

                if (__instance.windowedButton.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    toSpeak = "Fullscreen toggle Button";
                }

                if (TitleMenu.subMenu != null && __instance.backButton.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    string text = "Back Button";
                    ScreenReader.sayWithChecker(text, true);
                }

                if (TitleMenu.subMenu == null && toSpeak != "")
                    ScreenReader.sayWithChecker(toSpeak, true);
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void LoadGameMenuPatch(LoadGameMenu.SaveFileSlot __instance, LoadGameMenu ___menu, int i)
        {
            try
            {
                if (___menu.slotButtons[i].containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    if (__instance.Farmer == null)
                        return;

                    if (___menu.deleteButtons[i].containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                    {
                        // Fix for delete button hover text not narrating
                        ScreenReader.sayWithChecker($"Delete {__instance.Farmer.farmName} Farm", true);
                        return;
                    }

                    String farmerName = __instance.Farmer.Name;
                    String farmName = __instance.Farmer.farmName;
                    String money = __instance.Farmer.Money.ToString();
                    String hoursPlayed = Utility.getHoursMinutesStringFromMilliseconds(__instance.Farmer.millisecondsPlayed);
                    string dateStringForSaveGame = ((!__instance.Farmer.dayOfMonthForSaveGame.HasValue || 
                        !__instance.Farmer.seasonForSaveGame.HasValue || 
                        !__instance.Farmer.yearForSaveGame.HasValue) ? __instance.Farmer.dateStringForSaveGame : Utility.getDateStringFor(__instance.Farmer.dayOfMonthForSaveGame.Value, __instance.Farmer.seasonForSaveGame.Value, __instance.Farmer.yearForSaveGame.Value));

                    string toSpeak = $"{farmName} Farm, \t\n Farmer:{farmerName}, \t\nMoney:{money}, \t\nHours Played:{hoursPlayed}, \t\nDate:{dateStringForSaveGame}";

                    ScreenReader.sayWithChecker(toSpeak, true);
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        internal static void NewGameMenuPatch(CharacterCustomization __instance, TextBox ___nameBox, TextBox ___farmnameBox, TextBox ___favThingBox, ClickableTextureComponent ___skipIntroButton, ClickableTextureComponent ___okButton, ClickableComponent ___backButton)
        {
            try
            {
                if (__instance.source != CharacterCustomization.Source.NewGame)
                    return;


                bool isNextArrowPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right);
                bool isPrevArrowPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left);
                if (isNextArrowPressed && !isRunning)
                {
                    _ = CycleThroughItems(true, ___nameBox, ___farmnameBox, ___favThingBox, ___skipIntroButton, ___okButton, ___backButton);
                }
                else if (isPrevArrowPressed && !isRunning)
                {
                    _ = CycleThroughItems(false, ___nameBox, ___farmnameBox, ___favThingBox, ___skipIntroButton, ___okButton, ___backButton);
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }

        private static async Task CycleThroughItems(bool increase, TextBox ___nameBox, TextBox ___farmnameBox, TextBox ___favThingBox, ClickableTextureComponent ___skipIntroButton, ClickableTextureComponent ___okButton, ClickableComponent ___backButton)
        {
            isRunning = true;
            if (increase)
            {
                saveGameIndex++;
                if (saveGameIndex > 6)
                    saveGameIndex = 0; 
            } else
            {
                saveGameIndex--;
                if (saveGameIndex < 0)
                    saveGameIndex = 6;
            }

            await Task.Delay(200);

            switch (saveGameIndex)
            {
                case 0:
                    {
                        Rectangle bounds = new Rectangle(___nameBox.X, ___nameBox.Y, ___nameBox.Width, ___nameBox.Height);
                        Game1.input.SetMousePosition(bounds.Center.X, bounds.Center.Y);
                        ScreenReader.say("Enter Farmer's Name", true);
                    }
                    break;

                case 1:
                    {
                        Rectangle bounds = new Rectangle(___farmnameBox.X, ___farmnameBox.Y, ___farmnameBox.Width, ___farmnameBox.Height);
                        Game1.input.SetMousePosition(bounds.Center.X, bounds.Center.Y);
                        ScreenReader.say("Enter Farm's Name", true);
                    }
                    break;
                case 3:
                    {
                        Rectangle bounds = new Rectangle(___favThingBox.X, ___favThingBox.Y, ___favThingBox.Width, ___favThingBox.Height);
                        Game1.input.SetMousePosition(bounds.Center.X, bounds.Center.Y);
                        ScreenReader.say("Enter Favourite Thing", true);
                    }
                    break;
                case 4:
                    {
                        ___skipIntroButton.snapMouseCursor();
                        ScreenReader.say("Skip Intro Button", true);
                    }
                    break;
                case 5:
                    {
                        ___okButton.snapMouseCursor();
                        ScreenReader.say("Ok Button", true);
                    }
                    break;
                case 6:
                    {
                        ___backButton.snapMouseCursor();
                        ScreenReader.say("Back Button", true);
                    }
                    break;
            }

            isRunning = false;
        }

        internal static void ExitPagePatch(ExitPage __instance)
        {
            try
            {
                if (__instance.exitToTitle.visible &&
                        __instance.exitToTitle.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    ScreenReader.sayWithChecker("Exit to Title Button", true);
                }
                if (__instance.exitToDesktop.visible &&
                    __instance.exitToDesktop.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                {
                    ScreenReader.sayWithChecker("Exit to Desktop Button", true);
                }
            }
            catch (Exception e)
            {

                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }
    }
}
