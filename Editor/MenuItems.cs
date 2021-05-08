using System;
using UnityEditor;

namespace JakePerry.Unity.ScriptableData
{
    // TODO: Add menu item to remove generated classes for types that are no longer attributed
    [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Private static methods are accessed by Unity via reflection.")]
    internal static class MenuItems
    {
        internal const string OPEN_ABOUT_ITEM = "Plugins/JakePerry/ScriptableData/About";
        internal const string OPEN_SETTINGS_ITEM = "Plugins/JakePerry/ScriptableData/Settings";
        internal const string GENERATE_MISSING_CLASSES_ITEM = "Plugins/JakePerry/ScriptableData/Generate Missing Data Container Classes";
        internal const string GENERATE_ALL_CLASSES_ITEM = "Plugins/JakePerry/ScriptableData/Regenerate All Data Container Classes";

        [MenuItem(OPEN_ABOUT_ITEM)]
        private static void ShowAboutWindow()
        {
            AboutWindow.ShowAboutWindow();
        }

        [MenuItem(OPEN_SETTINGS_ITEM, false)]
        private static void CreateOrFocusSettingsWindow()
        {
            var inspectorType = Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll", false);

            var dockTypes = inspectorType == null
                ? Array.Empty<Type>()
                : new Type[] { inspectorType };

            var window = EditorWindow.GetWindow<SettingsWindow>(dockTypes);

            window.Show();
            window.Focus();
        }

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
