using System.Text.Json;

namespace stardew_access.Utils
{
    public static class JsonLoader
    {
        public delegate void NestedItemProcessor<TKey, TValue>(List<string> path, JsonElement element, ref Dictionary<TKey, TValue> res) where TKey : notnull;

        private const string DefaultDir = "assets";
        private static string GetFilePath(string fileName, string subdir)
        {
            return Path.Combine(MainClass.ModHelper!.DirectoryPath, subdir, fileName);
        }

        /// <summary>
        /// Loads a JSON file from the specified file name in the specified location (defaults to `DefaultDir`).
        /// </summary>
        /// <param name="fileName">The name of the JSON file to load.</param>
        /// <param name="subdir">The name of the stardew-access subdirectory to look in. Defaults to DefaultDir.</param>
        /// <returns>A <see cref="JsonDocument"/> containing the deserialized JSON data, or default if an error occurs.</returns>
        public static bool TryLoadJsonFile(string fileName, out JsonDocument document, string subdir = DefaultDir)
        {
            string filePath = GetFilePath(fileName, subdir);

            try
            {
                string json = File.ReadAllText(filePath);
                document = JsonDocument.Parse(json);
                return true;
            }         
            catch (FileNotFoundException ex)
            {
                Log.Error($"{fileName} file not found: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Log.Error($"Error parsing {fileName}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred while initializing {fileName}: {ex.Message}");
            }

            #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            document = default;
            #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            return false;
        }

        public static bool TryLoadJsonFile(string fileName, out JsonElement root, string subdir = DefaultDir)
        {
            bool loaded = TryLoadJsonFile(fileName, out JsonDocument document, subdir);
            if (loaded && document != null)
            {
                root = document.RootElement;
                return true;
            }
            root = default;
            return false;
        }

        public static bool TryLoadJsonArray<T>(string fileName, out T[] result, string subdir = DefaultDir)
        {
            bool loaded = TryLoadJsonFile(fileName, out JsonDocument document, subdir);

            if (loaded && document != null)
            {
                result = JsonSerializer.Deserialize<T[]>(document.RootElement.GetRawText()) ?? Array.Empty<T>();
                return true;
            }

            #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            result = default;
            #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            return false;
        }

        public static bool TryLoadJsonDictionary<T>(string fileName, out Dictionary<int, T> result, string subdir = DefaultDir)
        {
            bool loaded = TryLoadJsonFile(fileName, out JsonDocument document, subdir);
            if (loaded && document != null)
            {
                var tempResult = JsonSerializer.Deserialize<Dictionary<string, T>>(document.RootElement.GetRawText()) ?? new Dictionary<string, T>();
                result = tempResult.ToDictionary(kvp => int.Parse(kvp.Key), kvp => kvp.Value);
                return true;
            }

            #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            result = default;
            #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            return false;
        }

        public static bool TryLoadJsonDictionary<T>(string fileName, out Dictionary<string, T> result, string subdir = DefaultDir)
        {
            bool loaded = TryLoadJsonFile(fileName, out JsonDocument document, subdir);

            if (loaded && document != null)
            {
                result = JsonSerializer.Deserialize<Dictionary<string, T>>(document.RootElement.GetRawText()) ?? new Dictionary<string, T>();
                return true;
            }

            #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            result = default;
            #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            return false;
        }

        public static bool TryLoadNestedJson<TKey, TValue>(
            string fileName,
            NestedItemProcessor<TKey, TValue> nestedItemProcessor,
            out Dictionary<TKey, TValue> result,
            int nestingLevel,
            string subdir = DefaultDir
        ) where TKey : notnull
        {
            Log.Verbose($"[TryLoadNestedJson] Starting to load {fileName} with nesting level {nestingLevel}");

            result = new Dictionary<TKey, TValue>();

            if (TryLoadJsonFile(fileName, out JsonDocument document, subdir) && document != null)
            {
                #if DEBUG
                Log.Verbose("[TryLoadNestedJson] Successfully loaded JSON file. Starting to process root element.");
                #endif

                ProcessJsonElement(new List<string>(), document.RootElement, nestingLevel, result);

                #if DEBUG
                Log.Verbose("[TryLoadNestedJson] Finished processing root element.");
                #endif
                Log.Trace($"[TryLoadNestedJson] Loaded {result.Count} entries from {fileName}.");
                return true;
            }
            Log.Warn($"[TryLoadNestedJson] Failed to load or parse {fileName}", true);
            return false;

            void ProcessJsonElement(List<string> path, JsonElement element, int remainingLevels, Dictionary<TKey, TValue> res)
            {
                 #if DEBUG
                Log.Verbose($"[ProcessJsonElement] Processing element at path: {string.Join(" -> ", path)} with remaining levels: {remainingLevels}");
                #endif

                if (remainingLevels == 0)
                {
                    nestedItemProcessor(path, element, ref res);
                    #if DEBUG
                    Log.Verbose("[ProcessJsonElement] Processed element and populated result dictionary.");
                    #endif
                }
                else
                {
                    foreach (var child in element.EnumerateObject())
                    {
                        var newPath = new List<string>(path) { child.Name };
                        ProcessJsonElement(newPath, child.Value, remainingLevels - 1, res);
                    }
                    #if DEBUG
                    Log.Verbose("[ProcessJsonElement] Completed iteration over child elements.");
                    #endif
                }
            }
        }

        public static bool TryLoadJsonAsArray(string fileName, out List<object?> result, string subdir = DefaultDir)
        {
            bool loaded = TryLoadJsonFile(fileName, out JsonDocument? document, subdir);
            if (!loaded || document == null)
            {
                result = new List<object?>();
                return false;
            }

            result = ParseArray(document.RootElement);
            return true;
        }

        public static bool TryLoadJsonAsDictionary(string fileName, out Dictionary<string, object?>? result, string subdir = DefaultDir)
        {
            bool loaded = TryLoadJsonFile(fileName, out JsonDocument document, subdir);
            if (document == null)
            {
                result = null;
                return false;
            }

            result = ParseObject(document.RootElement);
            return true;
        }

        private static List<object?> ParseArray(JsonElement arrayElement)
        {
            List<object?> list = new();
            foreach (JsonElement element in arrayElement.EnumerateArray())
            {
                list.Add(ParseElement(element));
            }
            return list;
        }

        private static Dictionary<string, object?> ParseObject(JsonElement objectElement)
        {
            Dictionary<string, object?> dict = new();
            foreach (JsonProperty property in objectElement.EnumerateObject())
            {
                dict[property.Name] = ParseElement(property.Value);
            }
            return dict;
        }

        private static object? ParseElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out long longValue))
                    {
                        return longValue;
                    }
                    else
                    {
                        return element.GetDouble();
                    }
                case JsonValueKind.String:
                    return element.GetString();
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean();
                case JsonValueKind.Array:
                    return ParseArray(element);
                case JsonValueKind.Object:
                    return ParseObject(element);
                case JsonValueKind.Null:
                    return null;
                default:
                    throw new NotSupportedException($"Unsupported JSON value kind: {element.ValueKind}");
            }
        }

        /// <summary>
        /// Saves a JSON file to the specified file name in the specified location (defaults to `DefaultDir`).
        /// </summary>
        /// <param name="fileName">The name of the JSON file to save.</param>
        /// <param name="data">The data to save.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        public static bool SaveJsonFile(string fileName, JsonElement data, string subdir = DefaultDir)
        {
            string filePath = GetFilePath(fileName, subdir);

            try
            {
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                return true;
            }
            catch (IOException ex)
            {
                Log.Error($"Error writing to {fileName}: {ex.Message}");
            }
            catch (JsonException ex)
            {
                Log.Error($"Error serializing data for {fileName}: {ex.Message}");
            }
            catch (Exception ex)
            {
                Log.Error($"An error occurred while saving {fileName}: {ex.Message}");
            }

            return false;
        }

        public static bool SaveJsonFile(string fileName, JsonDocument document, string subdir = DefaultDir)
        {
            return SaveJsonFile(fileName, document.RootElement, subdir);
        }
    }
}