using HarmonyLib;
using stardew_access.Translation;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches;

internal class CollectionsPagePatch : IPatch
{
    public void Apply(Harmony harmony)
    {
        harmony.Patch(
            original: AccessTools.DeclaredMethod(typeof(CollectionsPage), "draw"),
            postfix: new HarmonyMethod(typeof(CollectionsPagePatch), nameof(CollectionsPagePatch.DrawPatch))
        );
    }

    internal static void DrawPatch(CollectionsPage __instance, string ___hoverText)
    {
        try
        {
            int x = StardewValley.Game1.getMousePosition().X, y = StardewValley.Game1.getMousePosition().Y;
            string toSpeak = "";
            if (__instance.letterviewerSubMenu != null)
            {
                LetterViewerMenuPatch.NarrateMenu(__instance.letterviewerSubMenu);
                return;
            }
            foreach (KeyValuePair<int, ClickableTextureComponent> v in __instance.sideTabs)
            {
                if (v.Value.containsPoint(x, y))
                {
                    bool selected = __instance.currentTab == v.Key;
                    var texture = v.Value;
                    object tokens = new
                    {
                        tab_name = texture.hoverText,
                        is_selected = selected ? 1 : 0
                    };
                    MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-collections_page-tabs", true, tokens, TranslationCategory.Menu);
                    return;
                }
            }
            if (__instance.currentPage > 0 && __instance.backButton.containsPoint(x, y))
            {
                toSpeak = Translator.Instance.Translate("common-ui-back_button", translationCategory: TranslationCategory.Menu);
            }
            else if (__instance.currentPage < __instance.collections[__instance.currentTab].Count - 1 && __instance.forwardButton.containsPoint(x, y))
            {
                toSpeak = Translator.Instance.Translate("common-ui-forward_button", translationCategory: TranslationCategory.Menu);
            }
            else
            {
                foreach (ClickableTextureComponent c in __instance.collections[__instance.currentTab][__instance.currentPage])
                {
                    if (c.containsPoint(x, y))
                    {
                        string[] nameParts = ArgUtility.SplitBySpace(c.name);
                        string text = __instance.createDescription(nameParts[0]);
                        bool drawColor = Convert.ToBoolean(nameParts[1]);
                        bool drawColorFaded = __instance.currentTab == 4 && Convert.ToBoolean(nameParts[2]);
                        string name = ItemRegistry.GetDataOrErrorItem(nameParts[0]).DisplayName;
                        switch (__instance.currentTab)
                        {
                            case 4: // recipes
                                if (drawColorFaded)
                                {
                                    string uncooked = Translator.Instance.Translate("menu-collections_page-uncooked", TranslationCategory.Menu);
                                    toSpeak = $"{name} ({uncooked})";
                                }
                                else if (!drawColor)
                                {
                                    toSpeak = Translator.Instance.Translate("menu-crafting_page-unknown_recipe", TranslationCategory.Menu);
                                }
                                else
                                {
                                    toSpeak = text;
                                }
                                break;
                            case 5: // achievements
                                if (drawColor)
                                {    
                                    toSpeak = text;
                                }
                                else
                                {
                                    string unachieved = Translator.Instance.Translate("menu-collections_page-unachieved", TranslationCategory.Menu);
                                    if (nameParts[0] != "???")
                                    {
                                        int index = int.Parse(nameParts[0]);
                                        toSpeak = $"{Game1.achievements[index].Split('^')[0]}, ({unachieved})\n{Game1.achievements[index].Split('^')[1]}";
                                    }
                                    else
                                    {
                                        toSpeak = Translator.Instance.Translate("common-unknown");
                                    }
                                }
                                break;
                            case 6: // secret notes
                                if (drawColor)
                                {
                                    toSpeak = text;
                                }
                                else
                                {
                                    int index2 = int.Parse(nameParts[0]);
                                    toSpeak= ((index2 >= GameLocation.JOURNAL_INDEX) ? (Game1.content.LoadString("Strings\\Locations:Journal_Name") + " #" + (index2 - GameLocation.JOURNAL_INDEX)) : (Game1.content.LoadString("Strings\\Locations:Secret_Note_Name") + " #" + index2));
                                }
                                if (toSpeak.Length >= 2)
                                {    
                                    string lastTwoChars = toSpeak[^2..];
                                    if (int.TryParse(lastTwoChars, out int note_id))
                                    {
                                        if (note_id == 11 || (note_id >= 16 && note_id <= 21))
                                        {
                                            object token = new
                                            {
                                                note_id
                                            };
                                            string description = Translator.Instance.Translate("menu-letter_viewer-image_note", token, TranslationCategory.Menu);
                                            toSpeak = $"{toSpeak}{Environment.NewLine}{Environment.NewLine}{description}";
                                        }
                                    }
                                }
                                break;
                            case 7: // letters
                                toSpeak = Game1.parseText(c.name[(c.name.IndexOf(' ', c.name.IndexOf(' ') + 1) + 1)..]);
                                break;
                            default: // 0 shipping, 1 fish, 2 artifacts, 3 minerals
                                if (drawColor)
                                {
                                    toSpeak = text;
                                }
                                else
                                {
                                    string unfound = "";
                                    if (__instance.currentTab == 0)
                                        unfound = Translator.Instance.Translate("menu-collections_page-unshipped", TranslationCategory.Menu);
                                    else if (__instance.currentTab == 1)
                                        unfound = Translator.Instance.Translate("menu-collections_page-uncaught", TranslationCategory.Menu);
                                    else
                                        unfound = Translator.Instance.Translate("menu-collections_page-unfound", TranslationCategory.Menu);
                                    toSpeak = $"{name} ({unfound})";
                                }
                                break;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(toSpeak))
                MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
        }
        catch (System.Exception e)
        {
            Log.Error($"An error occurred in collections page patch:\n{e.Message}\n{e.StackTrace}");
        }
    }
}
