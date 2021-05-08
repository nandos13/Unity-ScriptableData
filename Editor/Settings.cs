using UnityEditor;

namespace JakePerry.Unity.ScriptableData
{
    public static class Settings
    {
        public const string kAutoGeneratePrefsKey = "JakePerry.Unity.ScriptableData.AutoGenerate";

        public static bool AutoGenerateContainerClasses
        {
            get => EditorPrefs.GetBool(kAutoGeneratePrefsKey, true);
            set => EditorPrefs.SetBool(kAutoGeneratePrefsKey, value);
        }
    }
}
