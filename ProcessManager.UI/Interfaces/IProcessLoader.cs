namespace ProcessManager.UI
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public interface IProcessLoader
    {
        public IEnumerable<ProcessModel> GetProcessesFromFile(string filePath);
    
    };
};
