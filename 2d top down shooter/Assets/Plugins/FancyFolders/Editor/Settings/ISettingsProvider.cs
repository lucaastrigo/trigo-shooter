namespace FancyFolders.Editor.Settings
{
    public interface ISettingsProvider<out TSettings>
        where TSettings : BaseAssetSettings
    {
        TSettings Settings { get; }
    }
}
