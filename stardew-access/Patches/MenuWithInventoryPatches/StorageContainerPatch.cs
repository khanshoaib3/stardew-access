using HarmonyLib;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class StorageContainerPatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(StorageContainer), "draw"),
            postfix: new HarmonyMethod(typeof(StorageContainerPatch), nameof(StorageContainerPatch.DrawPatch))
        );
    }

    private static void DrawPatch(StorageContainer __instance)
    {
        try
        {
            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

            if (MainClass.Config.SnapToFirstSecondaryInventorySlotKey.JustPressed() &&
                __instance.ItemsToGrabMenu.inventory.Count > 0)
            {
                __instance.setCurrentlySnappedComponentTo(__instance.ItemsToGrabMenu.inventory[0].myID);
                __instance.ItemsToGrabMenu.inventory[0].snapMouseCursorToCenter();
            }
            else if (MainClass.Config.SnapToFirstInventorySlotKey.JustPressed() &&
                     __instance.inventory.inventory.Count > 0)
            {
                __instance.setCurrentlySnappedComponentTo(__instance.inventory.inventory[0].myID);
                __instance.inventory.inventory[0].snapMouseCursorToCenter();
            }

            // Player inventory
            if (InventoryUtils.NarrateHoveredSlot(__instance.inventory, giveExtraDetails: false))
            {
                return;
            }

            // Other inventory
            InventoryUtils.NarrateHoveredSlot(__instance.ItemsToGrabMenu, giveExtraDetails: false);
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in sotrage container menu patch:\n{e.Message}\n{e.StackTrace}");
        }
    }
}
