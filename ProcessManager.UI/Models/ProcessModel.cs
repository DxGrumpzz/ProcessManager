namespace ProcessManager.UI
{
    using System;

    public class ProcessModel
    {
        public string ProcessName { get; set; }
        public string ProcessArgs { get; set; }

        public ulong ProcessID { get; set; }

        public bool IsRunning => ProcessID != 0;
    };

};