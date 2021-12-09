using AccessibleOutput;
using stardew_access.Game;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using HarmonyLib;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using StardewValley.BellsAndWhistles;
using Microsoft.Xna.Framework;

namespace stardew_access
{
    /// <summary>The mod entry point.</summary>
    public class MainClass : Mod
    {
        public static IAccessibleOutput screenReader;
        Harmony harmony;
        public static IMonitor monitor;
        private static string prevText = "";
        private static DialogueBox? dialogueBox = null;
        private int index = 0;
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
               original: AccessTools.Method(typeof(DialogueBox), nameof(DialogueBox.draw), new Type[] {typeof(SpriteBatch)}),
               postfix: new HarmonyMethod(typeof(MainClass), nameof(MainClass.DialoguePatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(IClickableMenu), nameof(IClickableMenu.drawHoverText), new Type[] { typeof(SpriteBatch), typeof(string), typeof(SpriteFont), typeof(int), typeof(int), typeof(int) , typeof(string) , typeof(int) , typeof(string[]) , typeof(Item) , typeof(int) , typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe) , typeof(IList < Item >) }),
                postfix: new HarmonyMethod(typeof(MainClass), nameof(MainClass.HoverTextPatch))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(TitleMenu), nameof(TitleMenu.draw) , new Type[] {typeof(SpriteBatch)}),
                postfix: new HarmonyMethod(typeof(MainClass), nameof(TitleMenuPatch))
            );

            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            //helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
        }

        private static void TitleMenuPatch(TitleMenu __instance, SpriteBatch b)
        {
            try
            {
                __instance.allClickableComponents.ForEach(component =>
                {
                    if(component.containsPoint(Game1.getMousePosition(true).X, Game1.getMousePosition(true).Y))
                    {
                        string name = component.name;
                        string label = component.label;
                        string toSpeak = $"{name} {label}";

                        if(prevText != toSpeak)
                        {
                            prevText = toSpeak;
                            screenReader.Speak(toSpeak, true);
                        }
                    }
                });
            }
            catch (Exception)
            {
            }
        }

        private static void DialoguePatch(DialogueBox __instance, SpriteBatch b)
        {
            try
            {
                Dialogue dialogue = __instance.characterDialogue;
                string speakerName = dialogue.speaker.Name;
                List<string> dialogues = dialogue.dialogues;
                int dialogueIndex = dialogue.currentDialogueIndex;
                monitor.Log("" + dialogue.isCurrentStringContinuedOnNextScreen, LogLevel.Debug);

                if (prevText != $"{speakerName} said, {dialogues[dialogueIndex]}")
                {
                    prevText = $"{speakerName} said, {dialogues[dialogueIndex]}";
                    screenReader.Speak($"{speakerName} said, {dialogues[dialogueIndex]}", false);
                }
            }
            catch (Exception e)
            {
                monitor.Log($"Unable to narrate dialog:\n{e.StackTrace}", LogLevel.Error);
            }

        }

        private void OnOneSecondUpdateTicked(object sender, OneSecondUpdateTickedEventArgs e)
        {
            if (Game1.activeClickableMenu != null)
            {
                string name = Game1.activeClickableMenu.getCurrentlySnappedComponent().name;
                string label = Game1.activeClickableMenu.getCurrentlySnappedComponent().label;
                string toSpeeak = $"{name} {label}";

                if (prevText != toSpeeak)
                {
                    prevText = toSpeeak;
                    screenReader.Speak(toSpeeak, true);
                }
            }
        }

        private static void HoverTextPatch(SpriteBatch b, string text, SpriteFont font, int xOffset = 0, int yOffset = 0, int moneyAmountToDisplayAtBottom = -1, string boldTitleText = null, int healAmountToDisplay = -1, string[] buffIconsToDisplay = null, Item hoveredItem = null, int currencySymbol = 0, int extraItemToShowIndex = -1, int extraItemToShowAmount = -1, int overrideX = -1, int overrideY = -1, float alpha = 1f, CraftingRecipe craftingIngredients = null, IList<Item> additional_craft_materials = null)
        {
            try
            {
                StringBuilder toSpeak = new StringBuilder();

                #region Add title if any
                if (boldTitleText != null)
                    toSpeak.Append($"{boldTitleText}.\n");
                #endregion

                #region Add the base text
                toSpeak.Append(text); 
                #endregion

                #region Add crafting ingredients
                if (craftingIngredients != null)
                {

                    toSpeak.Append($"\n{craftingIngredients.description}");
                    toSpeak.Append("\nIngredients\n");

                    craftingIngredients.recipeList.ToList().ForEach(recipe =>
                    {
                        int count = recipe.Value;
                        int item = recipe.Key;
                        string name = craftingIngredients.getNameFromIndex(item);

                        toSpeak.Append($" ,{count} {name}");
                    });
                } 
                #endregion

                #region Add health & stamina
                if (hoveredItem is StardewValley.Object && (hoveredItem as StardewValley.Object).Edibility != -300)
                {
                    int stamina_recovery = (hoveredItem as StardewValley.Object).staminaRecoveredOnConsumption();
                    toSpeak.Append($"{stamina_recovery} Energy\n");
                    if (stamina_recovery >= 0)
                    {
                        int health_recovery = (hoveredItem as StardewValley.Object).healthRecoveredOnConsumption();
                        toSpeak.Append($"{health_recovery} Health");
                    }
                }
                #endregion

                #region Add buff items (effects like +1 walking speed)
                if (buffIconsToDisplay != null)
                {
                    for (int i = 0; i < buffIconsToDisplay.Length; i++)
                    {
                        string buffName = ((Convert.ToInt32(buffIconsToDisplay[i]) > 0) ? "+" : "") + buffIconsToDisplay[i] + " ";
                        if (i <= 11)
                        {
                            buffName = Game1.content.LoadString("Strings\\UI:ItemHover_Buff" + i, buffName);
                        }
                        try
                        {
                            int count = int.Parse(buffName.Substring(0, buffName.IndexOf(' ')));
                            monitor.Log("" + count);
                            if (count != 0)
                                toSpeak.Append($"{buffName}\n");
                        }
                        catch (Exception) { }
                    }
                }
                #endregion

                #region Add money
                if (moneyAmountToDisplayAtBottom != -1)
                    toSpeak.Append($"\nValue: {moneyAmountToDisplayAtBottom} coins\n");
                #endregion

                #region Narrate toSpeak
                if (prevText != toSpeak.ToString())
                {
                    prevText = toSpeak.ToString();
                    screenReader.Speak(toSpeak.ToString(), true);
                } 
                #endregion
            }
            catch (Exception e)
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
            if (Equals(e.Button, SButton.K))
            {
                if (Game1.activeClickableMenu != null)
                {
                    string name = Game1.activeClickableMenu.getCurrentlySnappedComponent().label;
                    screenReader.Speak(name, true);
                }
            }


            // ignore if player hasn't loaded a save yet
            if (!Context.IsWorldReady)
              return;

            // Narrate Health And Energy
            if (Equals(e.Button, SButton.I))
            {
                screenReader.Speak($"Health is {CurrentPlayer.getHealth()} and Stamina is {CurrentPlayer.getStamina()}");
            }
        }

    }
}