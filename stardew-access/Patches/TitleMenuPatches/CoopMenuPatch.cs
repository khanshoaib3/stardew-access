using System.Reflection;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class CoopMenuPatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(CoopMenu), nameof(CoopMenu.update), new Type[] { typeof(GameTime) }),
                postfix: new HarmonyMethod(typeof(CoopMenuPatch), nameof(UpdatePatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CoopMenu).GetNestedType("LabeledSlot", BindingFlags.NonPublic | BindingFlags.Instance), "Draw", new Type[] { typeof(SpriteBatch), typeof(int) }),
                postfix: new HarmonyMethod(typeof(CoopMenuPatch), nameof(LabeledSlot_DrawPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(CoopMenu).GetNestedType("FriendFarmSlot", BindingFlags.NonPublic | BindingFlags.Instance), "Draw", new Type[] { typeof(SpriteBatch), typeof(int) }),
                postfix: new HarmonyMethod(typeof(CoopMenuPatch), nameof(FriendFarmSlot_DrawPatch))
            );
        }

        private static void UpdatePatch(CoopMenu __instance, CoopMenu.Tab ___currentTab, bool ___isSetUp)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true);
                string translationKey = "";
                object? translationTokens = null;
                if (!___isSetUp)
                {
                    MainClass.ScreenReader.SayWithMenuChecker(Game1.content.LoadString("Strings\\UI:CoopMenu_ConnectingOnlineServices"), true);
                    return;
                }

                if (__instance.joinTab.containsPoint(x, y))
                {
                    translationKey = "menu-co_op-join_tab_button";
                    translationTokens = new
                    {
                        is_selected = ___currentTab == CoopMenu.Tab.JOIN_TAB ? 1 : 0,
                    };
                }
                else if (__instance.hostTab.containsPoint(x, y))
                {
                    translationKey = "menu-co_op-host_tab_button";
                    translationTokens = new
                    {
                        is_selected = ___currentTab == CoopMenu.Tab.HOST_TAB ? 1 : 0,
                    };
                }
                else if (___currentTab == CoopMenu.Tab.JOIN_TAB && __instance.refreshButton.containsPoint(x, y))
                {
                    translationKey = "menu-co_op-refresh_button";
                }
                else if (___currentTab == CoopMenu.Tab.JOIN_TAB && __instance.upArrow != null && __instance.upArrow.containsPoint(x, y))
                {
                    translationKey = "common-ui-scroll_up_button";
                }
                else if (__instance.downArrow != null && __instance.downArrow.containsPoint(x, y))
                {
                    translationKey = "common-ui-scroll_down_button";
                }

                MainClass.ScreenReader.TranslateAndSayWithMenuChecker(translationKey, true, translationTokens);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in co-op menu patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void LabeledSlot_DrawPatch(int i, CoopMenu ___menu, string ___message)
        {
            try
            {
                if (!___menu.slotButtons[i].visible || !___menu.slotButtons[i].containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                    return;

                MainClass.ScreenReader.SayWithMenuChecker(___message, true);
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in CoopMenu.LabeledSlot patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void FriendFarmSlot_DrawPatch(int i, CoopMenu ___menu, object ___Farm)
        {
            try
            {
                if (!___menu.slotButtons[i].visible || !___menu.slotButtons[i].containsPoint(Game1.getMouseX(true), Game1.getMouseY(true)))
                    return;

                string? farmName = ___Farm.GetType().GetField("FarmName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(___Farm)?.ToString();
                string? ownerName = ___Farm.GetType().GetField("OwnerName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(___Farm)?.ToString();
                bool? previouslyJoined = (bool?)(___Farm.GetType().GetField("PreviouslyJoined", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(___Farm));
                WorldDate? date = (WorldDate?)___Farm.GetType().GetField("Date", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(___Farm);

                if (farmName == null || ownerName == null || date == null || previouslyJoined == null)
                    return;

                string path = previouslyJoined == true ? "Strings\\UI:CoopMenu_RevisitFriendFarm" : "Strings\\UI:CoopMenu_JoinFriendFarm";
                farmName = Game1.content.LoadString(path, farmName);

                MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-co_op-friend_hosted_farm_details", true, new
                {
                    farm_name = farmName,
                    owner_name = ownerName,
                    date = date.Localize()
                });
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in CoopMenu.FriendFarmSlot patch:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
