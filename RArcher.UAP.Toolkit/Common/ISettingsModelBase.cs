namespace RArcher.UAP.Toolkit.Common
{
    public interface ISettingsModelBase
    {
        /// <summary>Saves state for all properties marked with the [AutoSettings] attribute</summary>
        void SaveState();

        /// <summary>Loads state for all properties marked with an [AutoSettings] attribute</summary>
        void LoadState();         
    }
}