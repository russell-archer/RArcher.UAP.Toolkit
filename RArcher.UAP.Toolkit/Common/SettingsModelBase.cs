namespace RArcher.UAP.Toolkit.Common
{
    public abstract class SettingsModelBase : ModelBase, ISettingsModelBase
    {
        protected SettingsModelBase() : this(SettingsStore.Roaming) {}
        protected SettingsModelBase(SettingsStore store)
        {
            // The settings state id
            StateSavedId = "GlobalSettings.";

            // Create the default state helper that will load/save this model's state
            StateHelper = new SettingsHelper(store);
        }

        /// <summary>Loads state for all properties marked with an [AutoSetting] attribute</summary>
        public virtual void LoadState()
        {
            LoadAuto<AutoSetting>();
        }

        /// <summary>Saves state for all properties marked with the [AutoSetting] attribute</summary>
        public virtual void SaveState()
        {
            SaveAuto<AutoSetting>();
        }
    }
}