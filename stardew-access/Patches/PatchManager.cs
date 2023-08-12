using HarmonyLib;

namespace stardew_access.Patches
{
    internal class PatchManager
    {
        public static void PatchAll(Harmony harmony)
        {
            List<IPatch> allPatches = new List<IPatch>()
            {
                // Bundle Menu Patches
                new JojaCDMenuPatch(),
                new JunimoNoteMenuPatch(),
                // Donation Menu Patches
                new FieldOfficeMenuPatch(),
                new MuseumMenuPatch(),
                // Game Menu Patches
                new CollectionsPagePatch(),
                new CraftingPagePatch(),
                new ExitPagePatch(),
                new GameMenuPatch(),
                new BillboardPatch(),
            };

            foreach (IPatch patch in allPatches)
            {
                try
                {
                    patch.Apply(harmony);
                }
                catch (Exception e)
                {
                    MainClass.ErrorLog($"An error occurred while applying {patch.GetType().FullName} patch:\n{e.Message}\n{e.StackTrace}");
                }
            }
        }
    }
}
