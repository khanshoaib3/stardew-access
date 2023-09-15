using HarmonyLib;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class FarmHandMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(FarmhandMenu), "update"),
                prefix: new HarmonyMethod(typeof(FarmHandMenuPatch), nameof(UpdatePatch))
            );
        }

        private static void UpdatePatch(FarmhandMenu __instance, List<LoadGameMenu.MenuSlot> ___menuSlots)
        {
            MouseUtils.SimulateMouseClicks(
                (x, y) => __instance.receiveLeftClick(x, y),
                (x, y) => __instance.receiveRightClick(x, y)
            );

            MainClass.ScreenReader.SayWithMenuChecker(getStatusText() ?? "", true);

            string? getStatusText()
            {
                if (__instance.client == null)
                    return Game1.content.LoadString("Strings\\UI:CoopMenu_NoInvites");
                if (__instance.client.timedOut)
                    return Game1.content.LoadString("Strings\\UI:CoopMenu_Failed");
                if (__instance.client.connectionMessage != null)
                    return __instance.client.connectionMessage;
                if (__instance.gettingFarmhands || __instance.approvingFarmhand)
                    return Game1.content.LoadString("Strings\\UI:CoopMenu_Connecting");
                return ___menuSlots.Count == 0 ? Game1.content.LoadString("Strings\\UI:CoopMenu_NoSlots") : null;
            }
        }
    }
}