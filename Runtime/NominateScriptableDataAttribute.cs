using System;

namespace JakePerry.Unity.ScriptableData
{
    // This one goes on assembly
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public sealed class NominateScriptableDataAttribute : Attribute
    {
        private readonly Type m_type;
        private readonly string m_typeName;
        private readonly bool m_useTypeName;

        public NominateScriptableDataAttribute(Type type)
        {
            m_type = type;
            m_typeName = null;
            m_useTypeName = false;
        }

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
