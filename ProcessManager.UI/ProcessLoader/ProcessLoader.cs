namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Windows;

    /// <summary>
    /// A json process file loader
    /// </summary>
    public class ProcessLoader : IProcessLoader
    {
        private readonly ISerializer _serializer;

        public ProcessLoader(ISerializer serializer)
        {
            _serializer = serializer;
        }

        /// <summary>
        /// Retrieves an <see cref="IEnumerable{T}"/> of ProcessModel from a file
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IProcessModel> GetProcessListFromFile(string processListFile)
        {
            // Read json data from file 
            var json = File.ReadAllText(processListFile);

            // Parse json data
            var jsonData = _serializer.DeserializeFromString<IEnumerable<JsonProcessModel>>(json);

            return jsonData.Select<JsonProcessModel, IProcessModel>(jsonProcess =>
            {
                if (jsonProcess.RunAsConsole == true)
                {
                    return new ConsoleProcess(jsonProcess.ConsoleScript, jsonProcess.StartInDirectory, jsonProcess.VisibleOnStartup)
                    {
                        ProcessLabel = jsonProcess.ProcessLabel,
                    };
                }
                else
                {
                    return new GUIProcess(jsonProcess.ProcessPath, jsonProcess.ProcessArgs, jsonProcess.VisibleOnStartup)
                    {
                        ProcessLabel = jsonProcess.ProcessLabel,
                    };
                };

            });
        }
    };
};
