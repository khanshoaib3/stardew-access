using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class FishingMiniGamePatch : IPatch
    {
        private static ICue? bobberSound = null;
        private static float previousDistanceFromCatching = 0f;

        public void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(BobberBar), nameof(BobberBar.update)),
                postfix: new HarmonyMethod(typeof(FishingMiniGamePatch), nameof(BobberBarPatch))
            );

        }

        private static void BobberBarPatch(BobberBar __instance, ref float ___difficulty, ref int ___motionType, float ___distanceFromCatching, float ___bobberPosition, float ___bobberBarPos, float ___bobberBarSpeed, bool ___bobberInBar, int ___bobberBarHeight, bool ___fadeOut, bool ___fadeIn)
        {
            try
            {
                if (___distanceFromCatching <= 0f)
                {
                    Cleanup();
                    return;
                }

                if (___fadeOut) return;
                if (___fadeIn) return;

                if (___difficulty > MainClass.Config.MaximumFishingDifficulty)
                {
                    Log.Debug($"Fish difficulty set to {MainClass.Config.MaximumFishingDifficulty} from {___difficulty}");
                    ___difficulty = MainClass.Config.MaximumFishingDifficulty;
                }

                if (___motionType != MainClass.Config.FixFishingMotionType &&
                        (MainClass.Config.FixFishingMotionType >= 0 && MainClass.Config.FixFishingMotionType <= 4))
                {
                    Log.Debug($"Motion type set to {MainClass.Config.FixFishingMotionType} from {___motionType}");
                    ___motionType = MainClass.Config.FixFishingMotionType;
                }

                if (Game1.soundBank == null) return;

                HandleProgressBarSound(___distanceFromCatching);

                HandleBobberBarCollisionSound(___bobberBarPos, ___bobberBarSpeed, ___bobberBarHeight);

                HandleBobberTargetSound(___bobberPosition, ___bobberBarPos, ___bobberInBar, ___bobberBarHeight);
            }
            catch (System.Exception e)
            {
                Log.Error($"An error occured while patching bobber bar:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void HandleBobberTargetSound(float ___bobberPosition, float ___bobberBarPos, bool ___bobberInBar, int ___bobberBarHeight)
        {
            bobberSound ??= Game1.soundBank.GetCue("SinWave");

            if (___bobberInBar)
            {
                if (bobberSound.IsPlaying)
                {
                    bobberSound.Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
                }
                return;
            }

            bool shouldPlay = false;

            if (___bobberPosition < ___bobberBarPos)
            {
                int distanceFromBobber = (int)(___bobberBarPos - ___bobberPosition + (___bobberBarHeight / 2));
                float calculatedPitch = 1200f + distanceFromBobber * 4;
                bobberSound.SetVariable("Pitch", calculatedPitch);
                shouldPlay = true;
            }

            if (___bobberPosition > ___bobberBarPos)
            {
                int distanceFromBobber = (int)(___bobberPosition - ___bobberBarPos - (___bobberBarHeight / 2));
                float calculatedPitch = 1200f - distanceFromBobber * 4;
                bobberSound.SetVariable("Pitch", calculatedPitch);
                shouldPlay = true;
            }

            if (shouldPlay && !bobberSound.IsPlaying)
            {
                bobberSound.Play();
            }

            if (!shouldPlay && bobberSound.IsPlaying)
            {
                bobberSound.Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
            }
        }

        private static void HandleProgressBarSound(float ___distanceFromCatching)
        {
            if (___distanceFromCatching > previousDistanceFromCatching)
            {
                if (___distanceFromCatching >= 0.2f && previousDistanceFromCatching < 0.2f)
                {
                    Game1.playSound("bobber_progress");
                }
                else if (___distanceFromCatching >= 0.4f && previousDistanceFromCatching < 0.4f)
                {
                    Game1.playSound("bobber_progress");
                }
                else if (___distanceFromCatching >= 0.6f && previousDistanceFromCatching < 0.6f)
                {
                    Game1.playSound("bobber_progress");
                }
                else if (___distanceFromCatching >= 0.8f && previousDistanceFromCatching < 0.8f)
                {
                    Game1.playSound("bobber_progress");
                }

                previousDistanceFromCatching = ___distanceFromCatching;
            }
            else if (___distanceFromCatching < previousDistanceFromCatching)
            {
                if (___distanceFromCatching <= 0.2f && previousDistanceFromCatching > 0.2f)
                {
                    Game1.playSound("bobber_progress", -100);
                }
                else if (___distanceFromCatching <= 0.4f && previousDistanceFromCatching > 0.4f)
                {
                    Game1.playSound("bobber_progress", -100);
                }
                else if (___distanceFromCatching <= 0.6f && previousDistanceFromCatching > 0.6f)
                {
                    Game1.playSound("bobber_progress", -100);
                }
                else if (___distanceFromCatching <= 0.8f && previousDistanceFromCatching > 0.8f)
                {
                    Game1.playSound("bobber_progress", -100);
                }

                previousDistanceFromCatching = ___distanceFromCatching;
            }
        }

        private static void HandleBobberBarCollisionSound(float ___bobberBarPos, float ___bobberBarSpeed, int ___bobberBarHeight)
        {
            float estimatedBobberBarPos = ___bobberBarPos + ___bobberBarSpeed;
            if (estimatedBobberBarPos + (float)___bobberBarHeight > 568f)
            {
                Log.Debug("Collided with bottom");
                Game1.playSound("axchop", -100);
            }
            else if (estimatedBobberBarPos < 0f)
            {
                Log.Debug("Collided with top");
                Game1.playSound("axchop", 1000);
            }
        }

        private static void Cleanup()
        {
            if (bobberSound != null && bobberSound.IsPlaying)
            {
                bobberSound.Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
            }

            previousDistanceFromCatching = 0f;
        }
    }
}
