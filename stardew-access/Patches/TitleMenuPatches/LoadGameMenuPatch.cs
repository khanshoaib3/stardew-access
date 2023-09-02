using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using StardewValley;
using StardewValley.Menus;
using static StardewValley.Menus.LoadGameMenu;

namespace stardew_access.Patches
{
    internal class LoadGameMenuPatch : IPatch
    {
        private static bool firstTimeInMenu = true;

        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(LoadGameMenu.SaveFileSlot), nameof(LoadGameMenu.SaveFileSlot.Draw), new Type[] { typeof(SpriteBatch), typeof(int) }),
                postfix: new HarmonyMethod(typeof(LoadGameMenuPatch), nameof(LoadGameMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(SaveFileSlot __instance, LoadGameMenu ___menu, int i)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);

                if (NarrateHoveredButtons(__instance, ___menu, x, y, i))
                {
                    return;
                }

                if (NarrateDeleteConfirmationMenu(___menu, x, y))
                {
                    return;
                }

                NarrateFarmButton(__instance, ___menu, x, y, i);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in load game menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static bool NarrateDeleteConfirmationMenu(LoadGameMenu ___menu, int x, int y)
        {
            if (!___menu.deleteConfirmationScreen)
            {
                firstTimeInMenu = true;
                return false;
            }

            string translationKey = "";
            if (firstTimeInMenu)
            {
                firstTimeInMenu = false;
                MainClass.ScreenReader.MenuPrefixNoQueryText = Translator.Instance.Translate("menu-load_game-delete_farm_confirmation_text") + " ";
            }

            if (___menu.okDeleteButton.containsPoint(x, y))
            {
                translationKey = "common-ui-ok_button";
            }
            else if (___menu.cancelDeleteButton.containsPoint(x, y))
            {
                translationKey = "common-ui-cancel_button";
            }

            MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true);
            return true;
        }

        private static void NarrateFarmButton(SaveFileSlot __instance, LoadGameMenu ___menu, int x, int y, int i)
        {
            if (!___menu.slotButtons[i].containsPoint(x, y)) return;
            if (__instance.Farmer == null) return;

            string dateStringForSaveGame = (!__instance.Farmer.dayOfMonthForSaveGame.HasValue ||
                !__instance.Farmer.seasonForSaveGame.HasValue ||
                !__instance.Farmer.yearForSaveGame.HasValue) ? __instance.Farmer.dateStringForSaveGame : Utility.getDateStringFor(__instance.Farmer.dayOfMonthForSaveGame.Value, __instance.Farmer.seasonForSaveGame.Value, __instance.Farmer.yearForSaveGame.Value);

            string translationKey = "menu-load_game-farm_details";
            object translationTokens = new
            {
                index = i + 1,
                farm_name = __instance.Farmer.farmName.Value,
                farmer_name = __instance.Farmer.isCustomized.Value
                                ? __instance.Farmer.displayName
                                : Game1.content.LoadString("Strings\\UI:CoopMenu_NewFarmhand"),
                money = __instance.Farmer.Money.ToString(),
                hours_played = Utility.getHoursMinutesStringFromMilliseconds(__instance.Farmer.millisecondsPlayed),
                date = dateStringForSaveGame
            };

            MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens);
        }

        private static bool NarrateHoveredButtons(SaveFileSlot __instance, LoadGameMenu ___menu, int x, int y, int i)
        {
            string translationKey;
            object? translationTokens = null;

            if (___menu.deleteButtons.Count > 0 && ___menu.deleteButtons[i].containsPoint(x, y))
            {
                translationKey = "menu-load_game-delete_farm_button";
                translationTokens = new
                {
                    name = __instance.Farmer.farmName.Value
                };
            }
            else if (___menu.upArrow != null && ___menu.upArrow.containsPoint(x, y))
            {
                translationKey = "common-ui-scroll_up_button";
            }
            else if (___menu.downArrow != null && ___menu.downArrow.containsPoint(x, y))
            {
                translationKey = "common-ui-scroll_down_button";
            }
            else
            {
                return false;
            }

            MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens);
            return true;
        }
    }
}
