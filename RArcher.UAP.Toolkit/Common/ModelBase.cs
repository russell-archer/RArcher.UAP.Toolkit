using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace RArcher.UAP.Toolkit.Common
{
    /// <summary>Base class for all model types, including ViewModelBase and SettingsModelBase</summary>
    public class ModelBase : IModelBase, INotifyPropertyChanged
    {
        /// <summary>PropertyChanged event. Raised when any property changes</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>The string used to prefix a state item's key, e.g. "MainViewModel.", "GlobalSettings.", etc.</summary>
        public string StateSavedId
        {
            get
            {
                if(string.IsNullOrEmpty(_stateSavedId)) throw new NullReferenceException("ModelBase.StateSavedId cannot be null");
                return _stateSavedId;
            }
            set
            {
                if(string.IsNullOrEmpty(value)) throw new NullReferenceException("ModelBase.StateSavedId cannot be null");
                _stateSavedId = value;
            }
        }

        /// <summary>The helper object used to load/save state</summary>
        public IStateHelper StateHelper
        {
            get
            {
                if(_stateHelper == null) throw new NullReferenceException("ModelBase.StateHelper cannot be null");
                return _stateHelper;
            }
            set
            {
                if(value == null) throw new NullReferenceException("ModelBase.StateHelper cannot be null");
                _stateHelper = value;
            }
        }

        private string _stateSavedId;
        private IStateHelper _stateHelper;

        public ModelBase() {}
        public ModelBase(string stateSaveId, IStateHelper stateHelper)
        {
            StateSavedId = stateSaveId;
            StateHelper = stateHelper;
        }

        /// <summary>Saves an object to the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <param name="value">The value to save</param>
        public virtual void SetStateItem(string key, object value)
        {
            key = StateSavedId + key;
            StateHelper[StateSavedId] = true;
            StateHelper[key] = value;
        }

        /// <summary>Gets an object from the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <returns>Returns the object with the specified key, or null if it doesn't exist in state</returns>
        public virtual object GetStateItem(string key)
        {
            key = StateSavedId + key;
            return StateHelper.ContainsKey(key) ? StateHelper[key] : null;
        }

        /// <summary>Gets an object from the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <param name="defaultValue">The default value to return if the key doesn't exist</param>
        /// <returns>Returns the object with the specified key, or null if it doesn't exist in state</returns>
        public virtual object GetStateItem(string key, object defaultValue)
        {
            key = StateSavedId + key;
            return StateHelper.ContainsKey(key) ? StateHelper[key] : defaultValue;
        }

        /// <summary>Gets an object of type T from the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <returns>Returns the object of type T with the specified key, or the default value of T if it doesn't exist in state</returns>
        public virtual T GetStateItem<T>(string key)
        {
            key = StateSavedId + key;
            return StateHelper.ContainsKey(key) ? (T)StateHelper[key] : default(T);
        }

        /// <summary>Gets an object of type T from the state store</summary>
        /// <param name="key">The key for the state value</param>
        /// <param name="defaultValue">The default value to return if the key doesn't exist</param>
        /// <returns>Returns the object of type T with the specified key, or the default value of T if it doesn't exist in state</returns>
        public virtual T GetStateItem<T>(string key, T defaultValue)
        {
            key = StateSavedId + key;
            return StateHelper.ContainsKey(key) ? (T)StateHelper[key] : defaultValue;
        }

        /// <summary>Load state for all properties marked with attributes that implement IAutoAttribute</summary>
        public virtual void LoadAuto<T>() where T : Attribute, IAutoAttribute
        {
            try
            {
                var properties = GetType().GetRuntimeProperties();  // Get all the properties for the view model

                foreach(var pi in properties)
                {
                    if(pi == null) continue;

                    var ca = pi.GetCustomAttribute<T>();  // See if the property is marked with IAutoAttribute
                    if(ca == null || ca.GetType() == typeof(AutoNoAction))
                        continue;  // Property was not marked with the IAutoAttribute attribute, or explicitly marked as no action

                    var type = pi.PropertyType;
                    if(type == typeof(RelayCommand)) continue;  // No need to save this type of property
                    if(type.IsArray)
                    {
                        Debug.WriteLine("ModelBase.LoadAutoState: Unable to restore state for {0} in {1}", type.Name, StateSavedId);
                        Debug.WriteLine("ModelBase.LoadAutoState: Array properties cannot be auto-loaded. Consider using a collection instead");
                        continue;
                    }

                    var val = pi.GetValue(this);  // This can be null (e.g. a null ObservableCollection<T>)
                    var stateVal = StateHelper.StateExists ? GetStateItem(pi.Name) : null;  // Get the value from the state store (if it exists)

                    if(stateVal != null)
                    {
                        var typeIsCollection = type.FullName.ToLower().StartsWith("system.collections");
                        if(typeIsCollection)
                        {
                            // Restore a collection...

                            // First, split the flattened collection of items into individuals rows
                            var rows = stateVal.ToString().Split(new[] { ';' });
                            if(rows.Length == 0) continue;

                            // We now need to create an instance of the collection to receive the state we're going to load
                            var collection = (IList)Activator.CreateInstance(type);

                            // See if we're dealing with a generic collection (e.g. ObservableCollection<string> 
                            // or ObservableCollection<MyType>)
                            var genericArgs = type.GenericTypeArguments;

                            if(genericArgs == null)
                            {
                                Debug.WriteLine("ModelBase.LoadAutoState: Unable to restore state for {0} in {1}", type.Name, StateSavedId);
                                Debug.WriteLine("ModelBase.LoadAutoState: Can't determine the generic type of the collection");
                                continue;  // Couldn't work out the generic type
                            }

                            if(genericArgs.Length != 1)
                            {
                                Debug.WriteLine("ModelBase.LoadAutoState: Unable to restore state for {0} in {1}", type.Name, StateSavedId);
                                Debug.WriteLine("ModelBase.LoadAutoState: Too many generic collection types (LoadAuto<T>() only supports collections with a single generic type)");
                                continue;  // We only support collections with a single generic type
                            }

                            var genericCollectionType = genericArgs[0];

                            // Is the generic collection type a fundamental type (string, int, etc.) or a custom type?
                            if(TypeHelper.IsFundamentalType(genericCollectionType))
                            {
                                // It's a fundamental type 
                                // Is it a collection of strings?
                                if(genericCollectionType == typeof(string))
                                {
                                    foreach(var sRow in rows)
                                        if(sRow != null) collection.Add(sRow);
                                }
                                else
                                {
                                    // It's a collection of ints, floats, doubles, etc.
                                    foreach(var row in rows)
                                    {
                                        var collectionItem = TypeHelper.CastFundamentalTypeValue(row, genericCollectionType);
                                        if(collectionItem == null) continue; // Skip null value (we couldn't cast it)
                                        collection.Add(collectionItem);
                                    }
                                }
                            }
                            else
                            {
                                // It's a custom type. For each row we ask the type to provide a string representation of its value
                                foreach(var row in rows)
                                {
                                    // The custom generic collection type needs to implement ISerialize, if not
                                    // the following assignment will fail with an exception
                                    var collectionItem = (ISerialize)Activator.CreateInstance(genericCollectionType);
                                    if(collectionItem.Deserialize(row) == null) continue;  // The object didn't want the item added to the collection
                                    collection.Add(collectionItem);
                                }
                            }

                            pi.SetValue(this, collection);  // Restore the property using the stored state values
                        }
                        else
                        {
                            // Restore a scalar object...

                            if(val == null && !TypeHelper.IsFundamentalType(type)) val = Activator.CreateInstance(type);

                            var canRestoreItself = val as ISerialize;
                            if(canRestoreItself != null)
                            {
                                // This type wants to restore itself from a string
                                var rehydratedValue = canRestoreItself.Deserialize(stateVal.ToString());
                                pi.SetValue(this, rehydratedValue);
                            }
                            else pi.SetValue(this, stateVal); // The state helper knows how to restore this type from state
                        }
                    }
                    else
                    {
                        // The value in the state store is null - restore it to the property?
                        if(ca.RestoreNullValues && TypeHelper.TypeCanBeNull(type)) pi.SetValue(this, null);  // Restore a null to the property
                        else if(ca.DefaultValue != null) pi.SetValue(this, ca.DefaultValue);  // Restore a default value
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("ModelBase.LoadAutoState: Error restoring state for type {0}: {1} ", StateSavedId, ex);
            }
        }

        /// <summary>Saves state for all properties marked with an attribute that implements IAutoAttribute</summary>
        public virtual void SaveAuto<T>() where T : Attribute, IAutoAttribute
        {
            try
            {
                var properties = GetType().GetRuntimeProperties();

                foreach(var pi in properties)
                {
                    if(pi == null) continue;

                    var ca = pi.GetCustomAttribute<T>();
                    if(ca == null || ca.GetType() == typeof(AutoNoAction))
                        continue;  // Property was not marked with a IAutoAttribute attribute, or marked for no action

                    var type = pi.PropertyType;

                    if(type == typeof(RelayCommand)) continue;
                    if(type.IsArray)
                    {
                        Debug.WriteLine("ModelBase.SaveAutoState: Array properties cannot be auto-saved. Consider converting to a collection");
                        continue;
                    }

                    var val = pi.GetValue(this);
                    if(val == null)
                    {
                        // The object's null, if it has a default value, save that
                        if(ca.DefaultValue != null) SetStateItem(pi.Name, ca.DefaultValue);
                        else if(ca.SaveNullValues && TypeHelper.TypeCanBeNull(type)) SetStateItem(pi.Name, null);  // Or save a null if that's allowed
                        continue;  // Don't save the value
                    }

                    if(type.FullName.ToLower().StartsWith("system.collections"))
                    {
                        // Type is a collection - flatten all elements in it into a single string by treating 
                        // it as simple IEnumerable (which all collections should implement)
                        var collection = val as IEnumerable;
                        if(collection == null)
                        {
                            Debug.WriteLine("ModelBase.SaveAutoState: Cannot save {0}. Type does not implement IEnumerable", pi.Name);
                            continue;
                        }

                        var flattenedCollection = new StringBuilder();
                        foreach(var item in collection)
                        {
                            // Try to get the collection item to stringify itself (it needs to implement ISerialize),
                            // otherwise we use the item's ToString() value
                            var canSaveItself = item as ISerialize;
                            flattenedCollection.Append(canSaveItself != null ? canSaveItself.Serialize() : item.ToString());
                            flattenedCollection.Append(";");
                        }

                        var s = flattenedCollection.ToString();  // If the collection is empty, we save the property as an empty string
                        SetStateItem(pi.Name, s.TrimEnd(new[] { ';' }));  // e.g. "99|52|0|0|101;100|53|5|0|102;101|55|0|0|103;102|53|10|0|104"
                    }
                    else
                    {
                        var canSaveItself = val as ISerialize;
                        if(canSaveItself != null)
                        {
                            var flatValue = canSaveItself.Serialize();
                            SetStateItem(pi.Name, flatValue);
                        }
                        else SetStateItem(pi.Name, val);  // The type can be serialized by the state helper - save the value
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("ModelBase.SaveAutoState: Error saving state for {0}: {1}", StateSavedId, ex);
            }
        }

        /// <summary>Raises the PropertyChanged event</summary>
        /// <param name="propertyName">The name of the property that has changed, null to use the CallerMemberName attribute</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if(handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }         
    }
}