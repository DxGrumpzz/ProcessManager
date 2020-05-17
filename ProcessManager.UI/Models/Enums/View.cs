namespace ProcessManager.UI
{
    /// <summary>
    /// An enum for every view available to switch to 
    /// </summary>
    public enum View
    {
        /// <summary>
        /// The apps main page
        /// </summary>
        ProjectsListView = 0,

        /// <summary>
        /// A view for a project, contains processes, and project settings
        /// </summary>
        ProjectItemView = 1,

        /// <summary>
        /// A view that allows editing a GUI process
        /// </summary>
        EditGUIProcessView = 2,

        /// <summary>
        /// A view that allows editing a Console process
        /// </summary>
        EditConsoleProcessView = 3,

        /// <summary>
        /// A view for adding a GUI process to a project
        /// </summary>
        AddGUIProcessView = 4,

        /// <summary>
        /// A view for adding a Console process to a project
        /// </summary>
        AddConsoleProcessView = 5,

        /// <summary>
        /// A view that allows the user to select a process type to add 
        /// </summary>
        AddProcessView = 6,

    };
};