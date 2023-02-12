using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches {
    internal class FishingMiniGamePatch {
        private static ICue? progressSound = null;
        private static long previousBobberTargetUpPlayedTime = 0;
        private static long previousBobberTargetDownPlayedTime = 0;

        internal static void BobberBarPatch(BobberBar __instance, ref float ___difficulty, ref int ___motionType, float ___distanceFromCatching, float ___bobberPosition, float ___bobberBarPos, bool ___bobberInBar, int ___bobberBarHeight, bool ___fadeOut, bool ___fadeIn) {
            try {
                if (___difficulty > MainClass.Config.MaximumFishingDifficulty) {
                    MainClass.DebugLog($"Fish difficulty set to {MainClass.Config.MaximumFishingDifficulty} from {___difficulty}");
                    ___difficulty = MainClass.Config.MaximumFishingDifficulty;
                }

                if (___motionType != MainClass.Config.FixFishingMotionType &&
                        (MainClass.Config.FixFishingMotionType >= 0 && MainClass.Config.FixFishingMotionType <= 4)) {
                    MainClass.DebugLog($"Motion type set to {MainClass.Config.FixFishingMotionType} from {___motionType}");
                    ___motionType = MainClass.Config.FixFishingMotionType;
                }

                handleProgressBarSound(___distanceFromCatching, ___fadeOut, ___fadeIn);

                handleBobberTargetSound(___bobberPosition, ___bobberBarPos, ___bobberInBar, ___bobberBarHeight, ___fadeOut, ___fadeIn);
            } catch (System.Exception e) {
                MainClass.ErrorLog($"An error occured while patching bobber bar:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void handleBobberTargetSound(float bobberPosition, float bobberBarPos, bool bobberInBar, int ___bobberBarHeight, bool ___fadeOut, bool ___fadeIn) {
            if (bobberInBar) return;
            if (___fadeIn) return;
            if (___fadeOut) return;

            DateTimeOffset now = (DateTimeOffset)DateTime.UtcNow;
            long currentTimeInMilliseconds = now.ToUnixTimeMilliseconds();

            if(bobberPosition < bobberBarPos && (currentTimeInMilliseconds - previousBobberTargetUpPlayedTime) >= 250) {
                previousBobberTargetUpPlayedTime = currentTimeInMilliseconds;
                int distanceFromBobber = (int)(bobberBarPos - bobberPosition + (___bobberBarHeight / 2));
                int calculatedPitch = distanceFromBobber * 4;
                MainClass.DebugLog(calculatedPitch.ToString());
                Game1.playSoundPitched("bobber_target_up", calculatedPitch);
            }

            if(bobberPosition > bobberBarPos && (currentTimeInMilliseconds - previousBobberTargetDownPlayedTime) >= 250) {
                previousBobberTargetDownPlayedTime = currentTimeInMilliseconds;
                int distanceFromBobber = (int)(bobberPosition - bobberBarPos - (___bobberBarHeight / 2));
                int calculatedPitch = distanceFromBobber * 4;
                MainClass.DebugLog(calculatedPitch.ToString());
                Game1.playSoundPitched("bobber_target_down", calculatedPitch);
            }
        }

        private static void handleProgressBarSound(float ___distanceFromCatching, bool ___fadeOut, bool ___fadeIn) {
            if (Game1.soundBank == null) return;

            if (progressSound == null) {
                progressSound = Game1.soundBank.GetCue("SinWave");
            }

            progressSound.SetVariable("Pitch", 2400f * ___distanceFromCatching);
            // progressSound.SetVariable("Volume", 300f);

            if (___fadeIn && !progressSound.IsPlaying) {
                // Start playing the sound on menu open
                progressSound.Play(); 
            }

            if (___fadeOut && progressSound.IsPlaying) {
                // Stop playing the sound on menu close
                progressSound.Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
            }
        }
    }
}
