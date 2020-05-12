namespace ProcessManager.UI
{
    using System.Diagnostics;
    using System.Linq;
    using System.Text.Json;


    /// <summary>
    /// A base class for <see cref="AddGUIProcessViewModel"/>, and <see cref="AddConsoleProcessViewModel"/> to extend functionality
    /// </summary>
    public class AddProcessViewModelBase : BaseViewModel
    {
      
        protected AddProcessViewModelBase() { }


        /// <summary>
        /// Converts a project's process list to a Json string
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        protected string SerializeProcessList(Project project)
        {
            // Convert the process list inside the project to json
            return DI.Serializer.SerializeToString(
                project.ProcessList
                .Select(process =>
                {
                    switch (process)
                    {
                        case ConsoleProcess consoleProcess:
                            return ConsoleProcessAsJsonProcess(consoleProcess);

                        case GUIProcess guiProcess:
                            return GUIProcessAsJsonProcess(guiProcess);

                        default:
                        {
                            Debugger.Break();
                            return default;
                        };
                    };
                }));
        }


        /// <summary>
        /// Converts a <see cref="GUIProcess"/> to a <see cref="ConsoleProcess"/> 
        /// </summary>
        /// <param name="consoleProcess"></param>
        /// <returns></returns>
        private JsonProcessModel ConsoleProcessAsJsonProcess(ConsoleProcess consoleProcess)
        {
            return new JsonProcessModel
            {
                RunAsConsole = true,

                VisibleOnStartup = consoleProcess.VisibleOnStartup,

                StartInDirectory = consoleProcess.StartupDirectory,
                ConsoleScript = consoleProcess.ConsoleScript,

                ProcessLabel = consoleProcess.ProcessLabel,
            };
        }

        /// <summary>
        /// Converts a <see cref="GUIProcess"/> to a <see cref="JsonProcessModel"/> 
        /// </summary>
        /// <param name="consoleProcess"></param>
        /// <returns></returns>
        private JsonProcessModel GUIProcessAsJsonProcess(GUIProcess guiProcess)
        {
            return new JsonProcessModel
            {
                RunAsConsole = false,

                VisibleOnStartup = guiProcess.VisibleOnStartup,

                ProcessPath = guiProcess.ProcessPath,
                ProcessArgs = guiProcess.ProcessArgs,

                ProcessLabel = guiProcess.ProcessLabel,
            };
        }


    };
};
