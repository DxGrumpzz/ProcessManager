namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// 
    /// </summary>
    public class ProjectListItemViewModel : BaseViewModel
    {

        public static ProjectListItemViewModel DesignInstance => new ProjectListItemViewModel(
            new Project()
            {
                ProjectPath = @"C:\Software\Secret Project",
            })
        {
            SettingsButtonVisible = true,
        };

         /// <summary>
        /// A common key for the data passed between drag and drop operations
        /// </summary>
        private const string DRAG_DROP_DATA_NAME = "ViewModelData";



        #region Private fields

        private bool _isSelected;

        private bool _settingsButtonVisible;
       
        private bool _dragAndDropEnabled;

        #endregion



        #region Public properties

        /// <summary>
        /// A boolean flag that indicates if the project settings button should be visible
        /// </summary>
        public bool SettingsButtonVisible
        {
            get => _settingsButtonVisible;
            set
            {
                _settingsButtonVisible = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A boolean flag that indicates that the "something" is over the control, the user's mouse, a draggable element, and such
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// A <see cref="Project"/> associated with this viewmodel
        /// </summary>
        public Project Project { get; }


        /// <summary>
        /// "Shared" drag and drop data that will be passed between "dragiee" and "dropiee" controls
        /// </summary>
        public Dictionary<string, object> DragDropData { get; }


        /// <summary>
        /// A boolean flag that indicates if the drag and drop operations are enabled
        /// </summary>
        public bool DragAndDropEnabled
        {
            get => _dragAndDropEnabled;
            set
            {
                _dragAndDropEnabled = value;
                OnPropertyChanged();
            }
        }


        #endregion



        #region Public commands

        public ICommand RunProjectCommand { get; }
        public ICommand CloseProjectCommand { get; }

        public ICommand MouseEnterCommand { get; }
        public ICommand MouseLeaveCommand { get; }

        public ICommand SwitchToProjectViewCommand { get; }


        public ICommand DragEnterCommand { get; }
        public ICommand DragLeaveCommand { get; }
        public ICommand DropCommand { get; }

        public ICommand MouseMovedCommand { get; }

        #endregion


        private ProjectListItemViewModel() { }
        public ProjectListItemViewModel(Project project)
        {
            Project = project;

            DragDropData = new Dictionary<string, object>()
            {
                { DRAG_DROP_DATA_NAME, this },
            };


            RunProjectCommand = new RelayCommand(ExecuteRunProjectCommand, singleFire: true);
            CloseProjectCommand = new RelayCommand(ExecuteCloseProjectCommand, singleFire: true);

            SwitchToProjectViewCommand = new RelayCommand(ExecuteSwitchToProjectViewCommand);


            // Bind drag and drop events
            DragEnterCommand = new RelayCommand<ProjectListItemViewModel>((project) =>
            {
                IsSelected = true;
            });

            DragLeaveCommand = new RelayCommand<ProjectListItemViewModel>((project) =>
            {
                IsSelected = false;
            });

            DropCommand = new RelayCommand<Dictionary<string, object>>((dragDropData) =>
            {
                // Validate that data actually exists
                if (dragDropData is null)
                {
                    DI.Logger.Log("Drag and drop error. Dropped data is null", LogLevel.Warning);
                    Debugger.Break();
                    return;
                };

                // Check if dropped data actually contains valid info
                if (dragDropData.TryGetValue(DRAG_DROP_DATA_NAME, out object dropData) == false)
                {
                    DI.Logger.Log("Drag and drop error. Unable to find valid data", LogLevel.Error);
                    Debugger.Break();
                    return;
                };

                // Check if dropped data is actually of the correct type
                if (!(dropData is ProjectListItemViewModel dropDataAsVM))
                {
                    DI.Logger.Log("Drag and drop error. Dropped data contains invald value(s)", LogLevel.Warning);
                    Debugger.Break();
                    return;
                };

                // Do drop stuff
                Drop(dropDataAsVM);
            });



            // Bind mouse commands

            // When the mouse moves over the drag drop button
            MouseMovedCommand = new RelayCommand<MouseMovedInfo>((mouseInfo) =>
            {
                // Check if user held his left mouse button
                if (mouseInfo.LeftButtonPressed == true)
                {
                    // Enable drag drop
                    DragAndDropEnabled = true;
                }
                else
                {
                    DragAndDropEnabled = false;
                };
            });

            MouseEnterCommand = new RelayCommand(() =>
            {
                SettingsButtonVisible = true;
                IsSelected = true;
            });

            MouseLeaveCommand = new RelayCommand(() =>
            {
                SettingsButtonVisible = false;
                IsSelected = false;
            });
        }




       
        private void ExecuteRunProjectCommand()
        {
            foreach (var process in Project.ProcessList)
            {
                Task.Run(process.RunProcess);
            };
        }

        private void ExecuteCloseProjectCommand()
        {
            // Only close running processes 
            var runningProcesses = Project.ProcessList.Where(process => process.IsRunning == true);

            foreach (var process in runningProcesses)
            {
                Task.Run(process.CloseProcess);
            };
        }

        private void ExecuteSwitchToProjectViewCommand()
        {
            DI.UI.ChangeView(View.ProjectItemView, new ProjectItemViewModel(Project));
        }


        private void Drop(ProjectListItemViewModel droppedData)
        {
            // Swap between the dragged project and dropped project
            SwapProjects(droppedData);

            // Write the changes to file
            var bytes = DI.Serializer.SerializerProjects(DI.Projects);
            File.WriteAllBytes(Localization.PROJECTS_FILE_NAME, bytes);

            // Update the projects list 
            DI.ProjectsListVM.Projects = new ObservableCollection<ProjectListItemViewModel>(DI.Projects.Select(project =>
            {
                return new ProjectListItemViewModel(project);
            }));
        }


        /// <summary>
        /// Swaps places between 2 projects
        /// </summary>
        /// <param name="droppedData"></param>
        private void SwapProjects(ProjectListItemViewModel droppedData)
        {
            // Find indices of the 2 projects (this, and the dropped)
            var currentIndex = DI.Projects.IndexOf(Project);
            var droppedIndex = DI.Projects.IndexOf(droppedData.Project);

            // Using a sophisticated swap algorithm with a constant runtime of O(1), swap the 2 elements between in the list
            var temp = DI.Projects[currentIndex];

            DI.Projects[currentIndex] = droppedData.Project;
            DI.Projects[droppedIndex] = temp;
        }



    };
};