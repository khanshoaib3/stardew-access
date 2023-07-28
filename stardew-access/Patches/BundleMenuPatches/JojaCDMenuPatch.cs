using StardewValley;
using StardewValley.Menus;
using stardew_access.Translation;

namespace stardew_access.Patches
{
    internal class JojaCDMenuPatch
    {
        internal static void DrawPatch(JojaCDMenu __instance)
        {
            try
            {
                int x = Game1.getMouseX(true),
                    y = Game1.getMouseY(true); // Mouse x and y position
                string toSpeak = "";

                for (int i = 0; i < __instance.checkboxes.Count; i++)
                {
                    ClickableComponent c = __instance.checkboxes[i];
                    if (!c.containsPoint(x, y))
                        continue;

                    if (c.name.Equals("complete"))
                    {
                        toSpeak = Translator.Instance.Translate(
                            "menu-bundle-completed-prefix",
                            new
                            {
                                content = Translator.Instance.Translate(
                                    "menu-joja_cd-project_name",
                                    new { project_index = i }
                                ),
                            }
                        );
                    }
                    else
                    {
                        toSpeak = Translator.Instance.Translate(
                            "menu-joja_cd-project_info",
                            new
                            {
                                name = Translator.Instance.Translate(
                                    "menu-joja_cd-project_name",
                                    new { project_index = i }
                                ),
                                price = __instance.getPriceFromButtonNumber(i),
                                description = __instance.getDescriptionFromButtonNumber(i)
                            }
                        );
                    }

                    break;
                }


                MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"Unable to narrate Text:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
