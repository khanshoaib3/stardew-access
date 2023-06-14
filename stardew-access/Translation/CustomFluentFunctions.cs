using Shockah.ProjectFluent;
using StardewModdingAPI;

namespace stardew_access
{
    internal class CustomFluentFunctions
    {
        private IManifest ModManifest { get; set; }
        private IFluentApi FluentApi { get; set; }

        internal CustomFluentFunctions(IManifest ModManifest, IFluentApi FluentApi)
        {
            this.ModManifest = ModManifest;
            this.FluentApi = FluentApi;
        }

        internal IEnumerable<(IManifest mod, string name, FluentFunction function)> GetAll()
        {
            yield return (ModManifest, "TEST", EE);
            yield return (ModManifest, "EMPTYSTRING", EmptyString);
        }

        /// <summary>
        /// Returns an empty string which can be used in places where a character is required.
        /// <para>
        /// For example: {$value ->
        ///     [0] {EMPTYSTRING()}
        ///     *[other] {$value}
        /// }
        /// In the above example, if the value given is zero,
        /// it will print nothing and it will print any other value just fine.
        /// </para>
        /// </summary>
        /// <returns>An empty string</returns>
        private IFluentFunctionValue EmptyString(
            IGameLocale locale,
            IManifest mod,
            IReadOnlyList<IFluentFunctionValue> positionalArguments,
            IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments
        )
        {
            return FluentApi.CreateStringValue("");
        }

        private IFluentFunctionValue EE(
            IGameLocale locale,
            IManifest mod,
            IReadOnlyList<IFluentFunctionValue> positionalArguments,
            IReadOnlyDictionary<string, IFluentFunctionValue> namedArguments
        )
        {
            return FluentApi.CreateIntValue(-1);
        }
    }
}
