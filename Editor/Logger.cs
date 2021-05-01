namespace JakePerry.Unity.ScriptableData
{
    /// <summary>
    /// Helper class that appends package name to logged messages.
    /// </summary>
    internal static class Logger
    {
        private static string FormatLogMessage(string message)
        {
            return $"[ScriptableData] {message}";
        }

        internal static void Log(string message)
        {
            UnityEngine.Debug.Log(FormatLogMessage(message));
        }

        internal static void Log(string message, UnityEngine.Object context)
        {
            UnityEngine.Debug.Log(FormatLogMessage(message), context);
        }

        internal static void LogError(string message)
        {
            UnityEngine.Debug.LogError(FormatLogMessage(message));
        }

        internal static void LogError(string message, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogError(FormatLogMessage(message), context);
        }
    }
}
