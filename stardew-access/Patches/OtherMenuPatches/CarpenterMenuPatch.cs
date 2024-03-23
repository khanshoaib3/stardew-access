using HarmonyLib;
using stardew_access.Translation;
using stardew_access.Utils;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class CarpenterMenuPatch : IPatch
{
    internal static CarpenterMenu? carpenterMenu = null;
    internal static bool isSayingBlueprintInfo = false;
    internal static string prevBlueprintInfo = "";
    internal static bool isOnFarm = false, isUpgrading = false, isDemolishing = false, isPainting = false, isConstructing = false, isMoving = false, isMagicalConstruction = false;

    public void Apply(Harmony harmony)
    {
        harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(CarpenterMenu), "draw"),
                prefix: new HarmonyMethod(typeof(CarpenterMenuPatch), nameof(CarpenterMenuPatch.DrawPatch))
        );
    }

    internal static void DrawPatch(CarpenterMenu __instance, List<Item> ___ingredients)
    {
        try
        {
            if (__instance.GetChildMenu() is BuildingSkinMenu) return;

            isOnFarm = __instance.onFarm;
            carpenterMenu = __instance;
            int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position

            if (!__instance.onFarm)
            {
                isUpgrading = false;
                isDemolishing = false;
                isPainting = false;
                isMoving = false;
                isConstructing = false;

                CarpenterMenu.BlueprintEntry currentBlueprint = __instance.Blueprint;
                if (currentBlueprint == null) return;

                bool isPrimaryInfoKeyPressed = MainClass.Config.PrimaryInfoKey.JustPressed();
                string blueprintInfo = GetCurrentBlueprintInfo(currentBlueprint, ___ingredients);

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
                    NarrateHoveredButton(__instance, x, y);
                }
            }
            else
            {
                if (__instance.demolishing)
                    isDemolishing = true;
                else if (__instance.upgrading)
                    isUpgrading = true;
                else if (__instance.painting)
                    isPainting = true;
                else if (__instance.moving)
                    isMoving = true;
                else
                    isConstructing = true;
            }
        }
        catch (Exception e)
        {
            Log.Error($"An error occurred in carpenter menu patch:\n{e.Message}\n{e.StackTrace}");
        }
    }

    private static string GetCurrentBlueprintInfo(CarpenterMenu.BlueprintEntry currentBlueprint, List<Item> ___ingredients)
    {
        string ingredients = "";
        List<string> ingredientsList = [];
        ___ingredients.ForEach(ingredient => ingredientsList.Add($"{InventoryUtils.GetPluralNameOfItem(ingredient)} {InventoryUtils.GetQualityFromItem(ingredient)}"));
        ingredients = string.Join(", ", ingredientsList);

        string translationKey = "menu-carpenter-blueprint_info";
        object? translationTokens = new
        {
            name = currentBlueprint.DisplayName,
            price = currentBlueprint.BuildCost,
            ingredients_list = ingredients,
            width = currentBlueprint.TilesWide,
            height = currentBlueprint.TilesHigh,
            description = currentBlueprint.Description,
            days = currentBlueprint.BuildDays,
        };

        return Translator.Instance.Translate(translationKey, translationTokens, TranslationCategory.Menu);
    }

    private static async void SpeakAndWait(string toSpeak)
    {
        isSayingBlueprintInfo = true;
        MainClass.ScreenReader.Say(toSpeak, true);
        await Task.Delay(300);
        isSayingBlueprintInfo = false;
    }

    private static void NarrateHoveredButton(CarpenterMenu __instance, int x, int y)
    {
        string translationKey = "";
        object? translationTokens = null;

        if (__instance.backButton != null && __instance.backButton.containsPoint(x, y))
        {
            translationKey = "menu-carpenter-previous_blueprint_button";
        }
        else if (__instance.forwardButton != null && __instance.forwardButton.containsPoint(x, y))
        {
            translationKey = "menu-carpenter-next_blueprint_button";
        }
        else if (__instance.demolishButton != null && __instance.demolishButton.containsPoint(x, y))
        {
            translationKey = "menu-carpenter-demolish_building_button";
            translationTokens = new { can_demolish = __instance.CanDemolishThis() ? 1 : 0 };
        }
        else if (__instance.okButton != null && __instance.okButton.containsPoint(x, y))
        {
            translationKey = "menu-carpenter-construct_building_button";
            translationTokens = new { can_construct = __instance.DoesFarmerHaveEnoughResourcesToBuild() ? 1 : 0 };
        }
        else if (__instance.moveButton != null && __instance.moveButton.containsPoint(x, y))
        {
            translationKey = "menu-carpenter-move_building_button";
        }
        else if (__instance.paintButton != null && __instance.paintButton.containsPoint(x, y))
        {
            translationKey = "menu-carpenter-paint_building_button";
        }
        else if (__instance.cancelButton != null && __instance.cancelButton.containsPoint(x, y))
        {
            translationKey = "common-ui-cancel_button";
        }
        else if (__instance.appearanceButton is { visible: true } && __instance.appearanceButton.containsPoint(x, y))
        {
            translationKey = "menu-carpenter-appearance_button";
        }
        else
        {
            return;
        }

        MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens);
    }

    internal static void Cleanup()
    {
        carpenterMenu = null;
        isSayingBlueprintInfo = false;
        prevBlueprintInfo = "";
        isUpgrading = false;
        isDemolishing = false;
        isPainting = false;
        isMoving = false;
        isConstructing = false;
        carpenterMenu = null;
    }
}
