using System;
using System.ComponentModel;

namespace RArcher.UAP.Toolkit.Common
{
    public interface IModelBase
    {
        /// <summary>PropertyChanged event. Raised when any property changes</summary>
        event PropertyChangedEventHandler PropertyChanged;

        /// <summary>Saves an object to the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <param name="value">The value to save</param>
        void SetStateItem(string key, object value);

        /// <summary>Gets an object from the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <returns>Returns the object with the specified key, or null if it doesn't exist in state</returns>
        object GetStateItem(string key);

        /// <summary>Gets an object from the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <param name="defaultValue">The default value to return if the key doesn't exist</param>
        /// <returns>Returns the object with the specified key, or null if it doesn't exist in state</returns>
        object GetStateItem(string key, object defaultValue);

        /// <summary>Gets an object of type T from the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <returns>Returns the object of type T with the specified key, or the default value of T if it doesn't exist in state</returns>
        T GetStateItem<T>(string key);

        /// <summary>Gets an object of type T from the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <param name="defaultValue">The default value to return if the key doesn't exist</param>
        /// <returns>Returns the object of type T with the specified key, or the default value of T if it doesn't exist in state</returns>
        T GetStateItem<T>(string key, T defaultValue);

        /// <summary>Saves state for all properties marked with an attribute that implements IAutoAttribute</summary>
        void SaveAuto<T>() where T : Attribute, IAutoAttribute;

        /// <summary>Loads state for all properties marked with an attribute that implements IAutoAttribute</summary>
        void LoadAuto<T>() where T : Attribute, IAutoAttribute;         
    }
}