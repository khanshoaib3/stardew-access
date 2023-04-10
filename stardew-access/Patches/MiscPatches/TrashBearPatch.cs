using StardewValley;
using StardewValley.Characters;

namespace stardew_access.Patches
{
    internal class TrashBearPatch
    {
        internal static void CheckActionPatch(TrashBear __instance, bool __result, int ___itemWantedIndex, int ___showWantBubbleTimer)
        {
            try
            {
                if (__result) return; // The true `true` value of __result indicates the bear is interactable i.e. when giving the bear the wanted item
                if (__instance.sprite.Value.CurrentAnimation != null) return;

                string itemName = Game1.objectInformation[___itemWantedIndex].Split('/')[4];
                MainClass.ScreenReader.Say($"Trash Bear wants {itemName}!", true);
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"An error occured TrashBearPatch::CheckActionPatch():\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
