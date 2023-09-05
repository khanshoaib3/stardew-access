using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Translation;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class SocialPagePatch : IPatch
    {
        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(SocialPage), nameof(SocialPage.draw),
                    new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(SocialPagePatch), nameof(SocialPagePatch.DrawPatch))
            );
        }

        private static void DrawPatch(SocialPage __instance, List<ClickableTextureComponent> ___sprites,
            int ___slotPosition, List<string> ___kidsNames)
        {
            try
            {
                int x = Game1.getMouseX(true), y = Game1.getMouseY(true); // Mouse x and y position
                for (int i = ___slotPosition; i < ___slotPosition + 5; i++)
                {
                    if (i >= ___sprites.Count)
                        continue;

                    if (__instance.names[i] is string && NarrateNPCDetails(__instance, i, ___kidsNames, x, y))
                    {
                        return;
                    }
                    else if (__instance.names[i] is long && NarrateFarmerDetails(__instance, i, ___sprites, x, y))
                    {
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred in social page patch:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static bool NarrateNPCDetails(SocialPage __instance, int i, List<string> ___kidsNames, int x, int y)
        {
            if (!__instance.characterSlots[i].bounds.Contains(x, y))
                return false;

            string name = $"{__instance.names[i] as string}";
            int heartLevel = Game1.player.getFriendshipHeartLevelForNPC(name);
            bool datable = SocialPage.isDatable(name);
            Friendship friendship = __instance.getFriendship(name);
            int giftsThisWeek = friendship.GiftsThisWeek;
            bool hasTalked = Game1.player.hasPlayerTalkedToNPC(name);
            bool spouse = friendship.IsMarried();
            bool housemate = spouse && SocialPage.isRoommateOfAnyone(name);
            ___kidsNames.Add("Robin");
            ___kidsNames.Add("Pierre");
            ___kidsNames.Add("Caroline");
            ___kidsNames.Add("Jodi");
            ___kidsNames.Add("Kent");
            ___kidsNames.Add("George");
            ___kidsNames.Add("Evelyn");
            ___kidsNames.Add("Demetrius");

            string relationshipStatus = "null";
            if (datable | housemate)
            {
                #region Taken from the source code

                relationshipStatus =
                    (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.pt)
                        ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635")
                        : ((__instance.getGender(name) == 0)
                            ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/')
                                .First()
                            : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/')
                                .Last());
                if (housemate)
                {
                    relationshipStatus = Game1.content.LoadString("Strings\\StringsFromCSFiles:Housemate");
                }
                else if (spouse)
                {
                    relationshipStatus = ((__instance.getGender(name) == 0)
                        ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11636")
                        : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11637"));
                }
                else if (__instance.isMarriedToAnyone(name))
                {
                    relationshipStatus = ((__instance.getGender(name) == 0)
                        ? Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_MaleNPC")
                        : Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_FemaleNPC"));
                }
                else if (!Game1.player.isMarried() && friendship.IsDating())
                {
                    relationshipStatus = ((__instance.getGender(name) == 0)
                        ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11639")
                        : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11640"));
                }
                else if (__instance.getFriendship(name).IsDivorced())
                {
                    relationshipStatus = ((__instance.getGender(name) == 0)
                        ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11642")
                        : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11643"));
                }

                #endregion
            }

            MainClass.ScreenReader.TranslateAndSayWithMenuChecker("menu-social_page-npc_info", true, new
            {
                name = name,
                has_talked = hasTalked ? 1 : 0,
                relationship_status = relationshipStatus,
                heart_level = heartLevel,
                gifts_this_week = giftsThisWeek
            });
            return true;
        }

        private static bool NarrateFarmerDetails(SocialPage __instance, int i,
            List<ClickableTextureComponent> ___sprites, int x, int y)
        {
            long farmerID = (long)__instance.names[i];
            Farmer farmer = Game1.getFarmerMaybeOffline(farmerID);
            if (farmer == null)
                return false;

            int gender = (!farmer.IsMale) ? 1 : 0;
            ClickableTextureComponent clickableTextureComponent = ___sprites[i];
            if (!clickableTextureComponent.containsPoint(x, y))
                return false;

            Friendship friendship = Game1.player.team.GetFriendship(Game1.player.UniqueMultiplayerID, farmerID);
            bool spouse = friendship.IsMarried();
            string toSpeak = "";

            string text2 = (LocalizedContentManager.CurrentLanguageCode != LocalizedContentManager.LanguageCode.pt)
                ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635")
                : ((gender == 0)
                    ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/').First()
                    : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11635").Split('/').Last());
            if (spouse)
            {
                text2 = ((gender == 0)
                    ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11636")
                    : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11637"));
            }
            else if (farmer.isMarried() && !farmer.hasRoommate())
            {
                text2 = ((gender == 0)
                    ? Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_MaleNPC")
                    : Game1.content.LoadString("Strings\\UI:SocialPage_MarriedToOtherPlayer_FemaleNPC"));
            }
            else if (!Game1.player.isMarried() && friendship.IsDating())
            {
                text2 = ((gender == 0)
                    ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11639")
                    : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11640"));
            }
            else if (friendship.IsDivorced())
            {
                text2 = ((gender == 0)
                    ? Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11642")
                    : Game1.content.LoadString("Strings\\StringsFromCSFiles:SocialPage.cs.11643"));
            }

            toSpeak = $"{farmer.displayName}, {text2}";

            MainClass.ScreenReader.SayWithMenuChecker(toSpeak, true);
            return true;
        }
    }
}