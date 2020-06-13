using System;
using System.Collections.Generic;

namespace RArcher.UAP.Toolkit.Common
{
    /// <summary>
    /// Provides helper methods to get/set data in the global state cache maintained by PersistenStateHelper.
    /// In general, view models should not access global state directly. Instead, use ViewModelStateHelper
    /// to ensure that only state belonging to the particular view model is read/written 
    /// </summary>
    public class ViewModelStateHelper : IStateHelper
    {
        private Dictionary<string, object> _state = PersistentStateHelper.GlobalStateCache;
        private readonly string _viewModelName;

        /// <summary>Get an object from global state using the specified key</summary>
        /// <param name="key">The key for the global state value</param>
        /// <returns>Returns a value from global state as an object (null if the key doesn't exist)</returns>
        public object this[string key]
        {
            get { return _state[key]; }
            set { _state[key] = value; }
        }

        /// <summary>The global state dictionary that holds state for all view models</summary>
        public Dictionary<string, object> State
        {
            get { return _state;  }
            set { _state = value; }
        }

        /// <summary>Constructor</summary>
        /// <param name="viewModel">The model (must implement IViewModelBase) that will use ViewModelStateHelper</param>
        public ViewModelStateHelper(IViewModelBase viewModel)
        {
            if(viewModel == null) throw new ArgumentNullException();
            _viewModelName = viewModel.GetType().Name;
        }

        /// <summary>Returns true if state for the view model exists, false otherwise</summary>
        public bool StateExists { get { return PersistentStateHelper.ViewModelStateExists(_viewModelName); } }

        /// <summary>Get an object of type T from global state using the specified key</summary>
        /// <typeparam name="T">The type of the value specified by the key</typeparam>
        /// <param name="key">The key for the global state value</param>
        /// <returns>Returns a value from global state</returns>
        public T Get<T>(string key)
        {
            // If the setting doesn't exist, create it with a default value
            if(!_state.ContainsKey(key)) _state[key] = default(T); 
            return (T)_state[key];
        }

        /// <summary>Get a string value from global state using the specified key</summary>
        /// <param name="key">The key for the global state value</param>
        /// <param name="defaultValue">The value to return if the key doesn't exist</param>
        /// <returns>Returns a value from global state</returns>
        public string Get(string key, string defaultValue = "")
        {
            // If the setting doesn't exist, create it with a default value
            if(!_state.ContainsKey(key)) _state[key] = defaultValue;
            return _state[key] as string;
        }

        /// <summary>Get a double value from global state using the specified key</summary>
        /// <param name="key">The key for the global state value</param>
        /// <param name="defaultValue">The value to return if the key doesn't exist</param>
        /// <returns>Returns a value from global state</returns>
        public double Get(string key, double defaultValue = 0)
        {
            // If the setting doesn't exist, create it with a default value
            if (!_state.ContainsKey(key)) _state[key] = defaultValue;
            return (double)_state[key];
        }

        /// <summary>Get a bool value from global state using the specified key</summary>
        /// <param name="key">The key for the global state value</param>
        /// <param name="defaultValue">The value to return if the key doesn't exist</param>
        /// <returns>Returns a value from global state</returns>
        public bool Get(string key, bool defaultValue = false)
        {
            // If the setting doesn't exist, create it with a default value
            if (!_state.ContainsKey(key)) _state[key] = defaultValue;
            return (bool)_state[key];
        }

        /// <summary>Returns true if global state contains the specified key, false otherwise</summary>
        /// <param name="key">The key for the global state value</param>
        /// <returns>Returns true if global state contains the specified key, false otherwise</returns>
        public bool ContainsKey(string key) { return _state.ContainsKey(key); }

        /// <summary>Get a DateTime value from global state using the specified key</summary>
        /// <param name="key">The key for the global state value</param>
        /// <param name="defaultValue">The DateTime to return if the key doesn't exist</param>
        /// <returns>Returns a DateTime from global state</returns>
        public DateTime Get(string key, DateTime defaultValue)
        {
            // If the setting doesn't exist, create it with a default value
            if (!_state.ContainsKey(key)) _state[key] = defaultValue;
            return (DateTime)_state[key];
        }

        /// <summary>Save an object to global state using the specified key</summary>
        /// <param name="key">The key for the global state value</param>
        /// <param name="value">The value to save</param>
        public void Set(string key, object value) { _state[key] = value; }
    }
}