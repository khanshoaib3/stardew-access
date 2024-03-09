using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class LetterViewerMenuPatch : IPatch
    {
        private static string letterViewerQueryText = "";

        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                    original: AccessTools.Method(typeof(LetterViewerMenu), nameof(LetterViewerMenu.draw), new Type[] { typeof(SpriteBatch) }),
                    postfix: new HarmonyMethod(typeof(LetterViewerMenuPatch), nameof(LetterViewerMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(LetterViewerMenu __instance)
        {
            try
            {
                if (!__instance.IsActive())
                    return;

                NarrateMenu(__instance);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in letter viewer menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void NarrateMenu(LetterViewerMenu __instance)
        {
            int x = Game1.getMousePosition().X, y = Game1.getMousePosition().Y;

            NarrateLetterContent(__instance, x, y);
            NarrateHoveredButtons(__instance, x, y);
        }

        private static void NarrateLetterContent(LetterViewerMenu __instance, int x, int y)
        {
            string translationKey = "menu-letter_viewer-letter_message";
            object? translationTokens = new
            {
                message_content = __instance.mailMessage[__instance.page],
                is_money_included = (__instance.ShouldShowInteractable() && __instance.moneyIncluded > 0) ? 1 : 0,
                received_money = __instance.moneyIncluded,
                learned_any_recipe = (__instance.ShouldShowInteractable() && __instance.learnedRecipe != null && __instance.learnedRecipe.Length > 0) ? 1 : 0,
                learned_recipe = Game1.content.LoadString("Strings\\UI:LetterViewer_LearnedRecipe", __instance.cookingOrCrafting),
                is_quest = (__instance.acceptQuestButton != null && __instance.questID != "-1") ? 1 : 0,
            };

            string toSpeak = Translator.Instance.Translate(translationKey, translationTokens, TranslationCategory.Menu);

            if (__instance.mailMessage.Count > 1)
            {
                toSpeak = Translator.Instance.Translate("menu-letter_viewer-pagination_text-prefix", new
                    {
                        current_page = __instance.page + 1,
                        total_pages = __instance.mailMessage.Count,
                        content = toSpeak
                    },
                    TranslationCategory.Menu);
            }

            if (!MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true)) return;
            // snap mouse to accept quest button
            if (__instance.acceptQuestButton != null && __instance.questID != "-1")
                __instance.acceptQuestButton.snapMouseCursorToCenter();
        }

        private static void NarrateHoveredButtons(LetterViewerMenu __instance, int x, int y)
        {
            if (__instance.backButton != null && __instance.backButton.visible && __instance.backButton.containsPoint(x, y))
                CheckAndSpeak(Translator.Instance.Translate("common-ui-previous_page_button", TranslationCategory.Menu));
            else if (__instance.forwardButton != null && __instance.forwardButton.visible && __instance.forwardButton.containsPoint(x, y))
                CheckAndSpeak(Translator.Instance.Translate("common-ui-next_page_button", TranslationCategory.Menu));
            else if (__instance.ShouldShowInteractable())
            {
                foreach (ClickableComponent c in __instance.itemsToGrab)
                {
                    if (c.item == null || !c.containsPoint(x, y))
                        continue;

                    object? token = new { name = InventoryUtils.GetPluralNameOfItem(c.item) };
                    CheckAndSpeak(Translator.Instance.Translate("menu-letter_viewer-grabbable_item_text", token, TranslationCategory.Menu));
                }
            }
        }

        private static void CheckAndSpeak(string toSpeak)
        {
            if (toSpeak == letterViewerQueryText) return;

            letterViewerQueryText = toSpeak;
            MainClass.ScreenReader.Say(toSpeak, false);
        }

        internal static void Cleanup()
        {
            letterViewerQueryText = "";
        }
    }
}
