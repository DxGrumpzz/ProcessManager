namespace ProcessManager.UI.Models.UI
{
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    public class UImanager : IUIManager
    {
        public void ChangeView<T>(View view, T viewmodel)
            where T : BaseViewModel
        {
            switch (view)
            {
                case View.ProjectsListView:
                {
                    DI.MainWindowViewModel.CurrentView = new ProjectListView(viewmodel as ProjectsListViewModel);
                    break;
                };

                case View.ProjectItemView:
                {
                    DI.MainWindowViewModel.CurrentView = new ProjectItemView(viewmodel as ProjectItemViewModel);
                    break;
                };

                case View.EditGUIProcessView:
                {
                    DI.MainWindowViewModel.CurrentView = new EditGUIProcessView(viewmodel as EditGUIProcessViewModel);
                    break;
                };
                case View.EditConsoleProcessView:
                {
                    DI.MainWindowViewModel.CurrentView = new EditConsoleProcessView(viewmodel as EditConsoleProcessViewModel);
                    break;
                };

                case View.AddGUIProcessView:
                {
                    DI.MainWindowViewModel.CurrentView = new AddGUIProcessView(viewmodel as AddGUIProcessViewModel);
                    break;
                };

                case View.AddConsoleProcessView:
                {
                    DI.MainWindowViewModel.CurrentView = new AddConsoleProcessView(viewmodel as AddConsoleProcessViewModel);
                    break;
                };

                case View.AddProcessView:
                {
                    DI.MainWindowViewModel.CurrentView = new AddProcessView(viewmodel as AddProcessViewModel);
                    break;
                };

                default:
                    Debugger.Break();
                    break;
            }
        }

    };
};