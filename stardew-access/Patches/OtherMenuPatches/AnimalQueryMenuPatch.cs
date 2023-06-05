using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class AnimalQueryMenuPatch
    {
        internal static bool isNarratingAnimalInfo = false;
        internal static string animalQueryMenuQuery = "";
        internal static AnimalQueryMenu? animalQueryMenu;
        internal static FarmAnimal? animalBeingMoved = null;
        internal static bool isOnFarm = false;

        private static double loveLevel;

        internal static void DrawPatch(AnimalQueryMenu __instance, bool ___confirmingSell, FarmAnimal ___animal, TextBox ___textBox, string ___parentName, bool ___movingAnimal, double ___loveLevel)
        {
            try
            {
                if (TextBoxPatch.isAnyTextBoxActive) return;

                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

                isOnFarm = ___movingAnimal;
                animalQueryMenu = __instance;
                animalBeingMoved = ___animal;

                loveLevel = ___loveLevel;

                narrateAnimalDetailsOnKeyPress(___animal, ___parentName);

                narrateHoveredButton(__instance, ___animal, ___confirmingSell, x, y);
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"An error occurred in AnimalQueryMenuPatch()->DrawPatch():\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void narrateAnimalDetailsOnKeyPress(FarmAnimal ___animal, string ___parentName)
        {
            bool isPrimaryInfoKeyPressed = MainClass.Config.PrimaryInfoKey.JustPressed();
            if (!isPrimaryInfoKeyPressed | isNarratingAnimalInfo)
                return;

            string name = ___animal.displayName;
            string type = ___animal.displayType;
            int age = (___animal.GetDaysOwned() + 1) / 28 + 1;
            string babyText = "";
            string parent = "null";

            if ((int)___animal.age.Value < (byte)___animal.ageWhenMature.Value)
            {
                babyText = Game1.content.LoadString("Strings\\UI:AnimalQuery_AgeBaby").Trim();
            }
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

            string toSpeak = MainClass.Translate("animal_query_menu-animal_info", new { name = name, type = type, baby = babyText, heart_count = heartCount, age = age, parent_name = parent });

            isNarratingAnimalInfo = true;
            Task.Delay(200).ContinueWith(_ => { isNarratingAnimalInfo = false; }); // Adds delay

            MainClass.ScreenReader.Say(toSpeak, true);
        }

        private static void narrateHoveredButton(AnimalQueryMenu __instance, FarmAnimal ___animal, bool ___confirmingSell, int x, int y)
        {
            string toSpeak = "";
            if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                toSpeak = MainClass.Translate("common-ui-ok_button");
            else if (__instance.sellButton != null && __instance.sellButton.containsPoint(x, y))
                toSpeak = MainClass.Translate("animal_query_menu-ui-selling_button", new {price = ___animal.getSellPrice()});
            else if (___confirmingSell && __instance.yesButton != null && __instance.yesButton.containsPoint(x, y))
                toSpeak = MainClass.Translate("animal_query_menu-ui-confirm_selling_button");
            else if (___confirmingSell && __instance.noButton != null && __instance.noButton.containsPoint(x, y))
                toSpeak = MainClass.Translate("animal_query_menu-ui-cancel_selling_button");
            else if (__instance.moveHomeButton != null && __instance.moveHomeButton.containsPoint(x, y))
                toSpeak = MainClass.Translate("animal_query_menu-ui-move_home_button");
            else if (__instance.allowReproductionButton != null && __instance.allowReproductionButton.containsPoint(x, y))
                toSpeak = MainClass.Translate("animal_query_menu-ui-allow_reproduction_button", new {checkbox_value = (___animal.allowReproduction.Value ? 1 : 0)});
            else if (__instance.textBoxCC != null && __instance.textBoxCC.containsPoint(x, y))
                toSpeak = MainClass.Translate("animal_query_menu-ui-text_box");

            if (animalQueryMenuQuery != toSpeak)
            {
                animalQueryMenuQuery = toSpeak;
                MainClass.ScreenReader.Say($"{toSpeak}", true);
            }
        }

        internal static void Cleanup()
        {
            AnimalQueryMenuPatch.animalQueryMenuQuery = "";
            AnimalQueryMenuPatch.animalQueryMenu = null;
        }
    }
}
