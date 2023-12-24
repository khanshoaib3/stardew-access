using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace stardew_access.Utils;

public static class JsonLoader
{
	private const string DefaultDir = "assets";
	public delegate void NestedItemProcessor<TKey, TValue>(List<string> path, JToken token, ref Dictionary<TKey, TValue> res) where TKey : notnull;
	public delegate void NestedItemProcessorWithUserFile<TKey, TValue>(List<string> path, JToken token, ref Dictionary<TKey, TValue> res, bool isUserFile) where TKey : notnull;

	/// <summary>
	/// Gets the full file path for a given file name and subdirectory within the mod's directory.
	/// </summary>
	/// <param name="fileName">The name of the file to retrieve the path for.</param>
	/// <param name="subdir">The subdirectory within the mod's directory, defaults to the default directory.</param>
	/// <returns>The full file path.</returns>
	public static string GetFilePath(string fileName, string subdir)
	{
		return Path.Combine(MainClass.ModHelper!.DirectoryPath, subdir, fileName);
	}

	/// <summary>
	/// Attempts to load a JSON file into a JToken object.
	/// </summary>
	/// <param name="fileName">The name of the JSON file to load.</param>
	/// <param name="jsonToken">An out parameter that will receive the loaded JToken if successful.</param>
	/// <param name="subdir">The subdirectory within the mod's directory, defaults to the default directory.</param>
	/// <returns>True if the file was loaded successfully, false otherwise.</returns>
	/// <remarks>
	/// Swallows FileNotFoundException and logs a trace message, as the caller may decide if this is an error.
	/// Logs errors for other exceptions.
	/// </remarks>
	public static bool TryLoadJsonFile(string fileName, out JToken? jsonToken, string subdir = DefaultDir)
	{
		string filePath = GetFilePath(fileName, subdir);

		try
		{
            using StreamReader reader = File.OpenText(filePath);
            using JsonTextReader jsonReader = new(reader);
            jsonToken = JToken.Load(jsonReader);
            return true;
        }
		catch (FileNotFoundException ex)
		{
			// Caller should decide if this is an error.
			Log.Trace($"{fileName} file not found: {ex.Message}");
		}
		catch (JsonReaderException ex)
		{
			Log.Error($"Error parsing {fileName}: {ex.Message}");
		}
		catch (Exception ex)
		{
			Log.Error($"An error occurred while initializing {fileName}: {ex.Message}");
		}

		jsonToken = null;
		return false;
	}

	/// <summary>
	/// Attempts to load a JSON file into a generic array.
	/// </summary>
	/// <typeparam name="T">The type of elements in the array.</typeparam>
	/// <param name="fileName">The name of the JSON file to load.</param>
	/// <param name="result">An out parameter that will receive the loaded array if successful.</param>
	/// <param name="subdir">The subdirectory within the mod's directory, defaults to the default directory.</param>
	/// <returns>True if the file was loaded successfully and parsed as an array, false otherwise.</returns>
	public static bool TryLoadJsonArray<T>(string fileName, out T[] result, string subdir = DefaultDir)
	{
		bool loaded = TryLoadJsonFile(fileName, out JToken? token, subdir);

		if (loaded && token != null)
		{
			result = token.ToObject<T[]>() ?? [];
			return true;
		}

		#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
		result = default;
		#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        return false;
	}

	/// <summary>
	/// Attempts to load a JSON file into a generic dictionary with integer keys.
	/// </summary>
	/// <typeparam name="T">The type of values in the dictionary.</typeparam>
	/// <param name="fileName">The name of the JSON file to load.</param>
	/// <param name="result">An out parameter that will receive the loaded dictionary if successful.</param>
	/// <param name="subdir">The subdirectory within the mod's directory, defaults to the default directory.</param>
	/// <returns>True if the file was loaded successfully and parsed as a dictionary with integer keys, false otherwise.</returns>
	/// <remarks>
	/// Assumes the keys in the JSON file are strings representing integers.
	/// </remarks>
	public static bool TryLoadJsonDictionary<T>(string fileName, out Dictionary<int, T> result, string subdir = DefaultDir)
	{
		bool loaded = TryLoadJsonFile(fileName, out JToken? token, subdir);
		if (loaded && token != null)
		{
			var tempResult = token.ToObject<Dictionary<string, T>>() ?? [];
			result = tempResult.ToDictionary(kvp => int.Parse(kvp.Key), kvp => kvp.Value);
			return true;
		}

		#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        result = default;
		#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        return false;
	}

