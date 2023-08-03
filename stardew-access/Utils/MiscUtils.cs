using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;

namespace stardew_access.Utils
{
    public static class MiscUtils
    {
        public delegate void OnMismatchAction();
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

        public static void UpdateAndRunIfChanged(ref int storedValue, int currentValue, OnMismatchAction action)
        {
            if (storedValue != currentValue)
            {
                storedValue = currentValue;
                action();
            }
        }

        
        /// <summary>
        /// Checks whether any player movement key is pressed be it from a keyboard or a gamepad/controller.
        /// </summary>
        /// <returns>true if a key is pressed otherwise false.</returns>
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

        public static bool IsUseToolKeyActive()
        {
            if (Game1.isOneOfTheseKeysDown(Game1.input.GetKeyboardState(), Game1.options.useToolButton))
                return true;

            return false;
        }
    }
}
