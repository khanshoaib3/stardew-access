using HarmonyLib;
using StardewValley;

namespace stardew_access.Patches
{
    internal class InstanceGamePatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                    original: AccessTools.Method(typeof(InstanceGame), nameof(InstanceGame.Exit)),
                    prefix: new HarmonyMethod(typeof(InstanceGamePatch), nameof(InstanceGamePatch.ExitPatch))
            );
        }

        private static void ExitPatch()
        {
            MainClass.ScreenReader?.CloseScreenReader();
        }
    }
}
