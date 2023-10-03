using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;
using System.Reflection;

namespace stardew_access.Utils
{
    /// <summary>
    /// Provides utility functions for various miscellaneous tasks.
    /// </summary>
    /// <remarks>
    /// This class houses a collection of static utility functions that perform a variety of tasks not specifically tied to one aspect of the mod.
    /// </remarks>
    public static class MiscUtils
    {
        public delegate void OnMismatchAction();

        /// <summary>
        /// Cycles through a list in either forward or backward direction.
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="list">The list to cycle through.</param>
        /// <param name="index">The current index, will be updated based on the cycling direction.</param>
        /// <param name="back">Set to true for backward cycling, false for forward.</param>
        /// <param name="wrapAround">Determines if cycling should wrap around when reaching the beginning or end of the list. If null, the list does not wrap.</param>
        /// <returns>A tuple containing the current item and a boolean indicating if the edge of the list was reached.</returns>
        /// <exception cref="ArgumentException">Thrown when the list is empty.</exception>
        /// <remarks>
        /// This function is ideal for navigating through collections in a UI or game setting, especially when you want to loop back to the start or end.
        /// The function updates the index passed in by reference and also returns whether the edge of the list was reached.
        /// </remarks>
        public static (T, bool) Cycle<T>(IList<T> list, ref int index, bool back = false, bool? wrapAround = false)
        {
            if (list.Count == 0)
            {
                throw new ArgumentException("List cannot be empty.", nameof(list));
            }

            bool edgeOfList = false;

            if (back)
            {
                index--;
                if (index < 0)
                {
                    if (wrapAround == true) index = list.Count - 1;
                    else index = 0;
                    edgeOfList = true;
                }
            }
            else
            {
                index++;
                if (index >= list.Count)
                {
                    if (wrapAround == true) index = 0;
                    else index = list.Count - 1;
                    edgeOfList = true;
                }
            }

            return (list[index], edgeOfList);
        }

        /// <summary>
        /// Checks if a given type inherits from a target type.
        /// </summary>
        /// <param name="typeToCheck">The type to check for inheritance.</param>
        /// <param name="targetType">The target type to check against.</param>
        /// <returns>True if <paramref name="typeToCheck"/> inherits from <paramref name="targetType"/>, otherwise false.</returns>
        /// <remarks>
        /// This function traverses up the inheritance chain to check for inheritance.
        /// </remarks>
        public static bool InheritsFrom(Type typeToCheck, Type targetType)
        {
            while (typeToCheck != null && typeToCheck != typeof(object))
            {
                if (typeToCheck.BaseType == targetType)
                    return true;
                #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                typeToCheck = typeToCheck.BaseType;
                #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            }
            return false;
        }

        /// <summary>
        /// Checks if a value has changed and runs an action if so.
        /// </summary>
        /// <param name="storedValue">The stored value to compare.</param>
        /// <param name="currentValue">The current value to compare against.</param>
        /// <param name="action">The action to run if the values are mismatched.</param>
        /// <remarks>
        /// This function updates the stored value and runs the provided action if the stored value and current value are different.
        /// </remarks>
        public static void UpdateAndRunIfChanged(ref int storedValue, int currentValue, OnMismatchAction action)
        {
            if (storedValue != currentValue)
            {
                storedValue = currentValue;
                action();
            }
        }

        /// <summary>
        /// Checks if any movement key or thumbstick is currently active.
        /// </summary>
        /// <returns>True if any movement control is active, otherwise false.</returns>
        /// <remarks>
        /// This function checks for both gamepad thumbstick movement and keypresses configured for movement.
        /// </remarks>
        public static bool IsAnyMovementKeyActive()
        {
            if (Game1.isGamePadThumbstickInMotion())
                return true;

            List<InputButton[]> keys = new()
            {
                Game1.options.moveUpButton,
                Game1.options.moveRightButton,
                Game1.options.moveDownButton,
                Game1.options.moveLeftButton
            };

            foreach (InputButton[] ibs in keys)
            {
                if (Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), ibs))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if the 'Use Tool' key is currently active.
        /// </summary>
        /// <returns>True if the 'Use Tool' key is pressed, otherwise false.</returns>
        /// <remarks>
        /// This function checks for keypresses configured for using a tool in the game.
        /// </remarks>
        public static bool IsUseToolKeyActive()
        {
            if (Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.useToolButton))
                return true;

            return false;
        }
    }
}
