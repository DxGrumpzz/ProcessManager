namespace ProcessManager.UI
{
    using System;
    using System.Diagnostics;
    using System.Windows;


    /// <summary>
    /// 
    /// </summary>
    public class WindowsUserDialog : IUserDialog
    {

        public UserDialogResult ShowDialog(string dialogText, string dialogTitle = "")
        {
            var messageBoxResult = MessageBox.Show(dialogText, dialogTitle);

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
