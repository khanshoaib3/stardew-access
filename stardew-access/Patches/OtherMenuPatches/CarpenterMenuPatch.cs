using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class CarpenterMenuPatch
    {
        internal static CarpenterMenu? carpenterMenu = null;
        internal static string carpenterMenuQuery = "";
        internal static bool isSayingBlueprintInfo = false;
        internal static string prevBlueprintInfo = "";
        internal static bool isOnFarm = false, isUpgrading = false, isDemolishing = false, isPainting = false, isConstructing = false, isMoving = false, isMagicalConstruction = false;

        internal static void DrawPatch(
            CarpenterMenu __instance, bool ___onFarm, List<Item> ___ingredients, int ___price,
            List<BluePrint> ___blueprints, int ___currentBlueprintIndex, bool ___upgrading, bool ___demolishing, bool ___moving,
            bool ___painting, bool ___magicalConstruction)
        {
            try
            {
                isOnFarm = ___onFarm;
                carpenterMenu = __instance;
                isMagicalConstruction = ___magicalConstruction;
                if (!___onFarm)
                {
                    isUpgrading = false;
                    isDemolishing = false;
                    isPainting = false;
                    isMoving = false;
                    isConstructing = false;

                    BluePrint currentBlueprint = __instance.CurrentBlueprint;
                    if (currentBlueprint == null)
                        return;

                    int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                    bool isPrimaryInfoKeyPressed = MainClass.Config.PrimaryInfoKey.JustPressed();
                    string blueprintInfo = GetCurrentBlueprintInfo(currentBlueprint, ___price, ___ingredients);

                    if (isPrimaryInfoKeyPressed && !isSayingBlueprintInfo)
                    {
                        SpeakAndWait(blueprintInfo);
                    }
                    else if (prevBlueprintInfo != blueprintInfo)
                    {
                        prevBlueprintInfo = blueprintInfo;
                        SpeakAndWait(blueprintInfo);
                    }
                    else
                    {
                        NarrateHoveredButton(__instance, ___blueprints, ___currentBlueprintIndex, x, y);
                    }
                }
                else
                {
                    if (___demolishing)
                        isDemolishing = true;
                    else if (___upgrading)
                        isUpgrading = true;
                    else if (___painting)
                        isPainting = true;
                    else if (___moving)
                        isMoving = true;
                    else
                        isConstructing = true;
                }
            }
            catch (Exception e)
            {
                Log.Error($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static string GetCurrentBlueprintInfo(BluePrint currentBlueprint, int ___price, List<Item> ___ingredients)
        {
            string ingredients = "";
            string name = currentBlueprint.displayName;
            string upgradeName = currentBlueprint.nameOfBuildingToUpgrade;
            string description = currentBlueprint.description;
            string price = $"{___price}g";
            int width = currentBlueprint.tilesWidth;
            int height = currentBlueprint.tilesHeight;

            #region Get ingredients
            for (int i = 0; i < ___ingredients.Count; i++)
            {
                string itemName = ___ingredients[i].DisplayName;
                int itemStack = ___ingredients[i].Stack;
                string itemQuality = "";

                int qualityValue = ((StardewValley.Object)___ingredients[i]).Quality;
                if (qualityValue == 1)
                {
                    itemQuality = "Silver quality";
                }
                else if (qualityValue == 2 || qualityValue == 3)
                {
                    itemQuality = "Gold quality";
                }
                else if (qualityValue >= 4)
                {
                    itemQuality = "Iridium quality";
                }

                ingredients = $"{ingredients}, {itemStack} {itemName} {itemQuality}";
            }
            #endregion

            return $"{name}, Price: {price}, Ingredients: {ingredients}, Dimensions: {width} width and {height} height, Description: {description}";
        }

        private static async void SpeakAndWait(string toSpeak)
        {
            isSayingBlueprintInfo = true;
            MainClass.ScreenReader.Say(toSpeak, true);
            await Task.Delay(300);
            isSayingBlueprintInfo = false;
        }

        private static void NarrateHoveredButton(CarpenterMenu __instance, List<BluePrint> ___blueprints, int ___currentBlueprintIndex, int x, int y)
        {
            string toSpeak = "";
            if (__instance.backButton != null && __instance.backButton.containsPoint(x, y))
            {
                toSpeak = "Previous Blueprint";
            }
            else if (__instance.forwardButton != null && __instance.forwardButton.containsPoint(x, y))
            {
                toSpeak = "Next Blueprint";
            }
            else if (__instance.demolishButton != null && __instance.demolishButton.containsPoint(x, y))
            {
                toSpeak = $"Demolish Building" + (__instance.CanDemolishThis(___blueprints[___currentBlueprintIndex]) ? "" : ", cannot demolish building");
            }
            else if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
            {
                toSpeak = "Construct Building" + (___blueprints[___currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild() ? "" : ", cannot cunstrut building, not enough resources to build.");
            }
            else if (__instance.moveButton != null && __instance.moveButton.containsPoint(x, y))
            {
                toSpeak = "Move Building";
            }
            else if (__instance.paintButton != null && __instance.paintButton.containsPoint(x, y))
            {
                toSpeak = "Paint Building";
            }
            else if (__instance.cancelButton != null && __instance.cancelButton.containsPoint(x, y))
            {
                toSpeak = "Cancel Button";
            }
            else
            {
                return;
            }

            if (carpenterMenuQuery != toSpeak)
            {
                carpenterMenuQuery = toSpeak;
                MainClass.ScreenReader.Say(toSpeak, true);
            }
        }

        internal static void Cleanup()
        {
            CarpenterMenuPatch.carpenterMenuQuery = "";
            CarpenterMenuPatch.isUpgrading = false;
            CarpenterMenuPatch.isDemolishing = false;
            CarpenterMenuPatch.isPainting = false;
            CarpenterMenuPatch.isMoving = false;
            CarpenterMenuPatch.isConstructing = false;
            CarpenterMenuPatch.carpenterMenu = null;
        }
    }
}
