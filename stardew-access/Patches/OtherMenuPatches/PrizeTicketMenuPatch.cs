using HarmonyLib;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class PrizeTicketMenuPatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(PrizeTicketMenu), "draw"),
            postfix: new HarmonyMethod(typeof(PrizeTicketMenuPatch), nameof(PrizeTicketMenuPatch.DrawPatch))
        );
    }

    private static void DrawPatch(PrizeTicketMenu __instance, List<Item> ___currentPrizeTrack, ClickableTextureComponent ___mainButton, bool ___gettingReward, bool ___movingRewardTrack)
    {
        try
        {
            if (___movingRewardTrack || ___gettingReward) return;

            int x = Game1.getMouseX(true), y = Game1.getMouseY(true);

            if (MainClass.Config.PrimaryInfoKey.JustPressed())
            {
                MainClass.ScreenReader.PrevMenuQueryText = "";
            }

            if (___mainButton is not { visible: true } || !___mainButton.containsPoint(x, y))
                return;

            int prizeTicketCount = Game1.player.Items.CountId("PrizeTicket");
            string prizeItems = string.Join(", ", ___currentPrizeTrack.ConvertAll<string>(item => InventoryUtils.GetItemDetails(item)));

            MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-prize_ticket-collect_prize_button", true, translationTokens: new
            {
                prize_items = prizeItems,
                prize_ticket_count = prizeTicketCount
            });
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in prize ticket menu patch:\n{e.Message}\n{e.StackTrace}");
        }
    }
}
