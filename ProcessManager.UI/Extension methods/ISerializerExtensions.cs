namespace ProcessManager.UI
{
    using System.Collections.Generic;
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

    };
};