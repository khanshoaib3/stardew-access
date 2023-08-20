using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class PondQueryMenuPatch
    {
        private static string pondQueryMenuQuery = "";
        private static bool isNarratingPondInfo = false;

        internal static void DrawPatch(PondQueryMenu __instance, StardewValley.Object ____fishItem, FishPond ____pond, string ____statusText, bool ___confirmingEmpty)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                bool isPrimaryInfoKeyPressed = MainClass.Config.PrimaryInfoKey.JustPressed();
                string toSpeak = "", extra = "";

                if (___confirmingEmpty)
                {
                    if (__instance.yesButton != null && __instance.yesButton.containsPoint(x, y))
                        toSpeak = "Confirm button";
                    else if (__instance.noButton != null && __instance.noButton.containsPoint(x, y))
                        toSpeak = "Cancel button";
                }
                else
                {
                    if (isPrimaryInfoKeyPressed && !isNarratingPondInfo)
                    {
                        string pond_name_text = Game1.content.LoadString("Strings\\UI:PondQuery_Name", ____fishItem.DisplayName);
                        string population_text = Game1.content.LoadString("Strings\\UI:PondQuery_Population", string.Concat(____pond.FishCount), ____pond.maxOccupants.Value);
                        bool has_unresolved_needs = ____pond.neededItem.Value != null && ____pond.HasUnresolvedNeeds() && !____pond.hasCompletedRequest.Value;
                        string bring_text = "";

                        if (has_unresolved_needs && ____pond.neededItem.Value != null)
                            bring_text = Game1.content.LoadString("Strings\\UI:PondQuery_StatusRequest_Bring") + $": {____pond.neededItemCount} {____pond.neededItem.Value.DisplayName}";

                        extra = $"{pond_name_text} {population_text} {bring_text} Status: {____statusText}";
                        pondQueryMenuQuery = "";

                        isNarratingPondInfo = true;
                        Task.Delay(200).ContinueWith(_ => { isNarratingPondInfo = false; });
                    }

                    if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                        toSpeak = "Ok button";
                    else if (__instance.changeNettingButton != null && __instance.changeNettingButton.containsPoint(x, y))
                        toSpeak = "Change netting button";
                    else if (__instance.emptyButton != null && __instance.emptyButton.containsPoint(x, y))
                        toSpeak = "Empty pond button";
                }

                if (pondQueryMenuQuery != toSpeak)
                {
                    pondQueryMenuQuery = toSpeak;
                    MainClass.ScreenReader.Say(extra + " \n\t" + toSpeak, true);
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void Cleanup()
        {
            pondQueryMenuQuery = "";
            isNarratingPondInfo = false;
        }
    }
}
