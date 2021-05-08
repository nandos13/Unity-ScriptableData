using UnityEditor;
using UnityEngine;

using static JakePerry.Unity.ScriptableData.Settings;

namespace JakePerry.Unity.ScriptableData
{
    // TODO: This window needs a design pass, need to improve the layout etc.
    [EditorWindowTitle(title = "Scriptable Data Settings", icon = "Settings")]
    internal sealed class SettingsWindow : EditorWindow
    {
        #region GUIContent values

        private static readonly GUIContent m_general_header = new GUIContent("General");
        private static readonly GUIContent m_general_autoGenerateContent = new GUIContent("Auto generate scripts on recompile", "If true, container scripts will be generated automatically on script compilation.");

        #endregion

        private void OnEnable()
        {
            // Create title GUIContent with a settings gear icon
            var titleContent = EditorGUIUtility.IconContent("Settings");
            titleContent.text = "Scriptable Data Settings";

            base.titleContent = titleContent;
        }

        private void DoGeneralSettings()
        {
            EditorGUILayout.LabelField(m_general_header, EditorStyles.boldLabel);

            // TODO: Consider making this recompile immediately when toggled on?
            AutoGenerateContainerClasses = EditorGUILayout.ToggleLeft(m_general_autoGenerateContent, AutoGenerateContainerClasses);
        }

        private void OnGUI()
        {
            DoGeneralSettings();
        }
    }
}
