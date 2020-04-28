namespace ProcessManager.UI
{
    using System.Windows;


    /// <summary>
    /// 
    /// </summary>
    public class WindowsUserDialog : IUserDialog
    {

        public void ShowDialog(string dialogText, string dialogTitle = "")
        {
            MessageBox.Show(dialogText, dialogTitle);
        }

    };
};
