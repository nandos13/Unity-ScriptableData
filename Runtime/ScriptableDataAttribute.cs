using System;

namespace JakePerry.Unity.ScriptableData
{
    // This one goes on a struct directly
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public sealed class ScriptableDataAttribute : Attribute { }
}
