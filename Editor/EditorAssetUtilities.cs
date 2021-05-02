using System;
using UnityEditor;

namespace JakePerry.Unity
{
    // TODO: Move this class to another project
    public static class EditorAssetUtilities
    {
        public static void AddLabel(UnityEngine.Object asset, string label)
        {
            var labels = AssetDatabase.GetLabels(asset);

            foreach (var l in labels)
                if (l == label)
                    return;

            ArrayUtility.Add(ref labels, label);

            AssetDatabase.SetLabels(asset, labels);
        }

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
    }
}