	/// <summary>
	/// Attempts to load a JSON file into a generic dictionary with string keys.
	/// </summary>
	/// <typeparam name="T">The type of values in the dictionary.</typeparam>
	/// <param name="fileName">The name of the JSON file to load.</param>
	/// <param name="result">An out parameter that will receive the loaded dictionary if successful.</param>
	/// <param name="subdir">The subdirectory within the mod's directory, defaults to the default directory.</param>
	/// <returns>True if the file was loaded successfully and parsed as a dictionary, false otherwise.</returns>
	public static bool TryLoadJsonDictionary<T>(string fileName, out Dictionary<string, T> result, string subdir = DefaultDir)
	{
		bool loaded = TryLoadJsonFile(fileName, out JToken? token, subdir);

		if (loaded && token != null)
		{
			result = token.ToObject<Dictionary<string, T>>() ?? [];
			return true;
		}

		#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        result = default;
		#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        return false;
	}

	/// <summary>
	/// Attempts to load a nested JSON file into a dictionary, processing items at a specified nesting level.
	/// </summary>
	/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
	/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
	/// <param name="fileName">The name of the JSON file to load.</param>
	/// <param name="nestedItemProcessor">A delegate to process items at the specified nesting level.</param>
	/// <param name="result">An out parameter that will receive the loaded dictionary if successful.</param>
	/// <param name="nestingLevel">The nesting level at which to process items.</param>
	/// <param name="subdir">The subdirectory within the mod's directory, defaults to the default directory.</param>
	/// <returns>True if the file was loaded and processed successfully, false otherwise.</returns>
	public static bool TryLoadNestedJson<TKey, TValue>(
		string fileName,
		NestedItemProcessor<TKey, TValue> nestedItemProcessor,
		ref Dictionary<TKey, TValue> result,
		int nestingLevel,
		string subdir = DefaultDir
	) where TKey : notnull
	{
		Log.Verbose($"[TryLoadNestedJson] Starting to load {fileName} with nesting level {nestingLevel}");

		if (TryLoadJsonFile(fileName, out JToken? document, subdir) && document != null)
		{
			// Debug info...
			int processed = 0;
			ProcessJToken([], document, nestingLevel, result, ref processed);
			Log.Trace($"TryLoadNestedJson: Loaded {processed} entries from {fileName}.");

			return true;
		}
		Log.Warn($"[TryLoadNestedJson] Failed to load or parse {fileName}");
		return false;

		/// <summary>
		/// Recursively processes JTokens within a nested JSON structure.
		/// </summary>
		/// <param name="path">The current path within the JSON structure.</param>
		/// <param name="token">The JToken to process.</param>
		/// <param name="remainingLevels">The number of nesting levels remaining.</param>
		/// <param name="res">The dictionary to add processed items to.</param>
		/// <param name="processed">An out parameter to track the number of processed items.</param>
		void ProcessJToken(List<string> path, JToken token, int remainingLevels, Dictionary<TKey, TValue> res, ref int processed)
		{
			// Debug info...

			if (remainingLevels == 0)
			{
				nestedItemProcessor(path, token, ref res);
				processed++;
				// Debug info...
			}
			else
			{
				if (token.Type == JTokenType.Array)
				{
					int index = 0;
					foreach (var arrayItem in token.Children())
					{
						var newPath = new List<string>(path) { index.ToString() };
						ProcessJToken(newPath, arrayItem, remainingLevels - 1, res, ref processed);
						index++;
					}
				}
				else if (token.Type == JTokenType.Object)
				{
					foreach (var property in ((JObject)token).Properties())
					{
						var newPath = new List<string>(path) { property.Name };
						ProcessJToken(newPath, property.Value, remainingLevels - 1, res, ref processed);
					}
				}
				// Debug info...
			}
		}
	}

	/// <summary>
	/// Attempts to load a nested JSON file and an optional user-specific JSON file into a dictionary.
	/// </summary>
	/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
	/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
	/// <param name="fileName">The name of the base JSON file to load.</param>
	/// <param name="nestedItemProcessorWithUserFile">A delegate to process items from both base and user files.</param>
	/// <param name="result">An out parameter that will receive the loaded dictionary if successful.</param>
	/// <param name="nestingLevel">The nesting level at which to process items.</param>
	/// <param name="subdir">The subdirectory within the mod's directory, defaults to the default directory.</param>
	/// <param name="caseInsensitive">Whether to perform case-insensitive comparisons for keys.</param>
	/// <returns>True if either the base or user file was loaded and processed successfully, false otherwise.</returns>
	public static bool TryLoadNestedJsonWithUserFile<TKey, TValue>(
		string fileName,
		NestedItemProcessorWithUserFile<TKey, TValue> nestedItemProcessorWithUserFile,
		ref Dictionary<TKey, TValue> result,
		int nestingLevel,
		string subdir = DefaultDir,
		bool caseInsensitive = false
	) where TKey : notnull
	{
		// First, load the base file
		if (TryLoadNestedJson(
			fileName,
			(List<string> path, JToken token, ref Dictionary<TKey, TValue> res) => nestedItemProcessorWithUserFile(path, token, ref res, false),
			ref result,
			nestingLevel,
			subdir
		))
		{
			// Successfully loaded base file
			// Modify the file name to check for a user file
			string userFileName = $"{System.IO.Path.GetFileNameWithoutExtension(fileName)}_user{System.IO.Path.GetExtension(fileName)}";

			// Try to load the user file.
			if (TryLoadNestedJson(
				userFileName,
				(List<string> path, JToken token, ref Dictionary<TKey, TValue> res) => nestedItemProcessorWithUserFile(path, token, ref res, true),
				ref result,
				nestingLevel,
				subdir
			))
			{
				Log.Info($"Loaded {fileName}");
			}
			return true;
		} 
		return false;
	}

