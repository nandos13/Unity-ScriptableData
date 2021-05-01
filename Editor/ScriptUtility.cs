using System;
using UnityEditor;

namespace JakePerry.Unity.ScriptableData
{
    public static class ScriptUtility
    {
        public static bool TryFindScript(Type type, out MonoScript script)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));

            foreach (var path in AssetDatabase.GetAllAssetPaths())
                if (AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(MonoScript))
                {
                    var monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                    var classType = monoScript.GetClass();

                    if (monoScript.GetClass() == type)
                    {
                        script = monoScript;
                        return true;
                    }
                }

            script = null;
            return false;
        }

        public static void SetScriptType(SerializedObject serializedObject, MonoScript script, bool apply = true)
        {
            _ = serializedObject ?? throw new ArgumentNullException(nameof(serializedObject));
            _ = script ?? throw new ArgumentNullException(nameof(script));

            var scriptProperty = serializedObject.FindProperty("m_Script");

            if (scriptProperty == null)
                throw new ArgumentException("Failed to find m_Script property on the given SerializedObject.", nameof(serializedObject));

            scriptProperty.objectReferenceValue = script;

            if (apply)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
