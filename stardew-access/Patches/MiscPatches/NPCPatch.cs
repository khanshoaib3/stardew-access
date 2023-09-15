using HarmonyLib;
using StardewValley;

namespace stardew_access.Patches
{
    internal class NPCPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.drawAboveAlwaysFrontLayer)),
                postfix: new HarmonyMethod(typeof(NPCPatch), nameof(NPCPatch.DrawAboveAlwaysFrontLayerPatch))
            );
        }

        private static void DrawAboveAlwaysFrontLayerPatch(NPC __instance, string ___textAboveHead, int ___textAboveHeadTimer)
        {
            try
            {
                if (___textAboveHeadTimer > 2900 && ___textAboveHead != null)
                {
                    MainClass.ScreenReader.SayWithChecker($"{__instance.displayName} says {___textAboveHead}", true);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error in patch:NPCShowTextAboveHeadPatch \n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
