using Shockah.ProjectFluent;
using StardewModdingAPI;

namespace stardew_access.Translation
{
    internal class Translator
    {
        private static Translator? instance = null;
        private IFluent<string>? Fluent { get; set; }
        private static readonly object instanceLock = new   ();
        private Translator() { }
        internal CustomFluentFunctions? CustomFunctions;

        public static Translator Instance
        {                                               get
            {
                lock (instanceLock)
                {
                    instance ??= new Translator();
                    return instance;
                }
            }
        }

        public void Initialize(IManifest modManifest)
        {
            #if DEBUG
            Log.Debug("Initializing FluentApi");
            #endif
            IFluentApi? fluentApi = MainClass.ModHelper?.ModRegistry.GetApi<IFluentApi>("Shockah.ProjectFluent");
            if (fluentApi != null)
            {
                Fluent = fluentApi.GetLocalizationsForCurrentLocale(modManifest);
                #if DEBUG
                Log.Verbose("Registering custom fluent functions");
                #endif
                CustomFunctions = new CustomFluentFunctions(modManifest, fluentApi);
                foreach ( var (mod, name, function) in CustomFunctions.GetAll())
                {
                    fluentApi.RegisterFunction( mod, name, function);
                    #if DEBUG
                    Log.Verbose($"Registered function \"{name}\"");
                    #endif
                }
            }
            else
            {
                Log.Error("Unable to initialize fluent api", true);
            }
        }

        public string Translate(string translationKey, bool disableWarning = false)
        {
            if (Fluent == null)
            {
                Log.Error("Fluent not initialized!", true);
                return translationKey;
            }

            if (Fluent.ContainsKey(translationKey))
            {
                #if DEBUG
                Log.Verbose($"Translate: found translation key \"{translationKey}\"");
                #endif
                return Fluent.Get(translationKey);
            }

            if (!disableWarning)
            {
                Log.Debug($"No translation available for key: {translationKey}", true);
            }

            return translationKey;
        }

        public string Translate(string translationKey, object? tokens, bool disableWarning = false)
        {
            if (Fluent == null)
            {
                Log.Error("Fluent not initialized!", true);
                return translationKey;
            }

            if (Fluent.ContainsKey(translationKey))
            {
                #if DEBUG
                if (tokens is Dictionary<string, object> dictTokens)
                {
                    Log.Verbose($"Translate with tokens: found translation key \"{translationKey}\" with tokens: {string.Join(", ", dictTokens.Select(kv => $"{kv.Key}: {kv.Value}"))}");
                }
                else
                {
                    var tokenStr = tokens is not null ? string.Join(", ", tokens.GetType().GetProperties().Select(prop => $"{prop.Name}: {prop.GetValue(tokens)}")) : "null";
                    Log.Verbose($"Translate with tokens: found translation key \"{translationKey}\" with tokens: {tokenStr}");
                }
                #endif
                var result = Fluent.Get(translationKey, tokens);
                #if DEBUG
                Log.Verbose($"Translated to: {result}");
                #endif
                return result;
            }

            if (!disableWarning)
            {
                Log.Debug($"No translation available for key: {translationKey}", true);
            }

            return translationKey;
        }
    }
}
