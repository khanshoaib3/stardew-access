using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class ChatBoxPatch : IPatch
{
    private static int currentChatMessageIndex = 0;
    private static bool isChatRunning = false;
    private static bool isChatBoxActive = false;

    public void Apply(Harmony harmony)
    {
        harmony.Patch(
           original: AccessTools.DeclaredMethod(typeof(ChatBox), "update"),
           postfix: new HarmonyMethod(typeof(ChatBoxPatch), nameof(ChatBoxPatch.UpdatePatch))
        );

        harmony.Patch(
           original: AccessTools.DeclaredMethod(typeof(KeyboardDispatcher), "Event_TextInput"),
           prefix: new HarmonyMethod(typeof(ChatBoxPatch), nameof(ChatBoxPatch.KeyboardDispatcher_RecieveTextInputPatch))
        );
    }

    private static bool KeyboardDispatcher_RecieveTextInputPatch()
    {
        if (!isChatBoxActive) return true;
        
        bool isLeftAltPressed = Game1.input.GetKeyboardState().IsKeyDown(Keys.LeftAlt);
        if (!isLeftAltPressed) return true;

        return false;
    }

    private static void UpdatePatch(ChatBox __instance, List<ChatMessage> ___messages)
    {
        try
        {
            if (__instance.chatBox.Selected)
            {
                isChatBoxActive = true;
                bool isLeftAltPressed = Game1.input.GetKeyboardState().IsKeyDown(Keys.LeftAlt);

                bool isPrevButtonPressed = MainClass.Config.ChatMenuNextKey.JustPressed();
                bool isNextButtonPressed = MainClass.Config.ChatMenuPreviousKey.JustPressed();

                if (isLeftAltPressed && !isChatRunning)
                {
                    int pressedNumKey = -1;
                    foreach (var key in Game1.input.GetKeyboardState().GetPressedKeys())
                    {
                        pressedNumKey = key switch
                        {
                            Keys.NumPad1 or Keys.D1 => 1,
                            Keys.NumPad2 or Keys.D2 => 2,
                            Keys.NumPad3 or Keys.D3 => 3,
                            Keys.NumPad4 or Keys.D4 => 4,
                            Keys.NumPad5 or Keys.D5 => 5,
                            Keys.NumPad6 or Keys.D6 => 6,
                            Keys.NumPad7 or Keys.D7 => 7,
                            Keys.NumPad8 or Keys.D8 => 8,
                            Keys.NumPad9 or Keys.D9 => 9,
                            _ => -1,
                        };

                        if (pressedNumKey != -1) break;
                    }

                    if (pressedNumKey != -1 && ___messages.Count >= pressedNumKey)
                    {
                        isChatRunning = true;
                        MainClass.ScreenReader.Say(GetMessage(___messages[^pressedNumKey]), true);
                        Task.Delay(200).ContinueWith(_ => { isChatRunning = false; });
                    }
                    return;
                }

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
            }
            else
            {
                isChatBoxActive = false;
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
