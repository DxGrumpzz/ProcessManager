namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Windows;

    /// <summary>
    /// A json process file loader
    /// </summary>
    public partial class JsonProcessLoader : IProcessLoader
    {


        /// <summary>
        /// Retrieves an <see cref="IEnumerable{T}"/> of ProcessModel from a file
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IProcessModel> GetProcessListFromFile(string processListFile)
        {
            // Read json data from file 
            var json = File.ReadAllText(processListFile);

            try
            {
                // Parse json data
                var jsonData = JsonSerializer.Deserialize<IEnumerable<JsonProcessModel>>(json);

                return jsonData.Select<JsonProcessModel, IProcessModel>(jsonProcess =>
                {
                    if(jsonProcess.RunAsConsole == true)
                    {
                        return new ConsoleProcess(jsonProcess.ConsoleScript, jsonProcess.StartInDirectory)
                        {
                            ProcessLabel = jsonProcess.ProcessLabel,
                        };
                    }
                    else
                    {
                        return new GUIProcess(jsonProcess.ProcessPath, jsonProcess.ProcessArgs)
                        {
                            ProcessLabel = jsonProcess.ProcessLabel,
                        };
                    };

                });

            }
            // If parsing failed return null
            catch (JsonException jsonException)
            {
                MessageBox.Show($"Unable to parse {processListFile}.\nError: {jsonException.Message}", "Error");
                throw;
            };
        }
    };
};
