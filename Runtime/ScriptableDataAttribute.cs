using System;

namespace JakePerry.Unity.ScriptableData
{
    /// <summary>
    /// This attribute is used to nominate the target struct type as a scriptable data type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class ScriptableDataAttribute : Attribute { }
}
