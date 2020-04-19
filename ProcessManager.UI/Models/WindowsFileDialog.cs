namespace ProcessManager.UI
{
    using Microsoft.Win32;


    /// <summary>
    /// 
    /// </summary>
    public class WindowsFileDialog : IFileDialog
    {
        OpenFileDialog _openFileDialog = new OpenFileDialog();

        public string GetOpenFileDialogRresult()
        {
            return _openFileDialog.FileName;
        }

        public bool ShowOpenFileDialog()
        {
            return _openFileDialog.ShowDialog().Value;
        }

    };
};