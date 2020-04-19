namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.Text;


    /// <summary>
    ///
    /// </summary>
    public interface IFileDialog
    {
        public string GetOpenFileDialogRresult();
        public bool ShowOpenFileDialog();

    };
};