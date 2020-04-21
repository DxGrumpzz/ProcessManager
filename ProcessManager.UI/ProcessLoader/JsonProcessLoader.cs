namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Windows;

    /// <summary>
    /// A json process file loader
    /// </summary>
    public class JsonProcessLoader : IProcessLoader
    {
        /// <summary>
        /// Retrieves an <see cref="IEnumerable{T}"/> of ProcessModel from a file
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProcessModel> GetProcessListFromFile(string processListFile)
        {
            // Read json data from file 
            var json = File.ReadAllText(processListFile);

            try
            {
                // Parse json data
                return JsonSerializer.Deserialize<IEnumerable<ProcessModel>>(json);
            }
            // If parsing failed return null
            catch(JsonException jsonException)
            {
                MessageBox.Show($"Unable to parse {processListFile}.\nError: {jsonException.Message}", "Error");
                throw;
            };
        }
    };
};
