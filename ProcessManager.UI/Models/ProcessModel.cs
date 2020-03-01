namespace ProcessManager.UI
{
    using System;

    public class ProcessModel
    {
        public string ProcessName { get; set; }
        public string ProcessArgs { get; set; }

        public ulong ProcessID { get; set; }

        public bool IsRunning => ProcessID != 0;



        public bool RunProcess()
        {
            if (IsRunning == true)
                return false;

            ulong result = CoreDLL.RunProcess(ProcessName, ProcessArgs);
            ProcessID = result;

            return ProcessID != 0 ? true : false;
        }


        public bool CloseProcess()
        {
            if (IsRunning == false)
                return false;

            CoreDLL.CloseProcess(ProcessID);
            ProcessID = 0;

            return true;
        }
    };

};