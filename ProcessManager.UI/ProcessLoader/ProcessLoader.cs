namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The default implementation of <see cref="IProcessLoader"/>
    /// </summary>
    public class ProcessLoader : IProcessLoader
    {
        private string[] _fileLines;

        public ProcessLoader(string filePath)
        {
            // Read the file line-by-line
            _fileLines = File.ReadAllLines(filePath)
                // Remove empty lines
                .Where(line => string.IsNullOrWhiteSpace(line) == false)
                .ToArray();
        }


        /// <summary>
        /// Retrieves an <see cref="IEnumerable{T}"/> of ProcessModel from a file
        /// </summary>
        /// <param name="filePath"> The path to the file </param>
        /// <returns></returns>
        public IEnumerable<ProcessModel> GetProcessesFromFile()
        {
            // Stores the processes
            List<ProcessModel> processList = new List<ProcessModel>();


            for (int a = 0; a < _fileLines.Length; a++)
            {
                // Hold the current line 
                string currentLine = _fileLines[a].ToLower();

                // If the current line specifies a process path
                if (currentLine == "[process]")
                {
                    // Add a new ProcessModel to the list with the name of the process
                    processList.Add(new ProcessModel()
                    {
                        ProcessPath = _fileLines[++a],
                    });
                }
                // If the current line specifies a process' arguments
                else if (currentLine == "[args]")
                {
                    // Get the last process added and set it's arguments to the next line
                    processList.Last().ProcessArgs = _fileLines[++a];
                };
            };

            return processList;
        }

    };
};
