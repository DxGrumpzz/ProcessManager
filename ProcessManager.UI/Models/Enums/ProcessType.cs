namespace ProcessManager.UI
{
    using System;

    /// <summary>
    /// An enum for available process types
    /// </summary>
    public enum ProcessType
    {
        /// <summary>
        /// A process that runs using a GUI
        /// </summary>
        GUI = 0,

        /// <summary>
        /// A process which will be ran from the OS' console system
        /// </summary>
        Console = 1,

    };
};