using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class PondQueryMenuPatch : IPatch
    {
        private static bool isNarratingPondInfo = false;

        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(PondQueryMenu), nameof(PondQueryMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(PondQueryMenuPatch), nameof(PondQueryMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(PondQueryMenu __instance, StardewValley.Object ____fishItem, FishPond ____pond, string ____statusText, bool ___confirmingEmpty)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                bool isPrimaryInfoKeyPressed = MainClass.Config.PrimaryInfoKey.JustPressed();
                string toSpeak = "", extra = "";

                if (___confirmingEmpty)
                {
                    if (__instance.yesButton != null && __instance.yesButton.containsPoint(x, y))
                        toSpeak = Translator.Instance.Translate("common-ui-confirm_button");
                    else if (__instance.noButton != null && __instance.noButton.containsPoint(x, y))
                        toSpeak = Translator.Instance.Translate("common-ui-cancel_button");
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

                        object translationTokens = new
                        {
                            pond_name = pond_name_text,
                            population_info = population_text,
                            required_item_info = bring_text,
                            status = ____statusText
                        };
                        extra = Translator.Instance.Translate("menu-pond_query-pond_info", translationTokens);
                        MainClass.ScreenReader.PrevMenuQueryText = "";

                        isNarratingPondInfo = true;
                        Task.Delay(200).ContinueWith(_ => { isNarratingPondInfo = false; });
                    }

                    if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                        toSpeak = Translator.Instance.Translate("common-ui-ok_button");
                    else if (__instance.changeNettingButton != null && __instance.changeNettingButton.containsPoint(x, y))
                        toSpeak = Translator.Instance.Translate("menu-pond_query-change_netting_button");
                    else if (__instance.emptyButton != null && __instance.emptyButton.containsPoint(x, y))
                        toSpeak = Translator.Instance.Translate("menu-pond_query-empty_pond_button");
                }

                MainClass.ScreenReader.SayWithMenuChecker(string.Join("\n", [extra, toSpeak]), true);
            }
            catch (System.Exception e)
            {
                Log.Error($"An error occurred in pond query menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void Cleanup()
        {
            isNarratingPondInfo = false;
        }
    }
}
