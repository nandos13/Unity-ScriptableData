using UnityEditor;

namespace JakePerry.Unity.ScriptableData
{
    // TODO: Add menu item to remove generated classes for types that are no longer attributed
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Private static methods are accessed by Unity via reflection.")]
    internal static class MenuItems
    {
        private const string GENERATE_MISSING_CLASSES_ITEM = "Plugins/JakePerry/ScriptableData/Generate Missing Data Container Classes";
        private const string GENERATE_ALL_CLASSES_ITEM = "Plugins/JakePerry/ScriptableData/Regenerate All Data Container Classes";

        [MenuItem(GENERATE_MISSING_CLASSES_ITEM, true)]
        [MenuItem(GENERATE_ALL_CLASSES_ITEM, true)]
        private static bool NotPlayingOrCompilingValidator()
        {
            return !EditorApplication.isPlayingOrWillChangePlaymode
                && !EditorApplication.isCompiling;
        }

        [MenuItem(GENERATE_MISSING_CLASSES_ITEM, false)]
        private static void GenerateMissingDataContainerClasses()
        {
            DataContainerGenerator.GenerateMissingTypes();
        }

        [MenuItem(GENERATE_ALL_CLASSES_ITEM, false)]
        private static void GenerateAllDataContainerClasses()
        {
            DataContainerGenerator.GenerateAllTypes();
        }
    }
}
