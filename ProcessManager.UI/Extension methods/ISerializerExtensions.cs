namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;


    /// <summary>
    /// A class that provides extensions for <see cref="ISerializer"/>
    /// </summary>
    public static class ISerializerExtensions
    {

        /// <summary>
        /// Converts a list of <see cref="Project"/> to a byte array
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="projects"></param>
        /// <returns></returns>
        public static byte[] SerializerProjects(this ISerializer serializer, IEnumerable<Project> projects)
        {
            // Convert the projects list to a json string and serialize
            var jsonBytes = serializer.Serialize(projects.Select(project => new JsonProject()
            {
                ProjectPath = project.ProjectPath,
            }));

            return jsonBytes;
        }


        /// <summary>
        /// Deserializes data to <see cref="Project"/> 
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="jsonProjectData"></param>
        /// <returns></returns>
        public static IEnumerable<Project> DeserializerProjects(this ISerializer serializer, byte[] jsonProjectData)
        {
            var projects = serializer.Deserialize<IEnumerable<Project>>(jsonProjectData);

            return projects;
        }


        public static byte[] SerializeProcessList(this ISerializer serializer, IList<IProcessModel> ProcessList)
        {
            byte[] projectBytes = default;

            if (serializer is JsonSerializer jsonSerializer)
            {
                projectBytes = serializer.Serialize(
                    ProcessList
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
            };


            return projectBytes;
        }





        /// <summary>
        /// Converts a <see cref="GUIProcess"/> to a <see cref="ConsoleProcess"/> 
        /// </summary>
        /// <param name="consoleProcess"></param>
        /// <returns></returns>
        private static JsonProcessModel ConsoleProcessAsJsonProcess(ConsoleProcess consoleProcess)
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
        private static JsonProcessModel GUIProcessAsJsonProcess(GUIProcess guiProcess)
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