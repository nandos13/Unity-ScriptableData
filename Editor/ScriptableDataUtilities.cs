using JakePerry.Reflection;
using System;
using System.Collections.Generic;
using UnityEditor;

using static JakePerry.Reflection.ReflectionUtility;

namespace JakePerry.Unity.ScriptableData
{
    public static class ScriptableDataUtilities
    {
        public static HashSet<Type> GetAllDataTypes()
        {
            var types = new HashSet<Type>();

            // Get data types nominated by an attribute directly on the type declaration.
            var typeDecorations = AttributeUtility.EnumerateDecorations<ScriptableDataAttribute>(
                targets: EnumerateAllTypes(),
                filter: provider => provider is Type type && type.IsValueType);

            foreach (var decoration in typeDecorations)
            {
                types.Add(decoration.Target as Type);
            }

            // Get data types nominated by an assembly attribute.
            var assemblyDecorations = AttributeUtility.EnumerateDecorations<NominateScriptableDataAttribute>(GetAssemblies());

            foreach (var decoration in assemblyDecorations)
            {
                var attribute = (NominateScriptableDataAttribute)decoration.CustomAttribute;
                if (attribute.TryGetIdentifiedType(out Type type) && type != null && type.IsValueType)
                {
                    types.Add(type);
                }
            }

            return types;
        }

        public static bool TryFindDataContainerType(Type dataType, out Type containerType)
        {
            _ = dataType ?? throw new ArgumentNullException(nameof(dataType));

            if (!dataType.IsValueType)
                throw new ArgumentException("Type must be a value type.", nameof(dataType));

            var genericContainerType = typeof(DataContainer<>).MakeGenericType(new Type[] { dataType });

            // Because the DataContainer<T> restricts T to a struct (value type), there should never be more
            // than one type assignable to the generic definition DataContainer<dataType>. Therefore,
            // it's safe to return the first element accessed via a foreach loop.
            foreach (var assignableType in GetAssignableTypes(genericContainerType))
            {
                containerType = assignableType;
                return true;
            }

            containerType = null;
            return false;
        }

        public static bool TryFindScriptForDataContainer(Type dataType, out MonoScript script)
        {
            if (TryFindDataContainerType(dataType, out Type containerType))
            {
                return ScriptUtility.TryFindScript(containerType, out script);
            }

            script = null;
            return false;
        }

        public static HashSet<Type> GetMissingDataTypes()
        {
            var hashset = GetAllDataTypes();

            // Remove all data types where a corresponding DataContainer<T> class implementation exists.
            hashset.RemoveWhere(dataType => TryFindDataContainerType(dataType, out _));

            return hashset;
        }
    }
}
