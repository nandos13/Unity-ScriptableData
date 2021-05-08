namespace JakePerry.Unity.ScriptableData
{
    /// <summary>
    /// This class is responsible for triggering auto-generation of missing data container
    /// classes upon script recompilation.
    /// </summary>
    internal static class AutoGenerateOnCompile
    {
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnReloadScripts()
        {
            if (Settings.AutoGenerateContainerClasses)
            {
                // TODO: If Settings class is turned into an editor window, change this message. (see Settings todo comment.)
                Logger.Log(
                    $"Script reload is triggering automatic generation of missing types. " +
                    $"This can be disabled via the settings window ({MenuItems.OPEN_SETTINGS_ITEM}), " +
                    $"or via the {nameof(Settings)}.{nameof(Settings.AutoGenerateContainerClasses)} property."
                    );

                DataContainerGenerator.GenerateMissingTypes();
            }
        }
    }
}
