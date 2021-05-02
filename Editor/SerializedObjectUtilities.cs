using System;
using System.Collections.Generic;
using UnityEditor;

namespace JakePerry.Unity
{
    // TODO: Move this class to another project
    public static class SerializedObjectUtilities
    {
        /// <summary>
        /// Enumerates all direct children of the given property; that is, all children with
        /// a hierarchy depth equal to the depth of the parent plus one.
        /// </summary>
        public static IEnumerable<SerializedProperty> EnumerateChildProperties(SerializedProperty parent)
        {
            var copy = parent.Copy();
            var depth = copy.depth + 1;

            while (copy.Next(true))
            {
                if (copy.depth == depth)
                    yield return copy;
            }
        }

        /// <summary>
        /// Sets the script type of the given serialized object.
        /// </summary>
        /// <param name="serializedObject">The target serialized object to change the script type for.</param>
        /// <param name="script">The script to assign to the object's type.</param>
        /// <param name="apply">If false, changes will not be immediately applied to the serialized object.</param>
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
