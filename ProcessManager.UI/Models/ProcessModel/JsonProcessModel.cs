namespace ProcessManager.UI
{
    /// <summary>
    /// A simple struct that contains the process data as a json 
    /// </summary>
    public struct JsonProcessModel
    {
        public bool VisibleOnStartup { get; set; }

        public string ProcessLabel { get; set; }


        public string ProcessPath { get; set; }
        public string ProcessArgs { get; set; }


        public bool RunAsConsole { get; set; }


        public string StartInDirectory { get; set; }

        public string ConsoleScript { get; set; }
    };
};
