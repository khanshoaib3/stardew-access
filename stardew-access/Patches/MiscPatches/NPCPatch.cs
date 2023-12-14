using HarmonyLib;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;

namespace stardew_access.Patches
{
    internal class NPCPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(NPC), "drawAboveAlwaysFrontLayer"),
                postfix: new HarmonyMethod(typeof(NPCPatch), nameof(DrawAboveAlwaysFrontLayerPatch))
            );
            
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(Junimo), "drawAboveAlwaysFrontLayer"),
                postfix: new HarmonyMethod(typeof(NPCPatch), nameof(DrawAboveAlwaysFrontLayerPatch))
            );
        }

        private static void DrawAboveAlwaysFrontLayerPatch(object __instance, string ___textAboveHead, int ___textAboveHeadTimer)
        {
            try
            {
                if (__instance is not NPC && __instance is not Junimo) return;
                
                if (___textAboveHeadTimer > 2900 && ___textAboveHead != null)
                {
                    string displayName = (__instance is Junimo junimo)
                        ? junimo.displayName
                        : ((NPC)__instance).displayName;
                    MainClass.ScreenReader.SayWithChecker($"{displayName} says {___textAboveHead}", true);
                }
            }
            catch (Exception e)
            {
                Log.Error($"Error in patch:NPCShowTextAboveHeadPatch \n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
