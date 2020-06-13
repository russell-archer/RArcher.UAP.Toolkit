using System;
using System.Collections.Generic;

namespace RArcher.UAP.Toolkit.Common
{
    /// <summary>Abstracts the interface for ViewModelStateHelper</summary>
    public interface IStateHelper
    {
        /// <summary>Get an object from the state store using the specified key</summary>
        /// <param name="key">The key for the state store value</param>
        /// <returns>Returns a value from the state store as an object</returns>
        object this[string key] { get; set; }

        /// <summary>The state store that holds state for a specific view model</summary>
        Dictionary<string, object> State { get; set; }

        /// <summary>Returns true if state for the view model exists, false otherwise</summary>
        bool StateExists { get; }

        /// <summary>Get an object of type T from the state store using the specified key</summary>
        /// <typeparam name="T">The type of the value specified by the key</typeparam>
        /// <param name="key">The key for the state store value</param>
        /// <returns>Returns a value from the state store</returns>
        T Get<T>(string key);

        /// <summary>Get a string value from the state store using the specified key</summary>
        /// <param name="key">The key for the state store value</param>
        /// <param name="defaultValue">The value to return if the key doesn't exist</param>
        /// <returns>Returns a value from the state store</returns>
        string Get(string key, string defaultValue = "");

        /// <summary>Get a double value from the state store using the specified key</summary>
        /// <param name="key">The key for the state store value</param>
        /// <param name="defaultValue">The value to return if the key doesn't exist</param>
        /// <returns>Returns a value from the state store</returns>
        double Get(string key, double defaultValue = 0);

        /// <summary>Get a bool value from the state store using the specified key</summary>
        /// <param name="key">The key for the state store value</param>
        /// <param name="defaultValue">The value to return if the key doesn't exist</param>
        /// <returns>Returns a value from the state store</returns>
        bool Get(string key, bool defaultValue = false);

        /// <summary>Returns true if the selected state store contains the specified key, false otherwise</summary>
        /// <param name="key">The key for the state store value</param>
        /// <returns>Returns true if the selected state store contains the specified key, false otherwise</returns>
        bool ContainsKey(string key);

        /// <summary>Get a DateTime value from the state store using the specified key</summary>
        /// <param name="key">The key for the state store value</param>
        /// <param name="defaultValue">The DateTime to return if the key doesn't exist</param>
        /// <returns>Returns a DateTime from the state store</returns>
        DateTime Get(string key, DateTime defaultValue);

        /// <summary>Save an object to the state store using the specified key</summary>
        /// <param name="key">The key for the state store value</param>
        /// <param name="value">The value to save to the state store</param>
        void Set(string key, object value);
    }
}