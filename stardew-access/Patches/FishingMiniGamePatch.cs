using StardewValley;
using StardewValley.Menus;

namespace stardew_access.Patches {
    internal class FishingMiniGamePatch {
        private static ICue? progressSound = null;

        internal static void BobberBarPatch(BobberBar __instance, float ___distanceFromCatching, float ___bobberPosition, float ___bobberBarPos, bool ___bobberInBar, bool ___fadeOut, bool ___fadeIn) {
            try {
                handleProgressBarSound(___distanceFromCatching, ___fadeOut, ___fadeIn);
                // MainClass.DebugLog($"dist: {___distanceFromCatching}\tbobPos: {___bobberPosition}\tbobbarpos{___bobberBarPos}");
            } catch (System.Exception e) {
                MainClass.ErrorLog($"An error occured while patching bobber bar:\n{e.Message}\n{e.StackTrace}");
            }
        }

        private static void handleProgressBarSound(float ___distanceFromCatching, bool ___fadeOut, bool ___fadeIn) {
            if(Game1.soundBank == null) return;

            if (progressSound == null) {
                progressSound = Game1.soundBank.GetCue("SinWave");
            }

            progressSound.SetVariable("Pitch", 2400f * ___distanceFromCatching);
            // progressSound.SetVariable("Volume", 300f);

            if (___fadeIn && !progressSound.IsPlaying) {
                // Start playing the sound on menu open
                progressSound.Play(); 
            }

            if(___fadeOut && progressSound.IsPlaying) {
                // Stop playing the sound on menu close
                progressSound.Stop(Microsoft.Xna.Framework.Audio.AudioStopOptions.Immediate);
            }
        }
    }
}
