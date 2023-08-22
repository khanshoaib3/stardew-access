using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class LevelUpMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(LevelUpMenu), nameof(LevelUpMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(LevelUpMenuPatch), nameof(LevelUpMenuPatch.DrawPatch))
            );
        }

        private static void DrawPatch(LevelUpMenu __instance, List<int> ___professionsToChoose,
                                       List<string> ___leftProfessionDescription,
                                       List<string> ___rightProfessionDescription, List<string> ___extraInfoForLevel,
                                       List<CraftingRecipe> ___newCraftingRecipes, string ___title, ref bool ___isActive,
                                       ref bool ___isProfessionChooser)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);

                if (!__instance.informationUp)
                    return;

                NarrateProfessionChooser(__instance, ___professionsToChoose, ___leftProfessionDescription,
                                         ___rightProfessionDescription, ___title, ref ___isActive,
                                         ref ___isProfessionChooser, x, y);
                NarrateLevelUpInfo(__instance, ___extraInfoForLevel, ___newCraftingRecipes, ___title, x, y);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in level up menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void NarrateProfessionChooser(LevelUpMenu __instance, List<int> ___professionsToChoose,
                                                     List<string> ___leftProfessionDescription,
                                                     List<string> ___rightProfessionDescription, string ___title,
                                                     ref bool ___isActive, ref bool ___isProfessionChooser, int x, int y)
        {
            if (!__instance.isProfessionChooser) return;
            if (___professionsToChoose.Count == 0) return;

            string translationKey = "";
            object? translationTokens = null;

            if (__instance.leftProfession.containsPoint(x, y))
            {
                if ((MainClass.Config.LeftClickMainKey.JustPressed() || MainClass.Config.LeftClickAlternateKey.JustPressed()) && __instance.readyToClose())
                {
                    Game1.player.professions.Add(___professionsToChoose[0]);
                    __instance.getImmediateProfessionPerk(___professionsToChoose[0]);
                    ___isActive = false;
                    __instance.informationUp = false;
                    ___isProfessionChooser = false;
                    __instance.RemoveLevelFromLevelList();
                    __instance.exitThisMenu();
                    return;
                }

                translationKey = "menu-level_up-profession_chooser_button";
                translationTokens = new
                {
                    profession_description_list = string.Join(", ", ___leftProfessionDescription)
                };
            }
            else if (__instance.rightProfession.containsPoint(x, y))
            {
                if ((MainClass.Config.LeftClickMainKey.JustPressed() || MainClass.Config.LeftClickAlternateKey.JustPressed()) && __instance.readyToClose())
                {
                    Game1.player.professions.Add(___professionsToChoose[1]);
                    __instance.getImmediateProfessionPerk(___professionsToChoose[1]);
                    ___isActive = false;
                    __instance.informationUp = false;
                    ___isProfessionChooser = false;
                    __instance.RemoveLevelFromLevelList();
                    __instance.exitThisMenu();
                    return;
                }

                translationKey = "menu-level_up-profession_chooser_button";
                translationTokens = new
                {
                    profession_description_list = string.Join(", ", ___rightProfessionDescription)
                };
            }
            else
            {
                translationKey = "menu-level_up-profession_chooser_heading";
                translationTokens = new
                {
                    title = ___title
                };
            }

            MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens);
        }

        private static void NarrateLevelUpInfo(LevelUpMenu __instance, List<string> ___extraInfoForLevel,
                                               List<CraftingRecipe> ___newCraftingRecipes, string ___title, int x, int y)
        {
            if (__instance.isProfessionChooser) return;
            if (!__instance.okButton.containsPoint(x, y)) return;

            if (MainClass.Config.LeftClickMainKey.JustPressed() || MainClass.Config.LeftClickAlternateKey.JustPressed())
                __instance.okButtonClicked();

            List<string> recipeInfoList = new();
            ___newCraftingRecipes.ForEach(recipe => recipeInfoList.Add(InventoryUtils.GetCraftingRecipeInfo(recipe)));
            string learnedRecipes = string.Join(", ", recipeInfoList);

            string extraInfo = string.Join(", ", ___extraInfoForLevel);

            object translationTokens = new
            {
                title = ___title,
                extra_info = extraInfo,
                learned_recipes = learnedRecipes
            };

            MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-level_up-ok_button", true, translationTokens);
        }
    }
}
