using Shockah.ProjectFluent;
using StardewModdingAPI;
using System.Collections.Generic;

namespace stardew_access.Translation
{
    public interface ILanguageHelper
    {
        IFluentFunctionValue Pluralize(
            IGameLocale locale,
            IManifest mod,
            IReadOnlyList<IFluentFunctionValue> positionalArguments,
            IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments
        );
        
        string Pluralize(int? count, string word, string? prefix = null);
        
        (int?, string) GetCacheKey(int? count, string word);

        bool InPluralizationCache((int? count, string word) key, out string? cached);
    }
}
