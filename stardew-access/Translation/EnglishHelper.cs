using Shockah.ProjectFluent;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace stardew_access.Translation;

public class EnglishHelper : LanguageHelperBase
{
    private readonly HashSet<string> _irregularsSet;
    private readonly HashSet<string> _consonantOExceptions;
    private readonly Dictionary<string, HashSet<string>> _modifiedIrregulars;
    private static readonly Regex _splitRegex = new(@"\s+", RegexOptions.Compiled); 

    public EnglishHelper() : base("en")
    {
        #if DEBUG
        Log.Verbose("Initializing EnglishHelper");
        #endif
        this._irregularsSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        this._consonantOExceptions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        this._modifiedIrregulars = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        bool regionalIrregularsLoaded = PopulateHashSetFromJson("irregulars", _irregularsSet);
        if (regionalIrregularsLoaded)
            Log.Info($"Loaded {_irregularsSet.Count} regional irregular plural entries.");
        else
            Log.Debug("No regional irregular plural entries found.", true);

        bool regionalConsonantOExceptions = PopulateHashSetFromJson("consonant_o_exceptions", _consonantOExceptions);
        if (regionalConsonantOExceptions)
            Log.Info($"Loaded {_consonantOExceptions.Count} regional consonant-o exception plural entries.");
        else
            Log.Debug("No regional consonant-o exception plural entries found.", true);

        bool regionalModifiedIrregularsLoaded = PopulateDictionaryFromJson("modified_irregulars", _modifiedIrregulars);
        if (regionalModifiedIrregularsLoaded)
            Log.Info($"Loaded {_modifiedIrregulars.Count} regional modified irregular plural entries.");
        else
            Log.Debug("No regional modified irregular plural entries found.", true);

        #if DEBUG
        Log.Verbose("Exiting EnglishHelper constructor");
        #endif
    }

    private bool PopulateHashSetFromJson(string key, HashSet<string> set)
    {
        #if DEBUG
        Log.Verbose($"Entering PopulateHashSetFromJson with key \"{key}\"");
        #endif

        Log.Trace("Adding locale data");
        int count = set.Count;
        AddItemsToSet(LocaleData, key, set);

        Log.Trace($"Added {set.Count - count} locale entries.");

        Log.Trace("Adding regional locale data");
        count = set.Count;
        bool regionalDataLoaded = AddItemsToSet(LocaleRegionalData, key, set);
        #if DEBUG
        if (regionalDataLoaded)
        {
            Log.Trace($"Total number of items in the set after adding locale regional data: {set.Count - count}");
            Log.Trace($"Final total number of items in the set: {set.Count}");
        }
        Log.Verbose("Exiting PopulateHashSetFromJson");
        #endif
        return regionalDataLoaded;

        static bool AddItemsToSet(Dictionary<string, object?>? data, string key, HashSet<string> set)
        {
            #if DEBUG
            Log.Verbose($"Entering AddItemsToSet with key \"{key}\"");
            #endif

            if (data == null)
            {
                Log.Trace($"The data dictionary is null. Skipping adding items for key '{key}'.");
            }
            else if (!data.ContainsKey(key))
            {
                Log.Trace($"The key '{key}' is not found in the JSON file. Skipping.");
            }
            else if (data[key] is not List<object> list)
            {
                Log.Trace($"The '{key}' key in the JSON file does not contain a list. Skipping.");
            }
            else
            {
                foreach (var item in list)
                {
                    string? item_text = item?.ToString();
                    if (!string.IsNullOrEmpty(item_text))
                    {
                        set.Add(item_text);
                    }
                }
                return true;
            }

            #if DEBUG
            Log.Verbose("Exiting AddItemsToSet");
            #endif
            return false;
        }
    }

