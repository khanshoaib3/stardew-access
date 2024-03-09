using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class DialogueBoxPatch : IPatch
    {
        private static bool isDialogueAppearingFirstTime = true;

        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.draw),
                    new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(DialogueBoxPatch), nameof(DialogueBoxPatch.DrawPatch))
            );
        }

        private static void DrawPatch(DialogueBox __instance)
        {
            try
            {
                if (__instance.transitioning) return;

                if (NarrateCharacterDialogue(__instance)) return;
                if (NarrateQuestionDialogue(__instance)) return;
                NarrateBasicDialogue(__instance.getCurrentString());
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in dialogue box patch:\n{e.StackTrace}\n{e.Message}");
            }
        }

        private static bool NarrateCharacterDialogue(DialogueBox __instance)
        {
            if (__instance.characterDialogue == null) return false;

            // For Normal Character dialogues
            Dialogue dialogue = __instance.characterDialogue;
            string dialogueText = "";
            string responseText = "";
            bool hasResponses = dialogue.isCurrentDialogueAQuestion();

            dialogueText = Translator.Instance.Translate(
                "menu-dialogue_box-npc_dialogue_format",
                new
                {
                    is_appearing_first_time = isDialogueAppearingFirstTime ? 1 : 0,
                    npc_name = dialogue.speaker.displayName,
                    dialogue = __instance.getCurrentString()
                }, TranslationCategory.Menu);

            if (hasResponses)
            {
                responseText = GetCurrentResponseText(__instance);

                CheckAndSpeak(isDialogueAppearingFirstTime ? $"{dialogueText}\n{responseText}" : responseText,
                    responseText);
                if (isDialogueAppearingFirstTime) isDialogueAppearingFirstTime = false;
            }
            else
            {
                CheckAndSpeak(dialogueText);
            }

            return true;
        }

        private static bool NarrateQuestionDialogue(DialogueBox __instance)
        {
            if (!__instance.isQuestion) return false;

            // For Dialogues with responses/answers like the dialogue when we click on tv
            string questionText = "";
            string responseText = "";
            bool hasResponses = false;

            if (__instance.responses.Count() > 0) hasResponses = true;
            if (!hasResponses) return false;

            questionText = __instance.getCurrentString();

            responseText = GetCurrentResponseText(__instance);

            CheckAndSpeak(isDialogueAppearingFirstTime ? $"{questionText}\n{responseText}" : responseText,
                responseText);
            if (isDialogueAppearingFirstTime) isDialogueAppearingFirstTime = false;

            return true;
        }

        private static void NarrateBasicDialogue(string dialogue)
        {
            // Basic dialogues like `No mails in the mail box`
            if (Game1.activeClickableMenu is not DialogueBox) return;
            CheckAndSpeak(dialogue);
        }

        private static string GetCurrentResponseText(DialogueBox __instance)
        {
            List<Response> responses = __instance.responses.ToList();
            if (__instance.selectedResponse >= 0 && __instance.selectedResponse < responses.Count)
            {
                return $"{__instance.selectedResponse + 1}: {responses[__instance.selectedResponse].responseText}";
            }
            else
            {
                // When the dialogue is not finished writing then the selectedResponse is <0 and this results
                // in the first response not being detcted, so this sets the first response option to be the default
                // if the current dialogue is a question or has responses
                return $"1: {responses[0].responseText}";
            }
        }

        private static void CheckAndSpeak(string toSpeak)
        {
            MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
        }

        private static void CheckAndSpeak(string toSpeak, string checkQuery)
        {
            MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true, checkQuery);
        }

        internal static void Cleanup()
        {
            isDialogueAppearingFirstTime = true;
        }
    }
}