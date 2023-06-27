using Shockah.ProjectFluent;
using StardewModdingAPI;

namespace stardew_access
{
    internal class Translator
    {
        private static Translator? instance = null;
        private IFluent<string>? Fluent { get; set; }
        private static readonly object instanceLock = new   ();
        private Translator() { }

        public static Translator Instance
        {
            get
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
            IFluentApi? fluentApi = MainClass.ModHelper?.ModRegistry.GetApi<IFluentApi>("Shockah.ProjectFluent");

            if (fluentApi != null)
            {
                Fluent = fluentApi.GetLocalizationsForCurrentLocale(modManifest);
                foreach ( var (mod, name, function) in new CustomFluentFunctions(modManifest, fluentApi).GetAll())
                {
                    fluentApi.RegisterFunction( mod, name, function);
                }
            }
            else
            {
                MainClass.ErrorLog("Unable to initialize fluent api");
            }
        }

        public string Translate(string translationKey, bool disableWarning = false)
        {
            if (Fluent == null)
            {
                MainClass.ErrorLog("Fluent not initialized!");
                return translationKey;
            }

            if (Fluent.ContainsKey(translationKey))
            {
                return Fluent.Get(translationKey);
            }

            if (!disableWarning)
            {
                MainClass.DebugLog($"No translation available for key: {translationKey}");
            }

            return translationKey;
        }

        public string Translate(string translationKey, object? tokens, bool disableWarning = false)
        {
            if (Fluent == null)
            {
                MainClass.ErrorLog("Fluent not initialized!");
                return translationKey;
            }

            if (Fluent.ContainsKey(translationKey))
            {
                return Fluent.Get(translationKey, tokens);
            }

            if (!disableWarning)
            {
                MainClass.DebugLog($"No translation available for key: {translationKey}");
            }

            return translationKey;
        }
    }
}
