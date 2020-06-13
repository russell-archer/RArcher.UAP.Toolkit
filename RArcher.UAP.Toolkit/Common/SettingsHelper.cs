using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace RArcher.UAP.Toolkit.Common
{
    public class SettingsHelper : IStateHelper
    {
        // Properties ---------------------------------------------------------

        /// <summary>Get an object from settings using the specified key</summary>
        /// <param name="key">The key for the settings value</param>
        /// <returns>Returns a value from settings as an object</returns>
        public object this[string key]
        {
            get { return State[key]; }
            set { State[key] = value; }
        }

        /// <summary>The state store (settings in this case)</summary>
        public Dictionary<string, object> State { get; set; }

        /// <summary>The kind of settings store in use (local or roaming)</summary>
        public SettingsStore Store { get; }

        /// <summary>
        /// Returns true if the roaming settings store is used, false indicates local settings are being used
        /// </summary>
        public bool SettingsAreRoaming { get { return Store == SettingsStore.Roaming; }}

        // Private members ----------------------------------------------------

        private readonly IPropertySet _settingsStore;  // Can be either local or roaming settings store

        // Methods ------------------------------------------------------------

        /// <summary>Constructor for SettingsHelper</summary>
        /// <param name="store">The origin of the settings store to use</param>
        public SettingsHelper(SettingsStore store)
        {
            Store = store;

            _settingsStore = store == SettingsStore.Local ? 
                ApplicationData.Current.LocalSettings.Values : 
                ApplicationData.Current.RoamingSettings.Values;

            State = _settingsStore.ToDictionary(k => k.Key, v => v.Value);
        }

        /// <summary>Returns true if settings exist, false otherwise</summary>
        public bool StateExists { get { return State.Count > 0; }}

        /// <summary>Get an object of type T from settings using the specified key</summary>
        /// <typeparam name="T">The type of the value specified by the key</typeparam>
        /// <param name="key">The key for the settings value</param>
        /// <returns>Returns a value from settings</returns>
        public T Get<T>(string key)
        {
            // If the setting doesn't exist, create it with a default value
            if(!_settingsStore.ContainsKey(key)) _settingsStore[key] = default(T);
            return (T)State[key];
        }

        /// <summary>Get a string value from settings using the specified key</summary>
        /// <param name="key">The key for the settings value</param>
        /// <param name="defaultValue">The value to return if the key doesn't exist</param>
        /// <returns>Returns a value from settings</returns>
        public string Get(string key, string defaultValue = "")
        {
            // If the setting doesn't exist, create it with a default value
            if(!State.ContainsKey(key)) State[key] = defaultValue;
            return State[key] as string;
        }

        /// <summary>Get a double value from settings using the specified key</summary>
        /// <param name="key">The key for the settings value</param>
        /// <param name="defaultValue">The value to return if the key doesn't exist</param>
        /// <returns>Returns a value from settings</returns>
        public double Get(string key, double defaultValue = 0)
        {
            // If the setting doesn't exist, create it with a default value
            if(!State.ContainsKey(key)) State[key] = defaultValue;
            return (double)State[key];
        }

        /// <summary>Get a bool value from settings using the specified key</summary>
        /// <param name="key">The key for the settings value</param>
        /// <param name="defaultValue">The value to return if the key doesn't exist</param>
        /// <returns>Returns a value from settings</returns>
        public bool Get(string key, bool defaultValue = false)
        {
            // If the setting doesn't exist, create it with a default value
            if(!State.ContainsKey(key)) State[key] = defaultValue;
            return (bool)State[key];
        }

        /// <summary>Returns true if settings contains the specified key, false otherwise</summary>
        /// <param name="key">The key for the settings value</param>
        /// <returns>Returns true if settings contains the specified key, false otherwise</returns>
        public bool ContainsKey(string key) { return State.ContainsKey(key); }


        /// <summary>Get a DateTime value from settings using the specified key</summary>
        /// <param name="key">The key for the settings value</param>
        /// <param name="defaultValue">The DateTime to return if the key doesn't exist</param>
        /// <returns>Returns a DateTime from settings</returns>
        public DateTime Get(string key, DateTime defaultValue)
        {
            // If the setting doesn't exist, create it with a default value
            if(!State.ContainsKey(key)) State[key] = defaultValue;
            return (DateTime)State[key];
        }

        /// <summary>Save an object to settings using the specified key</summary>
        /// <param name="key">The key for the settings value</param>
        /// <param name="value">The value to save to settings</param>
        public void Set(string key, object value) { State[key] = value; }

    }

    /// <summary>Enum giving possible options for the type of settings store</summary>
    public enum SettingsStore { Local, Roaming }
}