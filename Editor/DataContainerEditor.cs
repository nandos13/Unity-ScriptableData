using JakePerry.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JakePerry.Unity.ScriptableData
{
    [CustomEditor(typeof(DataContainer), editorForChildClasses: true)]
    public sealed class DataContainerEditor : Editor
    {
        private static readonly GUIContent s_dataTypeLabel = new GUIContent("Type", "This is the type of data that will be serialized in this data container.");

        private Type[] m_types;
        private string[] m_labels;
        private int m_index;

        private SerializedProperty m_dataProperty;

        private static void GetAvailableDataTypes(out Type[] types, out string[] labels)
        {
            types = ScriptableDataUtilities.GetAllDataTypes().OrderBy(type => type.FullName).ToArray();
            labels = new string[types.Length + 1];

            // First option is always none
            labels[0] = "None";

            for (int i = 0; i < types.Length; i++)
                labels[i + 1] = $"{types[i].Name} ({types[i].FullName})";
        }

        private static int FindLabelIndex(Type type, Type[] types)
        {
            // Null type corresponds to the "none" option
            if (type is null)
                return 0;

            int index = Array.IndexOf(types, type);

            // Label index is one greater than type index.
            // If for whatever reason the type is not found, index will be -1 and this addition will return 0 ("none" option).
            return index + 1;
        }

        private Type DataType => ((DataContainer)target).DataType;

        private void OnEnable()
        {
            GetAvailableDataTypes(out m_types, out m_labels);
            m_index = FindLabelIndex(DataType, m_types);

            m_dataProperty = serializedObject.FindProperty("m_data");
        }

        // This iterates child properties at depth 1 (no child of child).
        // TODO: Maybe move this to another class, maybe even extract out into another project.
        private static IEnumerable<SerializedProperty> EnumerateChildProperties(SerializedProperty parent)
        {
            var copy = parent.Copy();
            var depth = copy.depth + 1;

            while (copy.Next(true))
            {
                if (copy.depth == depth)
                    yield return copy;
            }
        }

        // TODO: This needs more testing with more complex data types.
        private static void DrawDataRegion(SerializedProperty property)
        {
            GUILayout.BeginVertical("GroupBox");

            foreach (var dataProperty in EnumerateChildProperties(property))
            {
                EditorGUILayout.PropertyField(dataProperty);
            }

            GUILayout.EndVertical();
        }

        private static void DrawNoTypeInfoBox()
        {
            // TODO: Add some info to this message about how to make new struct types work with the system.
            EditorGUILayout.HelpBox(
                $"You must select a data type from the dropdown menu above before you can modify serialized data.",
                MessageType.Info,
                true);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Data Type", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();

            m_index = EditorGUILayout.Popup(label: s_dataTypeLabel, selectedIndex: m_index, m_labels);

            if (EditorGUI.EndChangeCheck())
            {
                if (m_index < 1)
                {
                    var script = MonoScript.FromScriptableObject(ScriptableObject.CreateInstance<DataContainer>());
                    ScriptUtility.SetScriptType(serializedObject, script);
                }
                else if (m_index <= m_types.Length)
                {
                    int typeIndex = m_index - 1;
                    var selectedDataType = m_types[typeIndex];

                    if (ScriptableDataUtilities.TryFindScriptForDataContainer(selectedDataType, out MonoScript script))
                    {
                        ScriptUtility.SetScriptType(serializedObject, script);
                    }
                    else
                    {
                        Logger.LogError($"Failed to find MonoScript for scriptable data with data type {selectedDataType}.");
                    }
                }

                // Return early to avoid errors.
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Serialized Data", EditorStyles.boldLabel);

            if (m_dataProperty != null)
            {
                DrawDataRegion(m_dataProperty);
            }
            else
            {
                DrawNoTypeInfoBox();
            }
        }
    }
}
