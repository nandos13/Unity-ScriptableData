using UnityEditor;
using UnityEngine;

namespace JakePerry.Unity.ScriptableData
{
    // TODO: This window needs a design pass, need to improve the layout etc.
    internal sealed class AboutWindow : EditorWindow
    {
        private static readonly GUIContent m_header = new GUIContent(Constants.kPackageName);
        private static readonly GUIContent m_author = new GUIContent($"Author: {Constants.kPackageAuthor}");
        private static readonly GUIContent m_version = new GUIContent($"Version: {Constants.kPluginVersion}");

        internal static void ShowAboutWindow()
		{
			var rect = new Rect(100f, 100f, 570f, 360f);

			AboutWindow windowWithRect = GetWindowWithRect<AboutWindow>(rect, utility: true, $"About {Constants.kPackageName} plugin");
			windowWithRect.position = rect;
		}

        private void DoTitleRegion()
        {
            // TODO: Make an icon show here?
            EditorGUILayout.LabelField(m_header, EditorStyles.boldLabel);
        }

        private void DoPackageInfoRegion()
        {
            EditorGUILayout.LabelField(m_author);
            EditorGUILayout.LabelField(m_version);
        }

        private void OnGUI()
        {
            DoTitleRegion();
            DoPackageInfoRegion();
        }
    }
}
