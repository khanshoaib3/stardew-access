using System.IO;
using System.Text.Json;

namespace stardew_access.Utils
{
    public static class JsonLoader
    {
        private static string GetFilePath(string fileName)
        {
            return Path.Combine(MainClass.ModHelper!.DirectoryPath, "assets", fileName);
        }

        /// <summary>
        /// Loads a JSON file from the specified file name in the assets folder.
        /// </summary>
        /// <param name="fileName">The name of the JSON file to load.</param>
        /// <returns>A <see cref="JsonElement"/> containing the deserialized JSON data, or default if an error occurs.</returns>
        public static JsonElement LoadJsonFile(string fileName)
        {
            string filePath = GetFilePath(fileName);

            try
            {
                string json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<JsonElement>(json);
            }
            catch (FileNotFoundException ex)
            {
                MainClass.ErrorLog($"{fileName} file not found: {ex.Message}");
            }
            catch (JsonException ex)
            {
                MainClass.ErrorLog($"Error parsing {fileName}: {ex.Message}");
            }
            catch (Exception ex)
            {
                MainClass.ErrorLog($"An error occurred while initializing {fileName}: {ex.Message}");
            }

            return default;
        }

        /// <summary>
        /// Saves a JSON file to the specified file name in the assets folder.
        /// </summary>
        /// <param name="fileName">The name of the JSON file to save.</param>
        /// <param name="data">The data to save.</param>
        /// <returns>A boolean indicating whether the operation was successful.</returns>
        public static bool SaveJsonFile(string fileName, JsonElement data)
        {
            string filePath = GetFilePath(fileName);

            try
            {
                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                return true;
            }
            catch (IOException ex)
            {
                MainClass.ErrorLog($"Error writing to {fileName}: {ex.Message}");
            }
            catch (JsonException ex)
            {
                MainClass.ErrorLog($"Error serializing data for {fileName}: {ex.Message}");
            }
            catch (Exception ex)
            {
                MainClass.ErrorLog($"An error occurred while saving {fileName}: {ex.Message}");
            }

            return false;
        }
    }
}