	/// <summary>
	/// Attempts to load a JSON file as an array of objects.
	/// </summary>
	/// <param name="fileName">The name of the JSON file to load.</param>
	/// <param name="result">An out parameter that will receive the loaded array if successful.</param>
	/// <param name="subdir">The subdirectory within the mod's directory, defaults to the default directory.</param>
	/// <returns>True if the file was loaded and parsed as an array, false otherwise.</returns>
	public static bool TryLoadJsonAsArray(string fileName, out List<object?> result, string subdir = DefaultDir)
	{
		bool loaded = TryLoadJsonFile(fileName, out JToken? token, subdir);
		if (!loaded || token == null)
		{
			result = [];
			return false;
		}

		result = ParseArray(token);
		return true;
	}

	/// <summary>
	/// Attempts to load a JSON file as a dictionary of string keys and objects.
	/// </summary>
	/// <param name="fileName">The name of the JSON file to load.</param>
	/// <param name="result">An out parameter that will receive the loaded dictionary if successful.</param>
	/// <param name="subdir">The subdirectory within the mod's directory, defaults to the default directory.</param>
	/// <returns>True if the file was loaded and parsed as a dictionary, false otherwise.</returns>
	public static bool TryLoadJsonAsDictionary(string fileName, out Dictionary<string, object?>? result, string subdir = DefaultDir)
	{
		bool loaded = TryLoadJsonFile(fileName, out JToken? token, subdir);
		if (token == null)
		{
			result = null;
			return false;
		}

		result = ParseObject(token);
		return true;
	}

	/// <summary>
	/// Parses a JToken representing an array into a list of objects.
	/// </summary>
	/// <param name="arrayToken">The JToken representing the array.</param>
	/// <returns>A list of parsed objects.</returns>
	private static List<object?> ParseArray(JToken arrayToken)
	{
		List<object?> list = [];
		foreach (JToken token in arrayToken.Children())
		{
			list.Add(ParseToken(token));
		}
		return list;
	}

	/// <summary>
	/// Parses a JToken representing an object into a dictionary of string keys and objects.
	/// </summary>
	/// <param name="objectToken">The JToken representing the object.</param>
	/// <returns>A dictionary of parsed key-value pairs.</returns>
	private static Dictionary<string, object?> ParseObject(JToken objectToken)
	{
		Dictionary<string, object?> dict = [];
		foreach (JProperty property in objectToken.Children<JProperty>())
		{
			dict[property.Name] = ParseToken(property.Value);
		}
		return dict;
	}

	/// <summary>
	/// Parses a JToken into its corresponding object value.
	/// </summary>
	/// <param name="token">The JToken to parse.</param>
	/// <returns>The parsed object value.</returns>
	/// <exception cref="NotSupportedException">Thrown if the token type is not supported.</exception>
	private static object? ParseToken(JToken token)
	{
        return token.Type switch
        {
            JTokenType.Integer => token.Value<long>(),
            JTokenType.Float => token.Value<double>(),
            JTokenType.String => token.Value<string>(),
            JTokenType.Boolean => token.Value<bool>(),
            JTokenType.Array => ParseArray(token),
            JTokenType.Object => ParseObject(token),
            JTokenType.Null => null,
            _ => throw new NotSupportedException($"Unsupported JSON token type: {token.Type}"),
        };
    }

	/// <summary>
	/// Saves a JToken to a JSON file.
	/// </summary>
	/// <param name="fileName">The name of the JSON file to save.</param>
	/// <param name="data">The JToken to save.</param>
	/// <param name="subdir">The subdirectory within the mod's directory, defaults to the default directory.</param>
	/// <returns>True if the file was saved successfully, false otherwise.</returns>
	public static bool SaveJsonFile(string fileName, JToken data, string subdir = DefaultDir)
	{
		string filePath = GetFilePath(fileName, subdir);

		try
		{
			string json = JsonConvert.SerializeObject(data, Formatting.Indented);
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

	/// <summary>
	/// Saves an object to a JSON file by converting it to a JToken.
	/// </summary>
	/// <param name="fileName">The name of the JSON file to save.</param>
	/// <param name="data">The object to save.</param>
	/// <param name="subdir">The subdirectory within the mod's directory, defaults to the default directory.</param>
	/// <returns>True if the file was saved successfully, false otherwise.</returns>
	public static bool SaveJsonFile(string fileName, object data, string subdir = DefaultDir)
	{
		return SaveJsonFile(fileName, JToken.FromObject(data), subdir);
	}
}
