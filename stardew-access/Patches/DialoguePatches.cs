using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System.Text;

namespace stardew_access.Patches
{
    internal class DialoguePatches
    {
        private static string currentDialogue = " ";

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
                    string toSpeak = " ";
                    bool hasResponses = false;

                    if (__instance.responses.Count > 0)
                        hasResponses = true;

                    if (currentDialogue != __instance.getCurrentString())
                    {
                        toSpeak = __instance.getCurrentString();
                        currentDialogue = toSpeak;
                        toSpeak = $"{speakerName} said {toSpeak}";
                    }

                    if (__instance.responses.Count > 0)
                    {
                        for (int i = 0; i < __instance.responses.Count; i++)
                        {
                            if (i == __instance.selectedResponse)
                            {
                                toSpeak += $" \t\n Selected response: {__instance.responses[i].responseText}";
                            }
                        }
                    }

                    if (toSpeak != " ")
                    {
                        if (hasResponses)
                            MainClass.screenReader.SayWithChecker(toSpeak, false);
                        else
                            MainClass.screenReader.SayWithChecker(toSpeak, true);
                    }
                }
                else if (__instance.isQuestion)
                {
                    // For Dialogues with responses/answers like the dialogue when we click on tv
                    string toSpeak = " ";
                    bool hasResponses = false;

                    if (__instance.responses.Count > 0)
                        hasResponses = true;

                    if (currentDialogue != __instance.getCurrentString())
                    {
                        toSpeak = __instance.getCurrentString();
                        currentDialogue = toSpeak;
                    }

                    for (int i = 0; i < __instance.responses.Count; i++)
                    {
                        if (i == __instance.selectedResponse)
                        {
                            toSpeak += $" \t\n Selected response: {__instance.responses[i].responseText}";
                        }
                    }

                    if (toSpeak != " ")
                    {
                        if (hasResponses)
                            MainClass.screenReader.SayWithChecker(toSpeak, false);
                        else
                            MainClass.screenReader.SayWithChecker(toSpeak, true);
                    }
                }
                else if (Game1.activeClickableMenu is DialogueBox)
                {
                    // Basic dialogues like `No mails in the mail box`
                    if (currentDialogue != __instance.getCurrentString())
                    {
                        currentDialogue = __instance.getCurrentString();
                        MainClass.screenReader.Say(__instance.getCurrentString(), true);
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate dialog:\n{e.StackTrace}\n{e.Message}", LogLevel.Error);
            }

        }

        internal static void ClearDialogueString()
        {
            // CLears the currentDialogue string on closing dialog
            currentDialogue = " ";
        }

