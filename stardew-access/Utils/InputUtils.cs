namespace stardew_access.Utils;

using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;

public class InputUtils
{
    /// <summary>
    /// Checks if any movement key or thumbstick is currently active.
    /// </summary>
    /// <returns>True if any movement control is active, otherwise false.</returns>
    /// <remarks>
    /// This function checks for both gamepad thumbstick movement and keypresses configured for movement.
    /// </remarks>
    public static bool IsAnyMovementKeyPressed()
    {
        // For keyboard keybindings
        IEnumerable<InputButton[]> movementButtons = new[]
        {
            Game1.options.moveUpButton,
            Game1.options.moveRightButton,
            Game1.options.moveDownButton,
            Game1.options.moveLeftButton
        };

        foreach (var movementButton in movementButtons)
        {
            if (Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), movementButton))
                return true;
        }
        
        // For controller keybindings
        if (Game1.isGamePadThumbstickInMotion())
            return true;

        return false;
    }

    /// <summary>
    /// Checks whether the given SButton is any movement key.
    /// </summary>
    /// <param name="sButton">The SButton object to check.</param>
    /// <returns>true if sButton is one of the movement keys</returns>
    /// <remarks>This function checks for both gamepad thumbstick movement and keypresses configured for movement.</remarks>
    public static bool IsAnyMovementKey(SButton sButton)
    {
        // For keyboard keybindings
        IEnumerable<InputButton[]> movementButtons = new[]
        {
            Game1.options.moveUpButton,
            Game1.options.moveRightButton,
            Game1.options.moveDownButton,
            Game1.options.moveLeftButton
        };

        foreach (var movementButton in movementButtons)
        {
            foreach (var inputButton in movementButton)
            {
                if (sButton.Equals(inputButton.ToSButton()))
                {
                    return true;
                }
            }
        }
        
        // For controller keybindings
        if (sButton.Equals(SButton.LeftThumbstickUp))
            return true;
        if (sButton.Equals(SButton.LeftThumbstickRight))
            return true;
        if (sButton.Equals(SButton.LeftThumbstickDown))
            return true;
        if (sButton.Equals(SButton.LeftThumbstickLeft))
            return true;

        return false;
    }

    /// <summary>
    /// Checks if the 'Use Tool' key is currently active.
    /// </summary>
    /// <returns>True if the 'Use Tool' key is pressed, otherwise false.</returns>
    /// <remarks>
    /// This function checks for both gamepad button and keypresses configured for using a tool in the game.
    /// </remarks>
    public static bool IsUseToolKeyActive()
    {
        // For keyboard keybindings
        if (Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.useToolButton))
            return true;

        // For controller keybindings
        if (Game1.input.GetGamePadState().IsButtonDown(Buttons.X))
            return true;

        return false;
    }

    /// <summary>
    /// Checks if the 'Do Action' key is currently active.
    /// </summary>
    /// <returns>True if the 'Do Action' key is pressed, otherwise false.</returns>
    /// <remarks>
    /// This function checks for both gamepad button and keypresses configured for performing actions in the game.
    /// </remarks>
    public static bool IsDoActionKeyActive()
    {
        // For keyboard keybindings
        if (Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.actionButton))
            return true;

        // For controller keybindings
        if (Game1.input.GetGamePadState().IsButtonDown(Buttons.A))
            return true;

        return false;
    }
    
    /// <summary>
    /// Checks whether the given SButton is any of the inventory slot selection buttons or left/right trigger.
    /// </summary>
    /// <param name="sButton">The button to check.</param>
    /// <returns>true if the given button is one of the inventory slot buttons or left/right trigger.</returns>
    public static bool IsAnyInventorySlotButton(SButton sButton)
    {
        // For keyboard keybindings
        IEnumerable<InputButton[]> inventorySlotButtons = new[]
        {
            Game1.options.inventorySlot1,
            Game1.options.inventorySlot2,
            Game1.options.inventorySlot3,
            Game1.options.inventorySlot4,
            Game1.options.inventorySlot5,
            Game1.options.inventorySlot6,
            Game1.options.inventorySlot7,
            Game1.options.inventorySlot8,
            Game1.options.inventorySlot9,
            Game1.options.inventorySlot10,
            Game1.options.inventorySlot11,
            Game1.options.inventorySlot12,
        };
            
        foreach (var inventorySlotButton in inventorySlotButtons)
        {
            foreach (var inputButton in inventorySlotButton)
            {
                if (sButton.Equals(inputButton.ToSButton()))
                {
                    return true;
                }
            }
        }

        // For controller keybindings
        if (sButton.Equals(SButton.RightTrigger))
            return true;
        if (sButton.Equals(SButton.LeftTrigger))
            return true;
            
        return false;
    }

    /// <summary>
    /// Checks whether the given SButton is toolbar swap button or left/right shoulder button.
    /// </summary>
    /// <param name="sButton">The button to check.</param>
    /// <returns>true if the given button is toolbar swap button or left/right shoulder button.</returns>
    public static bool IsToolbarSwapButton(SButton sButton)
    {
        // For keyboard keybindings
        foreach (var inputButton in Game1.options.toolbarSwap)
        {
            if (sButton.Equals(inputButton.ToSButton()))
            {
                return true;
            }
        }

        // For controller keybindings
        if (sButton.Equals(SButton.RightShoulder))
            return true;
        if (sButton.Equals(SButton.LeftShoulder))
            return true;
            
        return false;
    }
}