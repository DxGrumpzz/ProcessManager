﻿namespace ProcessManager.UI
{
    using System.Collections.Generic;

    /// <summary>
    /// Simple DI container
    /// </summary>
    public static class DI
    {
        /// <summary>
        /// The process list loaded from file
        /// </summary>
        public static List<ProcessModel> ProcessList { get; set; }

        /// <summary>
        /// The process list file loader/reader
        /// </summary>
        public static IProcessLoader ProcessLoader { get; set; }

    };
};
