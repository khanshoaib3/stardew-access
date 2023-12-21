using Microsoft.Xna.Framework;
using stardew_access.Translation;
using stardew_access.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace stardew_access.Tiles;

public abstract class ConditionalBase
{
    private readonly (Func<ConditionalBase, bool> func, string name)[] Conditions;
    internal readonly Dictionary<string, string> ConditionArgs = new();
    private readonly bool ModsLoaded;

    public bool Visible
    {
        get
        {
            // If mods aren't loaded, return false
            if (!ModsLoaded)
            {
                #if DEBUG
                Log.Verbose($"ConditionalBase: {this}: Invisible due to mods not loaded");
                #endif
                return false;
            }
            // If Conditions array is empty, return true
            if (Conditions.Length == 0)
            {
                return true;
            }

            // Iterate over each condition function and evaluate it
            foreach (var (func, name) in Conditions)
            {
                // If any condition returns false, the whole property should return false
                if (!func(this))
                {
                    #if DEBUG
                    Log.Verbose($"ConditionalBase: {this}: Invisible due to failing condition {name}");
                    #endif
                    return false;
                }
            }

            // If we made it this far, all conditions returned true
            return true;
        }
    }

    public ConditionalBase(
        string[]? conditions = null,
        string[]? withMods = null
    )
    {
        Conditions = BindConditions(conditions);
        ModsLoaded = AreModsLoaded(withMods);
    }

    private static bool AreModsLoaded(string[]? withMods)
    {
        return withMods == null || withMods.Length == 0 || withMods.All(modName => MainClass.ModHelper!.ModRegistry.IsLoaded(modName));
    }

    private (Func<ConditionalBase, bool> func, string name)[] BindConditions(string[]? conditions)
    {
        if (conditions == null || conditions.Length == 0) return Array.Empty<(Func<ConditionalBase, bool> func, string name)>();
        List<(Func<ConditionalBase, bool> func, string name)> conditionsList = new();
        foreach (string condition in conditions)
        {
            string[] parts = condition.Split(':');
            string functionName = parts[0];
            string? arg = parts.Length > 1 ? parts[1] : null;
            #if DEBUG
            Log.Verbose($"ConditionalBase: Attempting to bind condition name \"{functionName}\" with args \"{arg}\" to {this}");
            #endif
            
            if (AccessibleTileHelpers.TryGetConditionHelper(functionName, out var conditionFunc) && conditionFunc != null)
            {
                conditionsList.Add((conditionFunc, functionName));
                #if DEBUG
                Log.Verbose($"ConditionalBase: Found and added function");
                #endif
                if (arg != null)
                {
                    ConditionArgs[functionName] = arg;
                }
            } else {
                Log.Error($"Could not find a condition function named {functionName} for {this}.");
            }
        }
        return conditionsList.ToArray();
    }
}
