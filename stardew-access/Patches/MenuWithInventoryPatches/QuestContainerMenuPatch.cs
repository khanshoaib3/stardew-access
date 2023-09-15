using HarmonyLib;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

public class QuestContainerMenuPatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(QuestContainerMenu), "draw"),
            postfix: new HarmonyMethod(typeof(QuestContainerMenuPatch), nameof(DrawPatch))
        );
    }

    private static void DrawPatch(QuestContainerMenu __instance)
    {
        try
        {
            if (MainClass.Config.SnapToFirstSecondaryInventorySlotKey.JustPressed() &&
                __instance.ItemsToGrabMenu.inventory.Count > 0)
            {
                __instance.ItemsToGrabMenu.inventory[0].snapMouseCursorToCenter();
                __instance.setCurrentlySnappedComponentTo(__instance.ItemsToGrabMenu.inventory[0].myID);
            }
            else if (MainClass.Config.SnapToFirstInventorySlotKey.JustPressed() &&
                     __instance.inventory.inventory.Count > 0)
            {
                __instance.inventory.inventory[0].snapMouseCursorToCenter();
                __instance.setCurrentlySnappedComponentTo(__instance.inventory.inventory[0].myID);
            }

            if (InventoryUtils.NarrateHoveredSlot(__instance.inventory.inventory, __instance.inventory.actualInventory))
                return;

            if (InventoryUtils.NarrateHoveredSlot(__instance.ItemsToGrabMenu.inventory,
                    __instance.ItemsToGrabMenu.actualInventory))
                return;

            if (__instance.okButton is { visible: true } &&
                __instance.okButton.containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                MainClass.ScreenReader.TranslateAndSayWithMenuChecker("common-ui-ok_button", true);
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in special orders board patch:\n{e.Message}\n{e.StackTrace}");
        }
    }
}