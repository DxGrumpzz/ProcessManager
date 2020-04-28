namespace ProcessManager.UI
{
    using System.Windows;

    /// <summary>
    /// 
    /// </summary>
    public interface IUserDialog
    {
        public UserDialogResult ShowDialog(string dialogText, string dialogTitle = "");

    };
};
