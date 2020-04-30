namespace ProcessManager.UI
{
    using System.Windows;

    /// <summary>
    /// An interface describing a user dialog
    /// </summary>
    public interface IUserDialog
    {
        /// <summary>
        /// Shows a user dialog that only notifies the user, Doesn't contain option buttons
        /// </summary>
        /// <param name="dialogText"> The text inside the dialog </param>
        /// <param name="dialogTitle"> The dialog's title text  </param>
        /// <returns></returns>
        public UserDialogResult ShowDialog(string dialogText, string dialogTitle = "");

        /// <summary>
        /// Shows a user dialog that can take input from the user in a form of:
        /// Yes, No, and close window button
        /// </summary>
        /// <param name="dialogText"> The text inside the dialog </param>
        /// <param name="dialogTitle"> The dialog's title text  </param>
        /// <returns></returns>
        public UserDialogResult ShowChoiceDialog(string dialogText, string dialogTitle = "", UserDialogButton defaultDialogButton = UserDialogButton.No);

    };
};
