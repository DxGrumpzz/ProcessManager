namespace ProcessManager.UI
{
    using System.Windows;

    /// <summary>
    /// 
    /// </summary>
    public interface IUserDialog
    {
        public UserDialogResult ShowDialog(string dialogText, string dialogTitle = "");
        public UserDialogResult ShowChoiceDialog(string dialogText, string dialogTitle = "");

    };
};
