namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;

    /// <summary>
    /// A json process file loader
    /// </summary>
    public class JsonProcessLoader : IProcessLoader
    {
        // The path to the process file
        private string _processListFile;

        public JsonProcessLoader(string processListFile)
        {
            _processListFile = processListFile;
        }


        /// <summary>
        /// Retrieves an <see cref="IEnumerable{T}"/> of ProcessModel from a file
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProcessModel> GetProcessListFromFile()
        {
            // Read json data from file 
            var json = File.ReadAllText(_processListFile);

            IEnumerable<ProcessModel> processList;
            try
            {
                // Parse json data
                processList = JsonSerializer.Deserialize<IEnumerable<ProcessModel>>(json);
            }
            // If parsing failed return null
            catch
            {
                return null;
            };

            return processList;
        }
    };
};
