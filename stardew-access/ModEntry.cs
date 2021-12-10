using stardew_access.Game;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using stardew_access.Patches;

namespace stardew_access
{

    public class MainClass : Mod
    {
        private Harmony? harmony;
        public static IMonitor? monitor;

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
            ScreenReader.initializeScreenReader();

            // Init harmony
            harmony = new Harmony(ModManifest.UniqueID);

            // Add patches
            harmony.Patch(
               original: AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.draw), new Type[] {typeof(SpriteBatch)}),
               postfix: new HarmonyMethod(typeof(DialoguePatch), nameof(DialoguePatch.CharachterDialoguePatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.drawHoverText), new Type[] { typeof(SpriteBatch), typeof(string), typeof(SpriteFont), typeof(int), typeof(int), typeof(int) , typeof(string) , typeof(int) , typeof(string[]) , typeof(Item) , typeof(int) , typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe) , typeof(IList < Item >) }),
                postfix: new HarmonyMethod(typeof(DialoguePatch), nameof(DialoguePatch.HoverTextPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(TitleMenu), nameof(TitleMenu.draw) , new Type[] {typeof(SpriteBatch)}),
                postfix: new HarmonyMethod(typeof(MenuPatch), nameof(MenuPatch.TitleMenuPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(LoadGameMenu.SaveFileSlot), nameof(LoadGameMenu.SaveFileSlot.Draw), new Type[] { typeof(SpriteBatch), typeof(int) }),
                postfix: new HarmonyMethod(typeof(MenuPatch), nameof(MenuPatch.LoadGameMenuPatch))
            );

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
              return;

            // Narrate Health And Energy
            if (Equals(e.Button, SButton.I))
            {
                string toSpeak = $"Health is {CurrentPlayer.getHealth()} and Stamina is {CurrentPlayer.getStamina()}";
                ScreenReader.say(toSpeak, true);
            }

            // Narrate Position
            if (Equals(e.Button, SButton.K))
            {
                string toSpeak = $"X: {CurrentPlayer.getPositionX()} , Y: {CurrentPlayer.getPositionX()}";
                ScreenReader.say(toSpeak, true);
            }
        }

    }
}