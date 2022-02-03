using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class BuildingNAnimalMenuPatches
    {
        internal static Vector2[] marked = new Vector2[10];
        internal static Building[] availableBuildings = new Building[100];
        internal static string carpenterMenuQuery = "";
        internal static bool isSayingBlueprintInfo = false;
        internal static string prevBlueprintInfo = "";
        internal static bool isOnFarm = false, isUpgrading = false, isDemolishing = false, isPainting = false, isConstructing = false, isMoving = false;
        internal static void CarpenterMenuPatch(
            CarpenterMenu __instance, bool ___onFarm, List<Item> ___ingredients, int ___price,
            List<BluePrint> ___blueprints, int ___currentBlueprintIndex, bool ___upgrading, bool ___demolishing, bool ___moving,
            bool ___painting)
        {
            try
            {
                isOnFarm = ___onFarm;
                if (!___onFarm)
                {
                    isUpgrading = false;
                    isDemolishing = false;
                    isPainting = false;
                    isMoving = false;
                    isConstructing = false;

                    #region The blueprint menu
                    BluePrint currentBluprint = __instance.CurrentBlueprint;
                    if (currentBluprint == null)
                        return;

                    int x = Game1.getMouseX(), y = Game1.getMouseY(); // Mouse x and y position
                    bool isBPressed = Game1.input.GetKeyboardState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.B);
                    string ingredients = "";
                    string name = currentBluprint.displayName;
                    string upgradeName = currentBluprint.nameOfBuildingToUpgrade;
                    string description = currentBluprint.description;
                    string price = $"{___price}g";
                    string blueprintInfo;

                    #region Get ingredients
                    for (int i = 0; i < ___ingredients.Count; i++)
                    {
                        string itemName = ___ingredients[i].DisplayName;
                        int itemStack = ___ingredients[i].Stack;
                        string itemQuality = "";

                        int qualityValue = ((StardewValley.Object)___ingredients[i]).quality;
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

                    blueprintInfo = $"{name}, Price: {price}, Ingredients: {ingredients}, Description: {description}";

                    if (isBPressed && !isSayingBlueprintInfo)
                    {
                        SayBlueprintInfo(blueprintInfo);
                    }
                    else if (prevBlueprintInfo != blueprintInfo)
                    {
                        prevBlueprintInfo = blueprintInfo;
                        SayBlueprintInfo(blueprintInfo);
                    }
                    else
                    {
                        if (__instance.backButton != null && __instance.backButton.containsPoint(x, y))
                        {
                            string toSpeak = "Previous Blueprint";
                            if (carpenterMenuQuery != toSpeak)
                            {
                                carpenterMenuQuery = toSpeak;
                                MainClass.screenReader.Say(toSpeak, true);
                            }
                            return;
                        }

                        if (__instance.forwardButton != null && __instance.forwardButton.containsPoint(x, y))
                        {
                            string toSpeak = "Next Blueprint";
                            if (carpenterMenuQuery != toSpeak)
                            {
                                carpenterMenuQuery = toSpeak;
                                MainClass.screenReader.Say(toSpeak, true);
                            }
                            return;
                        }

                        if (__instance.demolishButton != null && __instance.demolishButton.containsPoint(x, y))
                        {
                            string toSpeak = $"Demolish Building" + (__instance.CanDemolishThis(___blueprints[___currentBlueprintIndex]) ? "" : ", cannot demolish building");
                            if (carpenterMenuQuery != toSpeak)
                            {
                                carpenterMenuQuery = toSpeak;
                                MainClass.screenReader.Say(toSpeak, true);
                            }
                            return;
                        }

                        if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
                        {
                            string toSpeak = "Cunstruct Building" + (___blueprints[___currentBlueprintIndex].doesFarmerHaveEnoughResourcesToBuild() ? "" : ", cannot cunstrut building, not enough resources to build.");
                            if (carpenterMenuQuery != toSpeak)
                            {
                                carpenterMenuQuery = toSpeak;
                                MainClass.screenReader.Say(toSpeak, true);
                            }
                            return;
                        }

                        if (__instance.moveButton != null && __instance.moveButton.containsPoint(x, y))
                        {
                            string toSpeak = "Move Building";
                            if (carpenterMenuQuery != toSpeak)
                            {
                                carpenterMenuQuery = toSpeak;
                                MainClass.screenReader.Say(toSpeak, true);
                            }
                            return;
                        }

                        if (__instance.paintButton != null && __instance.paintButton.containsPoint(x, y))
                        {
                            string toSpeak = "Paint Building";
                            if (carpenterMenuQuery != toSpeak)
                            {
                                carpenterMenuQuery = toSpeak;
                                MainClass.screenReader.Say(toSpeak, true);
                            }
                            return;
                        }

                        if (__instance.cancelButton != null && __instance.cancelButton.containsPoint(x, y))
                        {
                            string toSpeak = "Cancel Button";
                            if (carpenterMenuQuery != toSpeak)
                            {
                                carpenterMenuQuery = toSpeak;
                                MainClass.screenReader.Say(toSpeak, true);
                            }
                            return;
                        }
                    }
                    #endregion
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
                MainClass.monitor.Log($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}", LogLevel.Error);
            }
        }
        private static async void SayBlueprintInfo(string info)
        {
            isSayingBlueprintInfo = true;
            MainClass.screenReader.Say(info, true);
            await Task.Delay(300);
            isSayingBlueprintInfo = false;
        }
    }

}