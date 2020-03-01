namespace ProcessManager.UI
{
    using System.Collections.Generic;

    /// <summary>
    /// 
    /// </summary>
    public static class DI
    {
        public static List<ProcessModel> ProcessList { get; set; }

        public static IProcessLoader ProcessLoader { get; set; }

    };
};
