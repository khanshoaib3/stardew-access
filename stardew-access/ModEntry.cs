using AccessibleOutput;
using stardew_access.Game;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;

namespace stardew_access
{
    /// <summary>The mod entry point.</summary>
    public class MainClass : Mod
    {
        public static IAccessibleOutput screenReader;
        Harmony harmony;
        public static IMonitor monitor;
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Inititalize monitor
            monitor = Monitor;

            // Initialize the screen reader
            initializeScreenReader();

            // Init harmony
            harmony = new Harmony(ModManifest.UniqueID);

            // Add patches
            harmony.Patch(
                original: AccessTools.Constructor(typeof(StardewValley.Menus.DialogueBox), new Type[] { typeof(Dialogue) }),
                postfix: new HarmonyMethod(typeof(MainClass), nameof(MainClass.Dialog_post))
            );

            harmony.Patch(
                original:,
                postfix:
            );


            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private static void Dialog_post(Dialogue dialogue)
        {
            try
            {
                string speakerName = dialogue.speaker.Name;
                List<string> dialogues = dialogue.dialogues;
                int dialogueIndex = dialogue.currentDialogueIndex;

                screenReader.Speak($"{speakerName} said, {dialogues[dialogueIndex]}", false);
                monitor.Log($"Dialogue", LogLevel.Info);
            }catch (Exception e)
            {
                monitor.Log($"Unable to narrate dialog:\n{e.StackTrace}", LogLevel.Error);
            }
        }

        private void initializeScreenReader()
        {
            NvdaOutput nvdaOutput = null;
            JawsOutput jawsOutput = null;
            SapiOutput sapiOutput = null;

            // Initialize NVDA
            try{
                nvdaOutput = new NvdaOutput();
            }catch(Exception ex){
                Monitor.Log($"Error initializing NVDA:\n{ex.StackTrace}", LogLevel.Error);
            }

            // Initialize JAWS
            try
            {
                jawsOutput = new JawsOutput();
            }catch (Exception ex){
                Monitor.Log($"Error initializing JAWS:\n{ex.StackTrace}", LogLevel.Error);
            }

            // Initialize SAPI
            try
            {
                sapiOutput = new SapiOutput();
            }catch (Exception ex){
                Monitor.Log($"Error initializing SAPI:\n{ex.StackTrace}", LogLevel.Error);
            }

            if (nvdaOutput != null && nvdaOutput.IsAvailable())
                screenReader = nvdaOutput;

            if(jawsOutput != null && jawsOutput.IsAvailable())
                screenReader = jawsOutput;

            if (sapiOutput != null && sapiOutput.IsAvailable())
                screenReader = sapiOutput;
        }


        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
              return;

            // Narrate Health And Energy
            if (Equals(e.Button, SButton.R))
            {
                screenReader.Speak($"Health is {CurrentPlayer.getHealth()} and Stamina is {CurrentPlayer.getStamina()}");
            }
        }

    }
}