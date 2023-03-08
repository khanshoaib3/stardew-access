using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class DialoguePatches
    {
        private static string currentDialogue = "";
        private static string previousSpeakerName = "";
        private static bool isDialogueAppearingFirstTime = true;

        internal static void DialoguePatch(DialogueBox __instance, SpriteBatch b)
        {
            try
            {
                if (__instance.transitioning) return;

                if (narrateCharacterDialogue(__instance)) return;
                if (narrateQuestionDialogue(__instance)) return;
                narrateBasicDialogue(__instance.getCurrentString());
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate dialog:\n{e.StackTrace}\n{e.Message}");
            }
        }

        internal static void RecieveLeftClickPatch()
        {
            // CLears the currentDialogue string on closing dialog
            Cleanup();
        }

        private static bool narrateCharacterDialogue(DialogueBox __instance)
        {
            if (__instance.characterDialogue == null) return false;

            // For Normal Character dialogues
            Dialogue dialogue = __instance.characterDialogue;
            string speakerName = dialogue.speaker.displayName;
            string dialogueText = "";
            string responseText = "";
            bool hasResponses = dialogue.isCurrentDialogueAQuestion();

            dialogueText = $"{speakerName} said {__instance.getCurrentString()}";

            if (hasResponses)
            {
                responseText = getCurrentResponseText(__instance);

                CheckAndSpeak(isDialogueAppearingFirstTime ? $"{dialogueText} \n\t {responseText}" : responseText, responseText);
                if (isDialogueAppearingFirstTime) isDialogueAppearingFirstTime = false;
            }
            else
            {
                CheckAndSpeak(dialogueText);
            }

            return true;
        }

        private static bool narrateQuestionDialogue(DialogueBox __instance)
        {
            if (!__instance.isQuestion) return false;

            // For Dialogues with responses/answers like the dialogue when we click on tv
            string questionText = "";
            string responseText = "";
            bool hasResponses = false;

            if (__instance.responses.Count > 0) hasResponses = true;
            if (!hasResponses) return false;

            questionText = __instance.getCurrentString();

            responseText = getCurrentResponseText(__instance);

            CheckAndSpeak(isDialogueAppearingFirstTime ? $"{questionText} \n\t {responseText}" : responseText, responseText);
            if (isDialogueAppearingFirstTime) isDialogueAppearingFirstTime = false;

            return true;
        }

        private static void narrateBasicDialogue(string dialogue)
        {
            // Basic dialogues like `No mails in the mail box`
            if (Game1.activeClickableMenu is not DialogueBox) return;
            CheckAndSpeak(dialogue);
        }

        private static string getCurrentResponseText(DialogueBox __instance)
        {
            List<Response> responses = __instance.responses;
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
            if (currentDialogue == toSpeak) return;
            currentDialogue = toSpeak;

            MainClass.ScreenReader.Say(toSpeak, true);
        }

        private static void CheckAndSpeak(string toSpeak, string checkQuery)
        {
            if (currentDialogue == checkQuery) return;
            currentDialogue = checkQuery;

            MainClass.ScreenReader.Say(toSpeak, true);
        }

        internal static void Cleanup()
        {
            currentDialogue = "";
            isDialogueAppearingFirstTime = true;
        }
    }
}
