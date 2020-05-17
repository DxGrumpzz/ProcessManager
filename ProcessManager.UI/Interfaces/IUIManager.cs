namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public enum View
    {
        ProjectsListView = 0,
        ProjectItemView = 1,

        EditGUIProcessView = 2,
        EditConsoleProcessView = 3,

        AddGUIProcessView = 4,
        AddConsoleProcessView = 5,

        AddProcessView = 6,
    };


    /// <summary>
    /// 
    /// </summary>
    public interface IUIManager
    {

        public void ChangeView<T>(View view, T viewmodel) 
            where T : BaseViewModel;

    };
};