        internal static void HoverTextPatch(string? text, int moneyAmountToDisplayAtBottom = -1, string? boldTitleText = null, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1, string[]? buffIconsToDisplay = null, Item? hoveredItem = null, CraftingRecipe? craftingIngredients = null)
        {
            try
            {
                #region Skip narrating hover text for certain menus
                if (Game1.activeClickableMenu is TitleMenu && !(((TitleMenu)Game1.activeClickableMenu).GetChildMenu() is CharacterCustomization))
                    return;

                if (Game1.activeClickableMenu is LetterViewerMenu || Game1.activeClickableMenu is QuestLog)
                    return;

                if (Game1.activeClickableMenu is Billboard)
                    return;

                if (Game1.activeClickableMenu is GeodeMenu)
                    return;

                if (Game1.activeClickableMenu is GameMenu && ((GameMenu)Game1.activeClickableMenu).GetCurrentPage() is InventoryPage)
                    return;

                if (Game1.activeClickableMenu is GameMenu && ((GameMenu)Game1.activeClickableMenu).GetCurrentPage() is CraftingPage)
                    return;

                if (Game1.activeClickableMenu is GameMenu && ((GameMenu)Game1.activeClickableMenu).GetCurrentPage() is OptionsPage)
                    return;

                if (Game1.activeClickableMenu is GameMenu && ((GameMenu)Game1.activeClickableMenu).GetCurrentPage() is ExitPage)
                    return;

                if (Game1.activeClickableMenu is GameMenu && ((GameMenu)Game1.activeClickableMenu).GetCurrentPage() is SocialPage)
                    return;

                if (Game1.activeClickableMenu is ItemGrabMenu)
                    return;

                if (Game1.activeClickableMenu is ShopMenu)
                    return;

                if (Game1.activeClickableMenu is ConfirmationDialog)
                    return;

                if (Game1.activeClickableMenu is JunimoNoteMenu)
                    return;

                if (Game1.activeClickableMenu is CarpenterMenu)
                    return;

                if (Game1.activeClickableMenu is PurchaseAnimalsMenu)
                    return;
                #endregion

                StringBuilder toSpeak = new StringBuilder(" ");

                #region Add item count before title
                if (hoveredItem != null && hoveredItem.HasBeenInInventory)
                {
                    int count = hoveredItem.Stack;
                    if (count > 1)
                        toSpeak.Append($"{count} ");
                }
                #endregion

                #region Add title if any
                if (boldTitleText != null)
                    toSpeak.Append($"{boldTitleText}\n");
                #endregion

                #region Add quality of item
                if (hoveredItem is StardewValley.Object && ((StardewValley.Object)hoveredItem).quality > 0)
                {
                    int quality = ((StardewValley.Object)hoveredItem).quality;
                    if (quality == 1)
                    {
                        toSpeak.Append("Silver quality");
                    }
                    else if (quality == 2 || quality == 3)
                    {
                        toSpeak.Append("Gold quality");
                    }
                    else if (quality >= 4)
                    {
                        toSpeak.Append("Iridium quality");
                    }
                }
                #endregion

                #region Narrate hovered required ingredients
                if (extraItemToShowIndex != -1)
                {
                    string itemName = Game1.objectInformation[extraItemToShowIndex].Split('/')[0];

                    if (extraItemToShowAmount != -1)
                        toSpeak.Append($"Required: {extraItemToShowAmount} {itemName}");
                    else
                        toSpeak.Append($"Required: {itemName}");
                }
                #endregion

                #region Add money
                if (moneyAmountToDisplayAtBottom != -1)
                    toSpeak.Append($"\nCost: {moneyAmountToDisplayAtBottom}g\n");
                #endregion

                #region Add the base text
                toSpeak.Append(text);
                #endregion

                #region Add crafting ingredients
                if (craftingIngredients != null)
                {
                    toSpeak.Append($"\n{craftingIngredients.description}");
                    toSpeak.Append("\nIngredients\n");

                    craftingIngredients.recipeList.ToList().ForEach(recipe =>
                    {
                        int count = recipe.Value;
                        int item = recipe.Key;
                        string name = craftingIngredients.getNameFromIndex(item);

                        toSpeak.Append($" ,{count} {name}");
                    });
                }
                #endregion

                #region Add health & stamina
                if (hoveredItem is StardewValley.Object && ((StardewValley.Object)hoveredItem).Edibility != -300)
                {
                    int stamina_recovery = ((StardewValley.Object)hoveredItem).staminaRecoveredOnConsumption();
                    toSpeak.Append($"{stamina_recovery} Energy\n");
                    if (stamina_recovery >= 0)
                    {
                        int health_recovery = ((StardewValley.Object)hoveredItem).healthRecoveredOnConsumption();
                        toSpeak.Append($"{health_recovery} Health");
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
                                toSpeak.Append($"{buffName}\n");
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
                        MainClass.screenReader.SayWithChecker(toSpeak.ToString(), true); // Normal Checker
                    else
                        MainClass.screenReader.SayWithMenuChecker(toSpeak.ToString(), true); // Menu Checker
                }
                #endregion
            }
            catch (Exception e)
            {
                MainClass.monitor.Log($"Unable to narrate dialog:\n{e.StackTrace}\n{e.Message}", LogLevel.Error);
            }
        }
    }
}
