using HarmonyLib;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class ChatBoxPatch : IPatch
{
    private static int currentChatMessageIndex = 0;
    private static string prevTextBoxMessage = "";
    private static bool isChatRunning = false;

    public void Apply(Harmony harmony)
    {
        harmony.Patch(
           original: AccessTools.DeclaredMethod(typeof(ChatBox), "update"),
           postfix: new HarmonyMethod(typeof(ChatBoxPatch), nameof(ChatBoxPatch.UpdatePatch))
        );
    }

    private static void UpdatePatch(ChatBox __instance, List<ChatMessage> ___messages)
    {
        try
        {
            if (__instance.chatBox.Selected)
            {
                bool isPrevButtonPressed = MainClass.Config.ChatMenuNextKey.JustPressed();
                bool isNextButtonPressed = MainClass.Config.ChatMenuPreviousKey.JustPressed();

                if (isNextButtonPressed && !isChatRunning)
                {
                    isChatRunning = true;
                    CycleThroughChatMessages(false, ___messages);
                    Task.Delay(200).ContinueWith(_ => { isChatRunning = false; });
                }
                else if (isPrevButtonPressed && !isChatRunning)
                {
                    isChatRunning = true;
                    CycleThroughChatMessages(true, ___messages);
                    Task.Delay(200).ContinueWith(_ => { isChatRunning = false; });
                }

                Log.Info($"{__instance.chatBox.Text}");
                if (prevTextBoxMessage != __instance.chatBox.Text)
                {
                    prevTextBoxMessage = __instance.chatBox.Text;
                    MainClass.ScreenReader.Say(prevTextBoxMessage, true);
                }
            }
            else
            {
                currentChatMessageIndex = 0;
                if (___messages.Count > 0)
                {
                    string toSpeak = GetMessage(___messages[^1]);
                    if (!string.IsNullOrWhiteSpace(toSpeak))
                    {
                        MainClass.ScreenReader.SayWithChatChecker(toSpeak, false);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in chat box patch:\n{e.Message}\n{e.StackTrace}");
        }
    }

    private static void CycleThroughChatMessages(bool prev, List<ChatMessage> ___messages)
    {
        if (___messages.Count <= 0) return;

        currentChatMessageIndex = prev
            ? Math.Min(currentChatMessageIndex + 1, ___messages.Count)
            : Math.Max(currentChatMessageIndex - 1, 1);

        MainClass.ScreenReader.Say(GetMessage(___messages[^currentChatMessageIndex]), true);
    }

    private static string GetMessage(ChatMessage chatMessage)
    {
        if (chatMessage.message.Count <= 1)
            return chatMessage.message[0].message;

        string toReturn = "";
        chatMessage.message.ForEach(chatSnippet => toReturn += $"{chatSnippet.message}, ");
        return toReturn;
    }
}
