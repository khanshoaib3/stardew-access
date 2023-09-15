using StardewValley;
using StardewValley.Menus;
using stardew_access.Translation;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;

namespace stardew_access.Patches
{
    internal class AnimalQueryMenuPatch : IPatch
    {
        internal static bool isNarratingAnimalInfo = false;
        internal static AnimalQueryMenu? animalQueryMenu;
        internal static FarmAnimal? animalBeingMoved = null;
        internal static bool isOnFarm = false;

        private static double loveLevel;
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(AnimalQueryMenu), nameof(AnimalQueryMenu.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(AnimalQueryMenuPatch), nameof(AnimalQueryMenuPatch.DrawPatch))
            );
        }

        internal static void DrawPatch(
            AnimalQueryMenu __instance,
            bool ___confirmingSell,
            FarmAnimal ___animal,
            TextBox ___textBox,
            string ___parentName,
            bool ___movingAnimal,
            double ___loveLevel
        )
        {
            try
            {
                if (TextBoxPatch.IsAnyTextBoxActive)
                    return;

                int x = Game1.getMouseX(true),
                    y = Game1.getMouseY(true); // Mouse x and y position

                isOnFarm = ___movingAnimal;
                animalQueryMenu = __instance;
                animalBeingMoved = ___animal;

                loveLevel = ___loveLevel;

                NarrateAnimalDetailsOnKeyPress(___animal, ___parentName);

                NarrateHoveredButton(__instance, ___animal, ___confirmingSell, x, y);
            }
            catch (System.Exception e)
            {
                Log.Error(
                    $"An error occurred in AnimalQueryMenuPatch()->DrawPatch():\n{e.Message}\n{e.StackTrace}"
                );
            }
        }

        private static void NarrateAnimalDetailsOnKeyPress( FarmAnimal ___animal, string ___parentName)
        {
            bool isPrimaryInfoKeyPressed = MainClass.Config.PrimaryInfoKey.JustPressed();
            if (!isPrimaryInfoKeyPressed | isNarratingAnimalInfo)
                return;

            isNarratingAnimalInfo = true;
            
            string name = ___animal.displayName;
            string type = ___animal.displayType;
            int age = (___animal.GetDaysOwned() + 1) / 28 + 1;
            string parent = "null";

            if (___parentName != null)
            {
                parent = Game1.content.LoadString("Strings\\UI:AnimalQuery_Parent", ___parentName);
            }

            // The loveLevel varies between 0 and 1
            // 1 indicates 5 hearts and similarily 0 indicates 0 hearts
            // the below code multiplies the loveLevel by 10 and
            // the numeric value of the resultent is divided by 2 to give the number of full hearts and
            // if its decimal value is above 0.5, then that indicates half a heart
            double heartCount = Math.Floor(loveLevel * 10);
            double remainder = (loveLevel * 10) % 1;
            heartCount /= 2;
            if (remainder >= 0.5)
            {
                heartCount += 0.5;
            }

            MainClass.ScreenReader.TranslateAndSay("animal_query_menu-animal_info", true, new
                {
                    name,
                    type,
                    is_baby = ___animal.isBaby() ? 1 : 0,
                    heart_count = heartCount,
                    age,
                    parent_name = parent
                },
                TranslationCategory.Menu);
            Task.Delay(200).ContinueWith(_ => { isNarratingAnimalInfo = false; }); // Adds delay
        }

        private static void NarrateHoveredButton(
            AnimalQueryMenu __instance,
            FarmAnimal ___animal,
            bool ___confirmingSell,
            int x,
            int y
        )
        {
            string translationKey = "";
            object? translationTokens = null;

            if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                translationKey = "common-ui-ok_button";
            else if (__instance.sellButton != null && __instance.sellButton.containsPoint(x, y))
            {
                translationKey = "menu-animal_query-selling_button";
                translationTokens = new { price = ___animal.getSellPrice() };
            }
            else if (___confirmingSell && __instance.yesButton != null && __instance.yesButton.containsPoint(x, y))
                translationKey = "menu-animal_query-confirm_selling_button";
            else if (___confirmingSell && __instance.noButton != null && __instance.noButton.containsPoint(x, y))
                translationKey = "menu-animal_query-cancel_selling_button";
            else if (__instance.moveHomeButton != null && __instance.moveHomeButton.containsPoint(x, y))
                translationKey = "menu-animal_query-move_home_button";
            else if (__instance.allowReproductionButton != null && __instance.allowReproductionButton.containsPoint(x, y))
            {
                translationKey = "menu-animal_query-allow_reproduction_button";
                translationTokens = new { checkbox_value = (___animal.allowReproduction.Value ? 1 : 0) };
            }
            else if (__instance.textBoxCC != null && __instance.textBoxCC.containsPoint(x, y))
                translationKey = "menu-animal_query-text_box";

            MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens);
        }

        internal static void Cleanup()
        {
            AnimalQueryMenuPatch.animalQueryMenu = null;
        }
    }
}
