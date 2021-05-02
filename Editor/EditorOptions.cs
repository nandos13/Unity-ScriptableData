using UnityEditor;

namespace JakePerry.Unity.ScriptableData
{
    // TODO: Rename this class. This should serve as the public interface for customizable options
    // for the user. This might need an editor window built around it or something similar so
    // users can change these options via editor UI.
    public static class EditorOptions
    {
        public const string kAutoGeneratePrefsKey = "JakePerry.Unity.ScriptableData.AutoGenerate";

        public static bool AutoGenerateContainerClasses
        {
            get => EditorPrefs.GetBool(kAutoGeneratePrefsKey, true);
            set => EditorPrefs.SetBool(kAutoGeneratePrefsKey, value);
        }
    }
}
