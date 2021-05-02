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

        // TODO: This needs more testing with more complex data types.
        private static void DrawDataRegion(SerializedProperty property)
        {
            GUILayout.BeginVertical("GroupBox");

            foreach (var dataProperty in SerializedObjectUtilities.EnumerateChildProperties(property))
            {
                EditorGUILayout.PropertyField(dataProperty);
            }

            GUILayout.EndVertical();
        }

        private static void DrawNoTypeInfoBox()
        {
            EditorGUILayout.HelpBox(
                $"You must select a data type from the dropdown menu above before you can modify serialized data.\nUse the {nameof(ScriptableDataAttribute)} attribute to mark a struct as a serializable data type.",
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
                    SerializedObjectUtilities.SetScriptType(serializedObject, script);
                }
                else if (m_index <= m_types.Length)
                {
                    int typeIndex = m_index - 1;
                    var selectedDataType = m_types[typeIndex];

                    if (ScriptableDataUtilities.TryFindScriptForDataContainer(selectedDataType, out MonoScript script))
                    {
                        SerializedObjectUtilities.SetScriptType(serializedObject, script);
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
