namespace ProcessManager.UI
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// An implementation of a <see cref="IUIManager"/> for WPF
    /// </summary>
    public class UImanager : IUIManager
    {

        public void ChangeView<TViewModel>(View view, TViewModel viewmodel)
            where TViewModel : BaseViewModel
        {

            switch (view)
            {
                case View.ProjectsListView:
                {
                    ValidateViewModelType<TViewModel, ProjectsListViewModel>(viewmodel);
                    DI.MainWindowViewModel.CurrentView = new ProjectListView(viewmodel as ProjectsListViewModel);
                    break;
                };

                case View.ProjectItemView:
                {
                    ValidateViewModelType<TViewModel, ProjectItemViewModel>(viewmodel);
                    DI.MainWindowViewModel.CurrentView = new ProjectItemView(viewmodel as ProjectItemViewModel);
                    break;
                };

                case View.EditGUIProcessView:
                {
                    ValidateViewModelType<TViewModel, EditGUIProcessViewModel>(viewmodel);
                    DI.MainWindowViewModel.CurrentView = new EditGUIProcessView(viewmodel as EditGUIProcessViewModel);
                    break;
                };

                case View.EditConsoleProcessView:
                {
                    ValidateViewModelType<TViewModel, EditConsoleProcessViewModel>(viewmodel);
                    DI.MainWindowViewModel.CurrentView = new EditConsoleProcessView(viewmodel as EditConsoleProcessViewModel);
                    break;
                };

                case View.AddGUIProcessView:
                {
                    ValidateViewModelType<TViewModel, AddGUIProcessViewModel>(viewmodel);
                    DI.MainWindowViewModel.CurrentView = new AddGUIProcessView(viewmodel as AddGUIProcessViewModel);
                    break;
                };

                case View.AddConsoleProcessView:
                {
                    ValidateViewModelType<TViewModel, AddConsoleProcessViewModel>(viewmodel);
                    DI.MainWindowViewModel.CurrentView = new AddConsoleProcessView(viewmodel as AddConsoleProcessViewModel);
                    break;
                };

                case View.AddProcessView:
                {
                    ValidateViewModelType<TViewModel, AddProcessViewModel>(viewmodel);
                    DI.MainWindowViewModel.CurrentView = new AddProcessView(viewmodel as AddProcessViewModel);
                    break;
                };

                default:
                    Debugger.Break();
                    break;
            };

        }

        /// <summary>
        /// Takes an expected viewmodel in <typeparamref name="T"/>, and the actual viewmodel passed in <typeparamref name="TViewModel"/>. And evalutes them to see if their types match
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="viewModel"></param>
        private void ValidateViewModelType<T, TViewModel>(T viewModel)
            where TViewModel : BaseViewModel
        {
            // Check if the viewmodel types match
            if (!(viewModel is TViewModel))
            {
                Debugger.Break();

                throw new ArgumentException("An invlid Viewmodel type was supplied.\n" +
                    $"Expected: {typeof(TViewModel)},\n" +
                    $"Recevied: {viewModel.GetType()}");
            };
        }

    };
};