using Microsoft.Xna.Framework.Audio;
using StardewValley;

namespace stardew_access
{
    internal class CustomSoundEffects
    {
        internal enum TYPE
        {
            Sound,
            Footstep
        }

        internal static void Initialize()
        {
            try
            {
                if (MainClass.ModHelper == null)
                    return;

                Dictionary<String, TYPE> soundEffects = new()
                {
                    { "drop_item", TYPE.Sound },
                    { "colliding", TYPE.Sound },
                    { "invalid-selection", TYPE.Sound },

                    { "bobber_progress", TYPE.Sound },

                    { "npc_top", TYPE.Footstep },
                    { "npc_right", TYPE.Footstep },
                    { "npc_left", TYPE.Footstep },
                    { "npc_bottom", TYPE.Footstep },

                    { "obj_top", TYPE.Footstep },
                    { "obj_right", TYPE.Footstep },
                    { "obj_left", TYPE.Footstep },
                    { "obj_bottom", TYPE.Footstep },

                    { "npc_mono_top", TYPE.Footstep },
                    { "npc_mono_right", TYPE.Footstep },
                    { "npc_mono_left", TYPE.Footstep },
                    { "npc_mono_bottom", TYPE.Footstep },

                    { "obj_mono_top", TYPE.Footstep },
                    { "obj_mono_right", TYPE.Footstep },
                    { "obj_mono_left", TYPE.Footstep },
                    { "obj_mono_bottom", TYPE.Footstep }
                };

                for (int i = 0; i < soundEffects.Count; i++)
                {
                    KeyValuePair<String, TYPE> soundEffect = soundEffects.ElementAt(i);

                    CueDefinition cueDefinition = new()
                    {
                        name = soundEffect.Key
                    };

                    if (soundEffect.Value == TYPE.Sound)
                    {
                        cueDefinition.instanceLimit = 1;
                        cueDefinition.limitBehavior = CueDefinition.LimitBehavior.ReplaceOldest;
                    }

                    SoundEffect effect;
                    string filePath = Path.Combine(MainClass.ModHelper.DirectoryPath, "assets", "sounds", $"{soundEffect.Key}.wav");
                    using (FileStream stream = new(filePath, FileMode.Open))
                    {
                        effect = SoundEffect.FromStream(stream);
                    }

                    if (soundEffect.Value == TYPE.Sound)
                        cueDefinition.SetSound(effect, Game1.audioEngine.GetCategoryIndex("Sound"), false);
                    else if (soundEffect.Value == TYPE.Footstep)
                        cueDefinition.SetSound(effect, Game1.audioEngine.GetCategoryIndex("Footsteps"), false);

                    Game1.soundBank.AddCue(cueDefinition);
                }
            }
            catch (Exception e)
            {
                MainClass.ErrorLog($"Unable to initialize custom sounds:\n{e.Message}\n{e.StackTrace}");
            }
        }
    }
}
