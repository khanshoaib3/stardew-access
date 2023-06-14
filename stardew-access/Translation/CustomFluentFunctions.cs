using Shockah.ProjectFluent;
using StardewModdingAPI;

namespace stardew_access
{
    internal class CustomFluentFunctions
    {
        private IManifest ModManifest { get; set; }
        private IFluentApi FluentApi { get; set; }

        internal CustomFluentFunctions(
            IManifest ModManifest,
            IFluentApi FluentApi
        )
        {
            this.ModManifest = ModManifest;
            this.FluentApi = FluentApi;
        }

        internal IEnumerable<(IManifest mod, string name, FluentFunction function)> GetAll()
        {
            yield return (ModManifest, "TEST", EE);
        }

        private IFluentFunctionValue EE(
            IGameLocale locale,
            IManifest mod,
            IReadOnlyList<IFluentFunctionValue> positionalArguments,
            IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments
        )
        {
            MainClass.DebugLog("HERE");
            if (positionalArguments.Count > 0)
                MainClass.DebugLog($"Value: {positionalArguments[0].ToString()}");
            return FluentApi.CreateIntValue(-1);
        }
    }
}