    private bool PopulateDictionaryFromJson(string key, Dictionary<string, HashSet<string>> dict)
    {
        #if DEBUG
        Log.Verbose($"Entering PopulateDictionaryFromJson with key \"{key}\"");
        #endif

        if (LocaleData?.ContainsKey(key) == true && LocaleData[key] is Dictionary<string, object> data)
        {
            foreach (var pair in data)
            {
                string modifier = pair.Key;
                if (pair.Value is List<object> words)
                {
                    var nonNullWords = words.ConvertAll(w => w?.ToString() ?? string.Empty).Where(w => !string.IsNullOrEmpty(w));
                    dict[modifier] = new HashSet<string>(nonNullWords, StringComparer.OrdinalIgnoreCase);

                    #if DEBUG
                    Log.Trace($"Added {dict[modifier].Count} items for the key \"{modifier}\".");
                    #endif
                }
            }
        }
        bool regionalDataLoaded = false;
        if (LocaleRegionalData?.ContainsKey(key) == true && LocaleRegionalData[key] is Dictionary<string, object> regionalData)
        {
            foreach (var pair in regionalData)
            {
                string modifier = pair.Key;
                if (pair.Value is List<object> words)
                {
                    var nonNullWords = words.ConvertAll(w => w?.ToString() ?? string.Empty).Where(w => !string.IsNullOrEmpty(w));
                    dict[modifier] = new HashSet<string>(nonNullWords, StringComparer.OrdinalIgnoreCase);
                    #if DEBUG
                    Log.Trace($"Added {dict[modifier].Count} items for the key \"{modifier}\".");
                    #endif
                }
            }
            regionalDataLoaded = true;
        }

        #if DEBUG
        Log.Verbose("Exiting PopulateDictionaryFromJson");
        #endif
        return regionalDataLoaded;
    }

    public override string Pluralize(int? count, string word, string? prefix = null)
    {
        #if DEBUG
        Log.Verbose($"In EnglishHelper.Pluralize with args: count = {count}, word = {word}, prefix = {(prefix ?? "null")}");
        #endif
        return                 HandleMultiWordString(count, word)
            ?? HandleIrregularPluralization(count, word, prefix)
            ?? HandleModifiedIrregularPluralization(count, word, prefix)
            ?? HandleStandardPluralization(count, word, prefix)
            ?? "";
    }

    private string? HandleMultiWordString(int? count, string word)
    {
        #if DEBUG
        Log.Verbose($"Entering HandleMultiWordString; count = {count}, word = {word}.");
        #endif
        var words = _splitRegex.Split(word);
        if (words.Length > 1)
        {
            string prefix = string.Join(" ", words, 0, words.Length - 1);
            string lastWord = words[^1];
            string pluralizedWord = Pluralize(count, lastWord, prefix);
            #if DEBUG
            Log.Verbose($"Exiting HandleMultiWordString; multiple words; prefix is \"{prefix}\"; lastWord is \"{lastWord}\"; pluralizing  \"{word}\" to \"{pluralizedWord}\".");
            #endif
            return pluralizedWord;
        }
        #if DEBUG
        Log.Verbose("Exiting HandleMultiWordString; not multiple words.");
        #endif
        return null;
    }

    private string? HandleIrregularPluralization(int? count, string word, string? prefix = null)
    {
        #if DEBUG
        Log.Verbose($"Entering HandleIrregularPluralization; count = {count}, word = \"{word}\", prefix = \"{prefix}\"");
        #endif
        if (_irregularsSet.Contains(word))
        {
            prefix = !string.IsNullOrEmpty(prefix) ? $"{prefix.Trim()} " : "";
            #if DEBUG
            Log.Verbose($"Exiting HandleIrregularPluralization; word \"{word}\" is irregular.");
            #endif
            return $"{prefix}{word}";
        }
        #if DEBUG
        Log.Verbose($"Exiting HandleIrregularPluralization; word \"{word}\" is not irregular.");
        #endif
        return null;
    }

