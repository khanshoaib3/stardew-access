using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches
{
    internal class FishingMiniGamePatch
    {
        private static ICue? bobberSound = null;

        internal static void BobberBarPatch(BobberBar __instance, ref float ___difficulty, ref int ___motionType, float ___distanceFromCatching, float ___bobberPosition, float ___bobberBarPos, bool ___bobberInBar, int ___bobberBarHeight, bool ___fadeOut, bool ___fadeIn)
        {
            try
            {
                if (___distanceFromCatching <= 0f)
                {
                    cleanup();
                    return;
                }

                if (___fadeOut) return;
                if (___fadeIn) return;

                if (___difficulty > MainClass.Config.MaximumFishingDifficulty)
                {
                    MainClass.DebugLog($"Fish difficulty set to {MainClass.Config.MaximumFishingDifficulty} from {___difficulty}");
                    ___difficulty = MainClass.Config.MaximumFishingDifficulty;
                }

                if (___motionType != MainClass.Config.FixFishingMotionType &&
                        (MainClass.Config.FixFishingMotionType >= 0 && MainClass.Config.FixFishingMotionType <= 4))
                {
                    MainClass.DebugLog($"Motion type set to {MainClass.Config.FixFishingMotionType} from {___motionType}");
                    ___motionType = MainClass.Config.FixFishingMotionType;
                }

                if (Game1.soundBank == null) return;

                // handleProgressBarSound(___distanceFromCatching);

                handleBobberTargetSound(___bobberPosition, ___bobberBarPos, ___bobberInBar, ___bobberBarHeight);
            }
            catch (System.Exception e)
            {
                MainClass.ErrorLog($"An error occured while patching bobber bar:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void handleBobberTargetSound(float bobberPosition, float bobberBarPos, bool bobberInBar, int ___bobberBarHeight)
        {
            if (bobberSound == null)
            {
                bobberSound = Game1.soundBank.GetCue("SinWave");
            }

            if (bobberInBar)
            {
                if (bobberSound.IsPlaying)
                {
                    bobberSound.Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
                }
                return;
            }

            bool shouldPlay = false;

            if (bobberPosition < bobberBarPos)
            {
                int distanceFromBobber = (int)(bobberBarPos - bobberPosition + (___bobberBarHeight / 2));
                float calculatedPitch = 1200f + distanceFromBobber * 4;
                bobberSound.SetVariable("Pitch", calculatedPitch);
                shouldPlay = true;
            }

            if (bobberPosition > bobberBarPos)
            {
                int distanceFromBobber = (int)(bobberPosition - bobberBarPos - (___bobberBarHeight / 2));
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

        private static void handleProgressBarSound(float ___distanceFromCatching)
        {
        }

        private static void cleanup()
        {
            if (bobberSound != null && bobberSound.IsPlaying)
            {
                bobberSound.Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
            }
        }
    }
}
