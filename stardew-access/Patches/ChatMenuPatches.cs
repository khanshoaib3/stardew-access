using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class ChatMenuPatches
    {
        private static int currentChatMessageIndex = 0;
        private static bool isChatRunning = false;

        internal static void ChatBoxPatch(ChatBox __instance, List<ChatMessage> ___messages)
        {
            try
            {
                string toSpeak = " ";

                if (__instance.chatBox.Selected)
                {
                    bool isPrevButtonPressed = MainClass.Config.ChatMenuNextKey.JustPressed();
                    bool isNextButtonPressed = MainClass.Config.ChatMenuPreviousKey.JustPressed();

                    if (___messages.Count > 0)
                    {
                        #region To narrate previous and next chat messages
                        if (isNextButtonPressed && !isChatRunning)
                        {
                            isChatRunning = true;
                            CycleThroughChatMessages(true, ___messages);
                            Task.Delay(200).ContinueWith(_ => { isChatRunning = false; });
                        }
                        else if (isPrevButtonPressed && !isChatRunning)
                        {
                            isChatRunning = true;
                            CycleThroughChatMessages(false, ___messages);
                            Task.Delay(200).ContinueWith(_ => { isChatRunning = false; });
                        }
                        #endregion
                    }
                }
                else if (___messages.Count > 0)
                {
                    #region To narrate latest chat message
                    ___messages[___messages.Count - 1].message.ForEach(message =>
                    {
                        toSpeak += $"{message.message}, ";
                    });
                    if (toSpeak != " ")
                        MainClass.ScreenReader.SayWithChatChecker(toSpeak, false);
                    #endregion
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void CycleThroughChatMessages(bool increase, List<ChatMessage> ___messages)
        {
            string toSpeak = " ";

            currentChatMessageIndex = (increase) ? (Math.Min(currentChatMessageIndex + 1, ___messages.Count - 1)) : (currentChatMessageIndex = Math.Max(currentChatMessageIndex - 1, 0));

            ___messages[currentChatMessageIndex].message.ForEach(message =>
            {
                toSpeak += $"{message.message}, ";
            });

            MainClass.ScreenReader.Say(toSpeak, true);
        }
    }
}
