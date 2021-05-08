using System;
using System.Linq;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace JakePerry.Unity.ScriptableData
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator")]
    public static class DataContainerGenerator
    {
        private struct CodeFileInfo
        {
            public string className;
            public string fileContents;
        }

        private const string GENERATED_SCRIPT_LABEL = "JakePerry.Unity.ScriptableData.GeneratedScriptFile";

        private static CodeTypeDeclaration[] GenerateTypeDeclarations(IReadOnlyCollection<Type> dataTypes)
        {
            var genericArgs = new Type[1];

            var output = new CodeTypeDeclaration[dataTypes.Count];
            int index = 0;

            foreach (var type in dataTypes)
            {
                genericArgs[0] = type;
                var baseType = new CodeTypeReference(typeof(DataContainer<>).MakeGenericType(genericArgs));

                var className = $"DataContainer_{type.FullName.Replace('.', '_').Replace('+', '_')}";

                var classDeclaration = new CodeTypeDeclaration(className);
                classDeclaration.BaseTypes.Add(baseType);
                classDeclaration.TypeAttributes |= System.Reflection.TypeAttributes.Sealed;

                var attributeDeclaration = new CodeAttributeDeclaration(
                    name: typeof(GeneratedCodeAttribute).FullName,
                    arguments: new[]
                    {
                        new CodeAttributeArgument(new CodePrimitiveExpression("SampleCodeGenerator")),
                        new CodeAttributeArgument(new CodePrimitiveExpression(Constants.kPluginVersion))
                    }
                    );

                classDeclaration.CustomAttributes.Add(attributeDeclaration);

                output[index++] = classDeclaration;
            }

            return output;
        }

        private static CodeFileInfo[] GenerateCodeFiles(string @namespace, IReadOnlyCollection<CodeTypeDeclaration> types)
        {
            var provider = CodeDomProvider.CreateProvider("CSharp");
            var options = new CodeGeneratorOptions()
            {
                BracingStyle = "C",
                IndentString = "    ",
                BlankLinesBetweenMembers = false
            };

            var compileUnit = new CodeCompileUnit();
            var codeNamespace = new CodeNamespace(@namespace);

            compileUnit.Namespaces.Add(codeNamespace);

            var output = new CodeFileInfo[types.Count];
            int index = 0;

            using (var writer = new StringWriter())
            {
                foreach (var type in types)
                {
                    writer.GetStringBuilder().Clear();
                    codeNamespace.Types.Clear();

                    codeNamespace.Types.Add(type);

                    provider.GenerateCodeFromCompileUnit(compileUnit, writer, options);

                    output[index++] = new CodeFileInfo() { className = type.Name, fileContents = writer.ToString() };
                }
            }

            return output;
        }

        private static FileInfo WriteCodeFile(CodeFileInfo codeFileInfo, DirectoryInfo destination)
        {
            var file = new FileInfo(Path.Combine(destination.FullName, $"{codeFileInfo.className}.cs"));

            using (var writeStream = file.Open(FileMode.Create, FileAccess.Write))
            {
                using (var writer = new StreamWriter(writeStream))
                    writer.Write(codeFileInfo.fileContents);
            }

            return file;
        }

        private static void GenerateForDataTypes(Type[] dataTypes)
        {
            var @namespace = $"{typeof(DataContainer).Namespace}.GeneratedClasses";
            var codeFileInfos = GenerateCodeFiles(@namespace, GenerateTypeDeclarations(dataTypes));

            var directory = new DirectoryInfo(Path.Combine(Application.dataPath, "Generated", "JakePerry", "ScriptableData"));
            directory.Create();

            // Get reference to the project folder (one up from Assets).
            var projectDirFullName = directory.Parent.Parent.Parent.Parent.FullName;

            int trimCount = projectDirFullName.Length + 1;
            if (projectDirFullName.EndsWith("/") || projectDirFullName.EndsWith("\\"))
                trimCount++;

            AssetDatabase.DisallowAutoRefresh();
            try
            {
                int debugCount = 0;

                foreach (var codeFileInfo in codeFileInfos)
                {
                    var file = WriteCodeFile(codeFileInfo, directory);

                    var fileFullName = file.FullName;
                    Debug.Assert(fileFullName.StartsWith(projectDirFullName));

                    var projectRelativePath = fileFullName.Substring(trimCount).Replace('\\', '/');

                    AssetDatabase.ImportAsset(projectRelativePath);
                    var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(projectRelativePath);

                    if (monoScript == null)
                    {
                        Logger.LogError($"Failed to find generated file at path {projectRelativePath}");
                        continue;
                    }

                    EditorAssetUtilities.AddLabel(monoScript, GENERATED_SCRIPT_LABEL);
                    debugCount++;
                }

                AssetDatabase.Refresh();

                if (debugCount == codeFileInfos.Length)
                {
                    Logger.Log($"Successfully generated {debugCount.ToString()} code files.");
                }
                else
                {
                    Logger.Log($"{debugCount.ToString()} of {codeFileInfos.Length.ToString()} files were successfully generated.");
                }
            }
            finally
            {
                AssetDatabase.AllowAutoRefresh();
            }
        }

        private static string GetTypeGenLogMessage(IEnumerable<Type> types)
        {
            var joinedTypeNames = string.Join("\n", types.Select(t => t.FullName));
            return $"Attempting to generate data containers for the following data types:\n{joinedTypeNames}";
        }

        public static void GenerateAllTypes()
        {
            var types = ScriptableDataUtilities.GetAllDataTypes();

            if (types.Count > 0)
            {
                Logger.Log(GetTypeGenLogMessage(types));
                GenerateForDataTypes(types.ToArray());
            }
            else
            {
                Logger.Log("No scriptable data types were found, doing nothing.");
            }
        }

        public static void GenerateMissingTypes()
        {
            var missingTypes = ScriptableDataUtilities.GetMissingDataTypes();

            if (missingTypes.Count > 0)
            {
                Logger.Log(GetTypeGenLogMessage(missingTypes));
                GenerateForDataTypes(missingTypes.ToArray());
            }
            else
            {
                Logger.Log("No missing types were found, doing nothing.");
            }
        }
    }
}
