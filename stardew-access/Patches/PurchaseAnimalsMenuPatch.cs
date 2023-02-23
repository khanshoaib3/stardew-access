using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class PurchaseAnimalsMenuPatch
    {
        internal static FarmAnimal? animalBeingPurchased = null;
        internal static bool isOnFarm = false;
        internal static string purchaseAnimalMenuQuery = "";
        internal static PurchaseAnimalsMenu? purchaseAnimalsMenu;
        internal static bool firstTimeInNamingMenu = true;

        internal static void DrawPatch(PurchaseAnimalsMenu __instance, bool ___onFarm, bool ___namingAnimal, TextBox ___textBox, FarmAnimal ___animalBeingPurchased)
        {
            try
            {
                if (TextBoxPatch.isAnyTextBoxActive) return;

                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                purchaseAnimalsMenu = __instance;
                isOnFarm = ___onFarm;
                animalBeingPurchased = ___animalBeingPurchased;

                if (___onFarm && ___namingAnimal)
                {
                    string toSpeak = "";
                    if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                    {
                        toSpeak = "Cancel Button";
                    }
                    else if (__instance.doneNamingButton != null && __instance.doneNamingButton.containsPoint(x, y))
                    {
                        toSpeak = "OK Button";
                    }
                    else if (__instance.randomButton != null && __instance.randomButton.containsPoint(x, y))
                    {
                        toSpeak = "Random Name Button";
                    }
                    else if (__instance.textBoxCC != null && __instance.textBoxCC.containsPoint(x, y))
                    {
                        toSpeak = "Name Text Box";
                        // string? value = ___textBox.Text;
                        // if (value != "" && value != null && value != "null")
                        //     toSpeak = $"{toSpeak}, Value: {value}";
                    }

                    if (purchaseAnimalMenuQuery != toSpeak)
                    {
                        purchaseAnimalMenuQuery = toSpeak;

                        if (firstTimeInNamingMenu)
                        {
                            toSpeak = $"Enter the name of animal in the name text box. {toSpeak}";
                            firstTimeInNamingMenu = false;
                        }

                        MainClass.ScreenReader.Say(toSpeak, true);
                    }
                }
                else if (___onFarm && !___namingAnimal)
                {
                    firstTimeInNamingMenu = true;
                }
                else if (!___onFarm && !___namingAnimal)
                {
                    firstTimeInNamingMenu = true;
                    if (__instance.hovered != null)
                    {
                        string toSpeak = "";
                        if (((StardewValley.Object)__instance.hovered.item).Type != null)
                        {
                            toSpeak = ((StardewValley.Object)__instance.hovered.item).Type;
                        }
                        else
                        {
                            string displayName = PurchaseAnimalsMenu.getAnimalTitle(__instance.hovered.hoverText);
                            int price = __instance.hovered.item.salePrice();
                            string description = PurchaseAnimalsMenu.getAnimalDescription(__instance.hovered.hoverText);

                            toSpeak = $"{displayName}, Price: {price}g, Description: {description}";
                        }

                        if (purchaseAnimalMenuQuery != toSpeak)
                        {
                            purchaseAnimalMenuQuery = toSpeak;
                            MainClass.ScreenReader.Say(toSpeak, true);
                        }
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }


    }
}
