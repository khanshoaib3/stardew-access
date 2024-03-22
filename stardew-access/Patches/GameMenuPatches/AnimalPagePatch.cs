using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class AnimalPagePatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(AnimalPage), "draw"),
            postfix: new HarmonyMethod(typeof(AnimalPagePatch), nameof(AnimalPagePatch.DrawPatch))
        );
    }

    private static void DrawPatch(AnimalPage __instance)
    {
        try
        {
            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

            for (int i = 0; i < __instance.characterSlots.Count; i++)
            {
                if (!__instance.characterSlots[i].containsPoint(x, y)) continue;

                var animalEntry = __instance.GetSocialEntry(i);
                int heartCount = animalEntry.FriendshipLevel != -1 ? (int)Math.Floor((double)animalEntry.FriendshipLevel / 195) : -1;

                MainClass.ScreenReader.TranslateAndSayWithMenuChecker($"menu-animal_page-animal_info", true, translationTokens: new
                {
                    name = animalEntry.DisplayName,
                    type = animalEntry.AnimalType,
                    heart_count = heartCount,
                    has_been_pet = animalEntry.WasPetYet != 0 ? 1 : 0,
                    has_received_animal_cracker = animalEntry.ReceivedAnimalCracker ? 1 : 0,
                });
                return;
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in animal page patch:\n{e.Message}\n{e.StackTrace}");
        }
    }
}
