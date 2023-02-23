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

        internal static void DrawPatch(AnimalQueryMenu __instance, bool ___confirmingSell, FarmAnimal ___animal, TextBox ___textBox, string ___parentName, bool ___movingAnimal)
        {
            try
            {
                if (TextBoxPatch.isAnyTextBoxActive) return;

                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                bool isPrimaryInfoKeyPressed = MainClass.Config.PrimaryInfoKey.JustPressed(); // For narrating animal details
                string toSpeak = " ", details = " ";

                isOnFarm = ___movingAnimal;
                animalQueryMenu = __instance;
                animalBeingMoved = ___animal;

                if (isPrimaryInfoKeyPressed & !isNarratingAnimalInfo)
                {
                    string name = ___animal.displayName;
                    string type = ___animal.displayType;
                    int age = (___animal.GetDaysOwned() + 1) / 28 + 1;
                    string ageText = (age <= 1) ? Game1.content.LoadString("Strings\\UI:AnimalQuery_Age1") : Game1.content.LoadString("Strings\\UI:AnimalQuery_AgeN", age);
                    string parent = "";
                    if ((int)___animal.age.Value < (byte)___animal.ageWhenMature.Value)
                    {
                        ageText += Game1.content.LoadString("Strings\\UI:AnimalQuery_AgeBaby");
                    }
                    if (___parentName != null)
                    {
                        parent = Game1.content.LoadString("Strings\\UI:AnimalQuery_Parent", ___parentName);
                    }

                    details = $"Name: {name} Type: {type} \n\t Age: {ageText} {parent}";
                    animalQueryMenuQuery = "";

                    isNarratingAnimalInfo = true;
                    Task.Delay(200).ContinueWith(_ => { isNarratingAnimalInfo = false; });
                }

                if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                    toSpeak = "OK button";
                else if (__instance.sellButton != null && __instance.sellButton.containsPoint(x, y))
                    toSpeak = $"Sell for {___animal.getSellPrice()}g button";
                else if (___confirmingSell && __instance.yesButton != null && __instance.yesButton.containsPoint(x, y))
                    toSpeak = "Confirm selling animal";
                else if (___confirmingSell && __instance.noButton != null && __instance.noButton.containsPoint(x, y))
                    toSpeak = "Cancel selling animal";
                else if (__instance.moveHomeButton != null && __instance.moveHomeButton.containsPoint(x, y))
                    toSpeak = "Change home building button";
                else if (__instance.allowReproductionButton != null && __instance.allowReproductionButton.containsPoint(x, y))
                    toSpeak = ((___animal.allowReproduction.Value) ? "Enabled" : "Disabled") + " allow reproduction button";
                else if (__instance.textBoxCC != null && __instance.textBoxCC.containsPoint(x, y))
                    toSpeak = "Animal name text box";

                if (animalQueryMenuQuery != toSpeak)
                {
                    animalQueryMenuQuery = toSpeak;
                    MainClass.ScreenReader.Say($"{details} {toSpeak}", true);
                }
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"An error occured in AnimalQueryMenuPatch()->DrawPatch():\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
