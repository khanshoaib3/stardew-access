using StardewValley;

namespace stardew_access.Patches
{
    internal class TrashBearPatch
    {
        internal static void DrawPatch(int ___itemWantedIndex, int ___showWantBubbleTimer)
        {
            try
            {
                if (___showWantBubbleTimer >= 2900)
                {
                    string itemName = Game1.objectInformation[___itemWantedIndex].Split('/')[4];
                    MainClass.ScreenReader.Say($"Trash Bear wants {itemName}!", true);
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"An error occured TrashBearPatch::DrawPatch():\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
