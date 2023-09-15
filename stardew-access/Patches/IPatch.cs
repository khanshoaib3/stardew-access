using HarmonyLib;

namespace stardew_access.Patches
{
    internal interface IPatch
    {
        public void Apply(Harmony harmony);
    }
}
