using System;
using System.IO;
using System.Security;
using System.Text.Json;

namespace Task_2
{
    public class JsonReader : IJsonReader
    {
        private readonly string _settingsFilePath = "appsettings.json";

        public string ReadBaseUrl()
        {
            string json;
            try
            {
                json = File.ReadAllText(_settingsFilePath);
            }
            catch (Exception exception)
            {
                if (exception is FileNotFoundException ||
                         exception is UnauthorizedAccessException ||
                         exception is SecurityException ||
                         exception is IOException)
                {
                    throw new DataAccessException($"Unable to read settings from file {_settingsFilePath}", exception);
                }

                throw;
            }

            try
            {
                return JsonSerializer.Deserialize<BaseUrlContainer>(json).BaseUrl;
            }
            catch (Exception exception)
            {
                if (exception is ArgumentNullException ||
                    exception is JsonException)
                {
                    throw new DataAccessException($"File with settings {_settingsFilePath} is damaged", exception);
                }

                throw;
            }
        }
    }
}
