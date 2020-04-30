namespace ProcessManager.UI
{
    using System;
    using System.Diagnostics;
    using System.Windows;


    /// <summary>
    /// A windows implementation of <see cref="IUserDialog"/>
    /// </summary>
    public class WindowsUserDialog : IUserDialog
    {
        public UserDialogResult ShowChoiceDialog(string dialogText, string dialogTitle = "")
        {
            // Show the messagebox with Yes, no, and close buttons
            var messageBoxResult = MessageBox.Show(dialogText, dialogTitle, MessageBoxButton.YesNo);


            return MessageBoxResultToUserDialogResult(messageBoxResult);
        }

        public UserDialogResult ShowDialog(string dialogText, string dialogTitle = "")
        {
            // Display a messagebox, contains only an OK button and text
            var messageBoxResult = MessageBox.Show(dialogText, dialogTitle);

            return MessageBoxResultToUserDialogResult(messageBoxResult);
        }

        /// <summary>
        /// Converts a <see cref="MessageBoxResult"/> to <see cref="UserDialogResult"/>
        /// </summary>
        /// <param name="messageBoxResult"></param>
        /// <returns></returns>
        private static UserDialogResult MessageBoxResultToUserDialogResult(MessageBoxResult messageBoxResult)
        {
            switch (messageBoxResult)
            {
                case MessageBoxResult.Cancel:
                    return UserDialogResult.Cancel;

                case MessageBoxResult.Yes:
                    return UserDialogResult.Yes;

                case MessageBoxResult.No:
                    return UserDialogResult.No;

                case MessageBoxResult.None:
                    return UserDialogResult.None;

                default:
                {
                    // How the hell did you get here ??
                    Debugger.Break();
                    throw new Exception($"Unexpected value in retured in {messageBoxResult}");
                };
            };
        }
    };
};
