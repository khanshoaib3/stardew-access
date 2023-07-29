using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class LevelUpMenuPatch
    {
        private static string currentLevelUpTitle = "";

        internal static void DrawPatch(LevelUpMenu __instance, List<int> ___professionsToChoose, List<string> ___leftProfessionDescription, List<string> ___rightProfessionDescription, List<string> ___extraInfoForLevel, List<CraftingRecipe> ___newCraftingRecipes, string ___title, bool ___isActive, bool ___isProfessionChooser)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
                string leftProfession = "", rightProfession = "", extraInfo = "", newCraftingRecipe = "", toSpeak = "";

                if (!__instance.informationUp)
                    return;
                
                if (__instance.isProfessionChooser)
                {
                    if (___professionsToChoose.Count == 0) return;
                    
                    for (int j = 0; j < ___leftProfessionDescription.Count; j++)
                    {
                        leftProfession += ___leftProfessionDescription[j] + ", ";
                    }
                    for (int i = 0; i < ___rightProfessionDescription.Count; i++)
                    {
                        rightProfession += ___rightProfessionDescription[i] + ", ";
                    }

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

                        toSpeak = $"Selected: {leftProfession} Left click to choose.";
                    }

                    if (__instance.rightProfession.containsPoint(x, y))
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

                        toSpeak = $"Selected: {rightProfession} Left click to choose.";
                    }
                }
                else
                {
                    foreach (string s2 in ___extraInfoForLevel)
                    {
                        extraInfo += s2 + ", ";
                    }
                    foreach (CraftingRecipe s in ___newCraftingRecipes)
                    {
                        string cookingOrCrafting = Game1.content.LoadString("Strings\\UI:LearnedRecipe_" + (s.isCookingRecipe ? "cooking" : "crafting"));
                        string message = Game1.content.LoadString("Strings\\UI:LevelUp_NewRecipe", cookingOrCrafting, s.DisplayName);

                        newCraftingRecipe += $"{message}, ";
                    }
                }

                if (__instance.okButton.containsPoint(x, y))
                {
                    if (MainClass.Config.LeftClickMainKey.JustPressed() || MainClass.Config.LeftClickAlternateKey.JustPressed())
                        __instance.okButtonClicked();

                    toSpeak = $"{___title} {extraInfo} {newCraftingRecipe}. Left click to close.";
                }

                if (toSpeak != "")
                    MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
                else if (__instance.isProfessionChooser && currentLevelUpTitle != $"{___title}. Select a new profession.")
                {
                    MainClass.ScreenReader.SayWithMenuChecker($"{___title}. Select a new profession.", true);
                    currentLevelUpTitle = $"{___title}. Select a new profession.";
                }
            }
            catch (Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        internal static void Cleanup()
        {
            currentLevelUpTitle = "";
        }
    }
}
