namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Text.Json;
    using System.Xml.Linq;

    /// <summary>
    /// 
    /// </summary>
    public class BuildResourceExtensionsFile
    {

        public const string BUILD_CONFIG_FILENAME = "ResourceExtension.Build.Config.Json";

        // The name of the attribute which dictaes if the resource will be loaded into file
        public const string LOAD_INTO_FILE_ATTRIBUTE_NAME = "BuildResourceExtentions.LoadIntoFile";

        // The name of the key attribute
        public const string KEY_ATTRIBUTE_NAME = "Key";

        const string RESOURCE_DICTIONARY_DECLARATION = "ResourceDictionary resourceDictionary";

        /// <summary>
        /// A simple struct that conatins info about an extension method that will be added to the ResourceExtensions file
        /// </summary>
        public struct FunctionData
        {
            /// <summary>
            /// The name of the function
            /// </summary>
            public string FunctionName { get; set; }

            /// <summary>
            /// The function's return statement
            /// </summary>
            public string FunctionReturn { get; set; }
        };
        

        /// <summary>
        /// A class that contains config information for creating a resource file
        /// </summary>
        public class ResourceExtensionsFileInfo
        {
            public string ProjectNamespace { get; set; }

            public string[] NamespaceIncludes { get; set; }

            public string ExtensionClassName { get; set; }
            
            public string ResourcesFilePath { get; set; }

            public string OutputFilePath { get; set; }

        };



        public static void Main()
        {
            // Deserialize the config file to a ResourceExtensionsFileInfo
            var buildInfo = JsonSerializer.Deserialize<ResourceExtensionsFileInfo>(File.ReadAllText(BUILD_CONFIG_FILENAME));

            // Parse the xaml resource file
            var parsedXaml = ParseXamlResourceFile(buildInfo);

            // Convert the parsed xaml to extension methods declarations
            var functions = XamlToFunctionData(parsedXaml);

            // Build a C# class using a StringBuilder
            var classStringBuilder = CreateResourceStringBuilder(buildInfo, functions);

            // Create the C# file
            BuildResourceFile(classStringBuilder, buildInfo);
        }


        /// <summary>
        /// Parse a xaml file 
        /// </summary>
        /// <param name="buildInfo"></param>
        /// <returns></returns>
        private static XElement ParseXamlResourceFile(ResourceExtensionsFileInfo buildInfo)
        {
            // Read the data inside the reosurce file
            var fileData = File.ReadAllText(buildInfo.ResourcesFilePath);

            // Parse the read data
            XElement xmlData = XElement.Parse(fileData);

            return xmlData;
        }


        /// <summary>
        /// Takes xml (xaml) data and converts it to a list of <see cref="FunctionData"/>
        /// </summary>
        /// <param name="xmlData"></param>
        /// <returns></returns>
        private static List<FunctionData> XamlToFunctionData(XElement xmlData)
        {
            // A list of elements inside the xaml resource
            var elements = xmlData.Elements();

            // Because we can't use colon(:) character in xml parsing I need to get the namespace of these prefixes before finding attributes
            var loalAsNamespace = xmlData.GetNamespaceOfPrefix("local");
            var xamlNamespace = xmlData.GetNamespaceOfPrefix("x");

            List<FunctionData> functions = new List<FunctionData>();

            // For every resource...
            foreach (var element in elements)
            {
                // The attribute that checks if this element will be added to the extension class
                var loadIntoFileattribute = element.Attribute(XName.Get(LOAD_INTO_FILE_ATTRIBUTE_NAME, loalAsNamespace.ToString()));

                if (loadIntoFileattribute is null)
                    continue;

                // The key of this element
                var keyAttribute = element.Attribute(XName.Get(KEY_ATTRIBUTE_NAME, xamlNamespace.ToString()));


                // Validation checks...

                // If element has LoadIntoFile attribute check if it has a valid value
                if (!(bool.TryParse(loadIntoFileattribute.Value, out bool attributeValue)))
                    throw new Exception($"Attribute contains invalid value: \'{loadIntoFileattribute.Value}\'.\n" +
                        $"Expected Boolean");

                // If the value isn't true for some reason
                if (attributeValue == false)
                    continue;


                string functionName = $"Get{keyAttribute.Value}(this {RESOURCE_DICTIONARY_DECLARATION})";

                // Convert to C# statment
                string csSctring = $"return resourceDictionary[\"{keyAttribute.Value}\"];";


                functions.Add(new FunctionData()
                {
                    FunctionName = functionName,
                    FunctionReturn = csSctring,
                });
            };

            return functions;
        }


        /// <summary>
        /// Takes a list of <see cref="FunctionData"/> and build a C# ResourceExtensions file
        /// </summary>
        /// <param name="buildInfo"></param>
        /// <param name="functions"></param>
        /// <returns></returns>
        private static StringBuilder CreateResourceStringBuilder(ResourceExtensionsFileInfo buildInfo, IEnumerable<FunctionData> functions)
        {
            // Stringbuilder used to create the class's inner data
            StringBuilder classStringBuilder = new StringBuilder();

            // Build the class
            string classNamespace = $"namespace {buildInfo.ProjectNamespace}";
            string classDeclaration = $"public static class {buildInfo.ExtensionClassName}";
            const string functionDeclaration = "public static object ";


            // Add the namespace
            classStringBuilder.AppendLine(classNamespace);
            classStringBuilder.AppendLine("{");

            // Add namespaces includes
            foreach (var include in buildInfo.NamespaceIncludes)
            {
                classStringBuilder.Append($"using {include}");
                classStringBuilder.AppendLine(";");
            };

            // Add the class declaration
            classStringBuilder.AppendLine(classDeclaration);
            classStringBuilder.AppendLine("{");


            // Add the functions
            foreach (var function in functions)
            {
                classStringBuilder.AppendLine($"{functionDeclaration} {function.FunctionName}");
                classStringBuilder.AppendLine("{");

                classStringBuilder.AppendLine($"{function.FunctionReturn}");

                classStringBuilder.AppendLine("}");
            };


            // End class declaration
            classStringBuilder.AppendLine("};");

            // End namespace
            classStringBuilder.Append("};");


            return classStringBuilder;
        }


        /// <summary>
        /// Creates a ResourceExtension.cs file 
        /// </summary>
        /// <param name="fileData"></param>
        /// <param name="buildInfo"></param>
        private static void BuildResourceFile(StringBuilder fileData, ResourceExtensionsFileInfo buildInfo)
        {
            // Write the class data into a file
            File.WriteAllText(buildInfo.OutputFilePath, fileData.ToString());
        }

    };
};