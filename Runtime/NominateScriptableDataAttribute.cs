using System;

namespace JakePerry.Unity.ScriptableData
{
    /// <summary>
    /// This attribute is defined at an assembly-level to nominate a given struct
    /// type as a scriptable data type. Doing so is the same as declaring a
    /// <see cref="ScriptableDataAttribute"/> directly on the struct type declaration itself.
    /// </summary>
    /// <remarks>
    /// This attribute can be used to nominate a type without requiring direct access to the
    /// type's source code. For example, this attribute can be used to register
    /// the <see cref="UnityEngine.Vector2"/> type as a scriptable data type.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class NominateScriptableDataAttribute : Attribute
    {
        private readonly Type m_type;
        private readonly string m_typeName;
        private readonly bool m_useTypeName;

        /// <summary>
        /// Nominates the given type as a scriptable data type.
        /// </summary>
        /// <param name="type">The struct data type to nominate.</param>
        public NominateScriptableDataAttribute(Type type)
        {
            m_type = type;
            m_typeName = null;
            m_useTypeName = false;
        }

        /// <summary>
        /// Nominates the type described by <paramref name="typeName"/> as a scriptable data type.
        /// </summary>
        /// <param name="typeName">The name of the struct data type to nominate.</param>
        /// <remarks>
        /// Note: The <see cref="Type.GetType(string)"/> method is used to resolve a type from the
        /// name string. As such, the assembly qualified name should be used in most cases.
        /// </remarks>
        public NominateScriptableDataAttribute(string typeName)
        {
            m_type = null;
            m_typeName = typeName;
            m_useTypeName = true;
        }

        public bool TryGetIdentifiedType(out Type type)
        {
            type = null;

            if (m_useTypeName)
            {
                if (!string.IsNullOrEmpty(m_typeName))
                {
                    type = Type.GetType(typeName: m_typeName, throwOnError: false, ignoreCase: false);

                    if (type is null)
                    {
                        type = Type.GetType(typeName: m_typeName, throwOnError: false, ignoreCase: true);
                    }
                }
            }
            else
            {
                type = m_type;
            }

            return type != null;
        }
    }
}
