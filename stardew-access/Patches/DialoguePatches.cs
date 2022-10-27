using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class DialoguePatches
    {
        internal static string currentLetterText = " ";
        internal static string currentDialogue = " ";
        internal static bool isDialogueAppearingFirstTime = true;

        internal static void DialoguePatch(DialogueBox __instance, SpriteBatch b)
        {
            try
            {
                if (__instance.transitioning)
                    return;

                if (__instance.characterDialogue != null)
                {
                    // For Normal Character dialogues
                    Dialogue dialogue = __instance.characterDialogue;
                    string speakerName = dialogue.speaker.displayName;
                    List<Response> responses = __instance.responses;
                    string toSpeak = " ";
                    string dialogueText = "";
                    string response = "";
                    bool hasResponses = dialogue.isCurrentDialogueAQuestion();

                    dialogueText = $"{speakerName} said {__instance.getCurrentString()}";

                    if (hasResponses)
                    {
                        if (__instance.selectedResponse >= 0 && __instance.selectedResponse < responses.Count)
                            response = $"{__instance.selectedResponse + 1}: {responses[__instance.selectedResponse].responseText}";
                        else
                            // When the dialogue is not finished writing then the selectedResponse is <0 and this results
                            // in the first response not being detcted, so this sets the first response option to be the default
                            // if the current dialogue is a question or has responses
                            response = $"1: {responses[0].responseText}";
                    }

                    if (hasResponses)
                    {
                        if (currentDialogue != response)
                        {
                            currentDialogue = response;

                            if (isDialogueAppearingFirstTime)
                            {
                                toSpeak = $"{dialogueText} \n\t {response}";
                                isDialogueAppearingFirstTime = false;
                            }
                            else
                                toSpeak = response;

                            MainClass.ScreenReader.Say(toSpeak, true);
                        }
                    }
                    else
                    {
                        if (currentDialogue != dialogueText)
                        {
                            currentDialogue = dialogueText;
                            MainClass.ScreenReader.Say(dialogueText, true);
                        }
                    }
                }
                else if (__instance.isQuestion)
                {
                    // For Dialogues with responses/answers like the dialogue when we click on tv
                    string toSpeak = "";
                    string dialogueText = "";
                    string response = "";
                    bool hasResponses = false;

                    if (__instance.responses.Count > 0)
                        hasResponses = true;

                    dialogueText = __instance.getCurrentString();

                    if (hasResponses)
                        if (__instance.selectedResponse >= 0 && __instance.selectedResponse < __instance.responses.Count)
                            response = $"{__instance.selectedResponse + 1}: {__instance.responses[__instance.selectedResponse].responseText}";
                        else
                            // When the dialogue is not finished writing then the selectedResponse is <0 and this results
                            // in the first response not being detcted, so this sets the first response option to be the default
                            // if the current dialogue is a question or has responses
                            response = $"1: {__instance.responses[0].responseText}";


                    if (hasResponses)
                    {
                        if (currentDialogue != response)
                        {
                            currentDialogue = response;

                            if (isDialogueAppearingFirstTime)
                            {
                                toSpeak = $"{dialogueText} \n\t {response}";
                                isDialogueAppearingFirstTime = false;
                            }
                            else
                                toSpeak = response;

                            MainClass.ScreenReader.Say(toSpeak, true);
                        }
                    }
                    else
                    {
                        if (currentDialogue != dialogueText)
                        {
                            currentDialogue = dialogueText;
                            MainClass.ScreenReader.Say(dialogueText, true);
                        }
                    }
                }
                else if (Game1.activeClickableMenu is DialogueBox)
                {
                    // Basic dialogues like `No mails in the mail box`
                    if (currentDialogue != __instance.getCurrentString())
                    {
                        currentDialogue = __instance.getCurrentString();
                        MainClass.ScreenReader.Say(__instance.getCurrentString(), true);
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate dialog:\n{e.StackTrace}\n{e.Message}");
            }

        }

        internal static void ClearDialogueString()
        {
            // CLears the currentDialogue string on closing dialog
            currentDialogue = " ";
            isDialogueAppearingFirstTime = true;
        }

        internal static void HoverTextPatch(string? text, int moneyAmountToDisplayAtBottom = -1, string? boldTitleText = null, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1, string[]? buffIconsToDisplay = null, Item? hoveredItem = null, CraftingRecipe? craftingIngredients = null)
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
                    if (Context.IsPlayerFree)
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

        internal static void LetterViewerMenuPatch(LetterViewerMenu __instance)
        {
            try
            {
                if (!__instance.IsActive())
                    return;

                NarrateLetterContent(__instance);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void NarrateLetterContent(LetterViewerMenu __instance)
        {
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

                MainClass.ScreenReader.Say(toSpeak, true);
            }
            #endregion

            #region Narrate items given in the mail
            if (__instance.ShouldShowInteractable())
            {
                foreach (ClickableComponent c in __instance.itemsToGrab)
                {
                    if (c.item == null)
                        continue;

                    string name = c.item.DisplayName;

                    if (c.containsPoint(x, y))
                        MainClass.ScreenReader.SayWithChecker($"Left click to collect {name}", false);
                }
            }
            #endregion

            #region Narrate buttons
            if (__instance.backButton != null && __instance.backButton.visible && __instance.backButton.containsPoint(x, y))
                MainClass.ScreenReader.SayWithChecker($"Previous page button", false);

            if (__instance.forwardButton != null && __instance.forwardButton.visible && __instance.forwardButton.containsPoint(x, y))
                MainClass.ScreenReader.SayWithChecker($"Next page button", false);

            #endregion
        }

        internal static void drawAboveAlwaysFrontLayerPatch(NPC __instance, string ___textAboveHead, int ___textAboveHeadTimer)
        {
            try
            {
                if (___textAboveHeadTimer > 2900 && ___textAboveHead != null)
                {
                    MainClass.ScreenReader.SayWithChecker($"{__instance.displayName} says {___textAboveHead}", true);
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Error in patch:NPCShowTextAboveHeadPatch \n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
