using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class LetterViwerMenuPatch
    {
        private static string currentLetterText = "";

        internal static void DrawPatch(LetterViewerMenu __instance)
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

        internal static void Cleanup()
        {
            currentLetterText = "";
        }
    }
}
