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


        /// <summary>
        /// Check if Processes.txt is valid
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        public bool IsProcessesFileFileValid()
        {
            // The process file must contain at least 2 lines for it to be valid
            if (_fileLines.Length < 2)
                return false;

            // The [processes] keyword must show up at least once
            if (_fileLines.Count(line => line.ToLower() == "[process]") < 1)
                return false;

            // Get processes 
            var processPaths = GetProcessPaths(_fileLines);

            // Check if the process paths are valid
            foreach (string processPath in processPaths)
            {
                if (File.Exists(processPath) == false)
                {
                    return false;
                };
            };

            // We reached here, Processes file is valid
            return true;
        }


        /// <summary>
        /// Reads the processes file and retrieves the process paths
        /// </summary>
        /// <param name="fileData"> The porcess file inner text </param>
        /// <returns></returns>
        private IEnumerable<string> GetProcessPaths(string[] fileData)
        {
            // Go through every line
            for (int a = 0; a < fileData.Length; a++)
            {
                // Get the current line being read
                string currentLine = fileData[a].ToLower();

                // If the current line specifies a process
                if(currentLine == "[process]")
                {
                    // Read the next line and "return" the process path
                    yield return fileData[++a]; 
                };
            };
        }

    };
};