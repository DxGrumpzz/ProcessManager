namespace ProcessManager.UI
{
    using System;

    /// <summary>
    /// An Enum contaning results for the <see cref="IUserDialog.ShowDialog(string, string)"/> function
    /// </summary>
    public enum UserDialogResult
    {
        /// <summary>
        /// No chose was made. (User closed dialog)
        /// </summary>
        None = 0,

        /// <summary>
        /// User decided to cancel the action
        /// </summary>
        Cancel = 1,

        /// <summary>
        /// User chose yes 
        /// </summary>
        Yes = 2,

        /// <summary>
        /// User chose no
        /// </summary>
        No = 3,

    };
};