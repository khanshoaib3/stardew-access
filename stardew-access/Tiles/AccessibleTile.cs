using Microsoft.Xna.Framework;
using stardew_access.Translation;
using stardew_access.Utils;
using System;
using System.Collections.Generic;

namespace stardew_access.Tiles
{
    public class AccessibleTile
    {
        private readonly string? StaticNameOrTranslationKey;
        private readonly Func<object?, string>? DynamicNameOrTranslationKey;
        public string NameOrTranslationKey
        {
            get
            {
                if (DynamicNameOrTranslationKey != null)
                {
                    return DynamicNameOrTranslationKey(this);
                }
                else if (StaticNameOrTranslationKey != null)
                {
                    return StaticNameOrTranslationKey;
                }
                else
                {
                    throw new InvalidOperationException("Neither static nor dynamic name is set.");
                }
            }
        }

        private readonly Vector2 StaticCoordinates;
        private readonly Func<object?, Vector2>? DynamicCoordinates;
        public Vector2 Coordinates
        {
            get
            {
                if (StaticCoordinates != Vector2.Zero)
                {
                    return StaticCoordinates;
                }
                else if (DynamicCoordinates != null)
                {
                    return DynamicCoordinates(this);
                }
                else
                {
                    throw new InvalidOperationException("Neither static nor dynamic coordinates are set.");
                }
            }
        }

        public readonly CATEGORY Category;
        
        private readonly Func<object?, bool>[] Conditions;
        public bool Visible
        {
            get
            {
                // If Conditions array is empty, return true
                if (Conditions.Length == 0)
                {
                    return true;
                }

                // Iterate over each condition function and evaluate it
                foreach (var condition in Conditions)
                {
                    // If any condition returns false, the whole property should return false
                    if (!condition(this))
                    {
                        return false;
                    }
                }

                // If we made it this far, all conditions returned true
                return true;
            }
        }

        public readonly bool IsEvent ;

        // Super constructor
        public AccessibleTile(
            string? staticNameOrTranslationKey = null,
            Func<object?, string>? dynamicNameOrTranslationKey = null,
            Vector2? staticCoordinates = null,
            Func<object?, Vector2>? dynamicCoordinates = null,
            CATEGORY? category = null,
            Func<object?, bool>[]? conditions = null,
            bool isEvent = false
        )
        {
            // Error handling for invalid combinations
            if (staticNameOrTranslationKey == null && dynamicNameOrTranslationKey == null)
            {
                throw new ArgumentException("At least one of static or dynamic name must be provided.");
            }

            if (!(staticCoordinates == null ^ dynamicCoordinates == null))
            {
                throw new ArgumentException("Exactly one of static or dynamic coordinates must be provided.");
            }

            // Set properties
            StaticNameOrTranslationKey = staticNameOrTranslationKey;
            DynamicNameOrTranslationKey = dynamicNameOrTranslationKey;
            StaticCoordinates = staticCoordinates ?? Vector2.Zero;
            DynamicCoordinates = dynamicCoordinates;
            Category = category ?? CATEGORY.Others;
            Conditions = conditions ?? Array.Empty<Func<object?, bool>>();
            IsEvent = isEvent;
        }

        public AccessibleTile(
            string? staticNameOrTranslationKey,
            Vector2? staticCoordinates,
            CATEGORY? category = null,
            Func<object?, bool>[]? conditions = null,
            bool isEvent = false
        ) : this (staticNameOrTranslationKey, null, staticCoordinates, null, category, conditions, isEvent)
        {
        }

        public AccessibleTile(
            string? staticNameOrTranslationKey,
            Func<object?, Vector2>? dynamicCoordinates,
            CATEGORY? category = null,
            Func<object?, bool>[]? conditions = null,
            bool isEvent = false
        ) : this (staticNameOrTranslationKey, null, null, dynamicCoordinates, category, conditions, isEvent)
        {
        }

        public AccessibleTile(
            Func<object?, string>? dynamicNameOrTranslationKey,
            Vector2? staticCoordinates,
            CATEGORY? category = null,
            Func<object?, bool>[]? conditions = null,
            bool isEvent = false
        ) : this (null, dynamicNameOrTranslationKey, staticCoordinates, null, category, conditions, isEvent)
        {
        }

        public AccessibleTile(
            Func<object?, string>? dynamicNameOrTranslationKey,
            Func<object?, Vector2>? dynamicCoordinates,
            CATEGORY? category = null,
            Func<object?, bool>[]? conditions = null,
            bool isEvent = false
        ) : this (null, dynamicNameOrTranslationKey, null, dynamicCoordinates, category, conditions, isEvent)
        {
        }

        public AccessibleTile(
            string? staticNameOrTranslationKey,
            Func<object?, string>? dynamicNameOrTranslationKey,
            Vector2? staticCoordinates,
            CATEGORY? category = null,
            Func<object?, bool>[]? conditions = null,
            bool isEvent = false
        ) : this (staticNameOrTranslationKey, dynamicNameOrTranslationKey, staticCoordinates, null, category, conditions, isEvent)
        {
        }

        public AccessibleTile(
            string? staticNameOrTranslationKey,
            Func<object?, string>? dynamicNameOrTranslationKey,
            Func<object?, Vector2>? dynamicCoordinates,
            CATEGORY? category = null,
            Func<object?, bool>[]? conditions = null,
            bool isEvent = false
        ) : this (staticNameOrTranslationKey, dynamicNameOrTranslationKey, null, dynamicCoordinates, category, conditions, isEvent)
        {
        }

        public (string nameOrTranslationKey, CATEGORY category) NameAndCategory
        {
            get
            {
                return (
                    Translator.Instance.Translate(
                        NameOrTranslationKey, 
                        translationCategory: TranslationCategory.StaticTiles,
                        disableWarning: true
                    ),
                    Category
                );
            }
        }


    }
}