    private string? HandleModifiedIrregularPluralization(int? count, string word, string? prefix = null)
    {
        #if DEBUG
        Log.Verbose($"Entering HandleModifiedIrregularPluralization; count = {count}, word = \"{word}\", prefix = \"{prefix}\"");
        #endif

        (string, string)? GetModifiedIrregularsComponents()
        {
            foreach (var pair in _modifiedIrregulars)
            {
                string modifier = pair.Key;
                HashSet<string> words = pair.Value;
                if (words.Contains(word))
                {
                    return (modifier, word);
                }
            }
            return null;
        }

        var components = GetModifiedIrregularsComponents();
        if (components != null)
        {
            string modifier = components.Value.Item1;
            string pluralModifier = (count ?? 0) == 1 ? modifier : Pluralize(count, modifier) ?? throw new InvalidOperationException("An error occurred while pluralizing the modifier.");
            prefix = !string.IsNullOrEmpty(prefix) ? $"{prefix.Trim()} " : "";
            #if DEBUG
            Log.Verbose($"Exiting HandleModifiedIrregularPluralization; count = {count}, pluralModifier = \"{pluralModifier}\", prefix = \"{prefix}\", pluralized word = \"{components.Value.Item2}\"");
            #endif
            return $"{pluralModifier} of {prefix} {components.Value.Item2}";
        }

        #if DEBUG
        Log.Verbose($"Exiting HandleModifiedIrregularPluralization; \"{word}\" is not a modified irregular.");
        #endif
        return null;
    }

    private string HandleStandardPluralization(int? count, string word, string? prefix = null)
    {
        #if DEBUG
        Log.Verbose($"Entering HandleStandardPluralization; count = {count}, word = \"{word}\", prefix = \"{prefix}\"");
        #endif

        if (count == 1)
        {
            #if DEBUG
            Log.Verbose($"Exiting HandleStandardPluralization; count is 1, returning original word \"{word}\"");
            #endif
            return $"{prefix}{word}";
        }

        char lastLetter = word[^1];
        char secondToLastLetter = word.Length > 1 ? word[^2] : '\0';
        bool isSecondToLastVowel = "aeiou".Contains(secondToLastLetter);

        // Trim and append space to prefix if not empty
        prefix = !string.IsNullOrEmpty(prefix) ? $"{prefix.Trim()} " : "";

        string modifiedWord;

        switch (lastLetter)
        {
            case 's':
            case 'x':
            case 'z':
                // Add "es" for words ending with s, x, z
                #if DEBUG
                Log.Verbose($"\"{word}\" ends with s, x or z; adding -es");
                #endif
                modifiedWord = $"{word}es";
                break;
            case 'h':
                if (secondToLastLetter == 'c' || secondToLastLetter == 's')
                {
                    #if DEBUG
                    Log.Verbose($"\"{word}\" ends with ch or sh; adding -es");
                    #endif
                    // Add "es" for words ending with ch, sh
                    modifiedWord = $"{word}es";
                    break;
                }
                goto default;
            case 'y':
                if (!isSecondToLastVowel)
                {
                    // Replace "y" with "ies" if preceded by a consonant
                    #if DEBUG
                    Log.Verbose($"\"{word}\" ends with y preceded by a consonant; replacing -y with -ies");
                    #endif
                    modifiedWord = $"{word[..^1]}ies";
                    break;
                }
                goto default;
            case 'f':
                // Replace "f" with "ves"
                #if DEBUG
                Log.Verbose($"\"{word}\" ends with f; replacing with -ves");
                #endif
                modifiedWord = $"{word[..^1]}ves";
                break;
            case 'e':
                if (secondToLastLetter == 'f')
                {
                    // Replace "fe" with "ves"
                    #if DEBUG
                    Log.Verbose($"\"{word}\" ends with fe; replacing with -ves");
                    #endif
                    modifiedWord = $"{word[..^2]}ves";
                    break;
                }
                goto default;
            case 'o':
                if (!isSecondToLastVowel && !_consonantOExceptions.Contains(word))
                {
                    // Add "es" for words ending with o preceded by a consonant, except for exceptions
                    #if DEBUG
                    Log.Verbose($"\"{word}\" ends with o preceded by a consonant, and is not a known exception; adding -es");
                    #endif
                    modifiedWord = $"{word}es";
                    break;
                }
                goto default;
            default:
                // Default case: just add "s"
                #if DEBUG
                Log.Verbose($"\"{word}\" has no special rule; adding -s");
                #endif
                modifiedWord = $"{word}s";
                break;
        }

        #if DEBUG
        Log.Verbose($"Exiting HandleStandardPluralization; modified word = \"{modifiedWord}\"");
        #endif

        return $"{prefix}{modifiedWord}";
    }

    // Other English-specific methods can be added here in the future
}
