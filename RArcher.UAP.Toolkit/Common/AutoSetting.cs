using System;

namespace RArcher.UAP.Toolkit.Common
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoSetting : Attribute, IAutoAttribute
    {
        /// <summary>
        /// The default value for a property if the settings store does not contain an 
        /// entry for the property
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>True if you want null values to be saved, false otherwise</summary>
        public bool SaveNullValues { get; set; }

        /// <summary>True if you want null values to be restored, false otherwise</summary>
        public bool RestoreNullValues { get; set; }

        /// <summary>
        /// The [AutoSetting] attribute is used to mark properties for automatic save/load 
        /// through the ViewModelBase.SaveAutoState() and ViewModelBase.LoadAutoState() methods
        /// </summary>
        public AutoSetting() {}

        /// <summary>
        /// The [AutoSetting] attribute is used to mark properties for automatic save/load 
        /// through the ViewModelBase.SaveAutoState() and ViewModelBase.LoadAutoState() methods
        /// </summary>
        /// <param name="defaultValue">
        /// The default value for a property if the settings store does not contain an entry 
        /// for the property
        /// </param>
        public AutoSetting(object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// The [AutoSetting] attribute is used to mark properties for automatic save/load 
        /// through the ViewModelBase.SaveAutoState() and ViewModelBase.LoadAutoState() methods
        /// </summary>
        /// <param name="saveNullValues">Set to true if you want null property values to be saved</param>
        public AutoSetting(bool saveNullValues)
        {
            SaveNullValues = saveNullValues;
        }

        /// <summary>
        /// The [AutoSetting] attribute is used to mark properties for automatic save/load 
        /// through the ViewModelBase.SaveAutoState() and ViewModelBase.LoadAutoState() methods
        /// </summary>
        /// <param name="saveNullValues">Set to true if you want null property values to be saved</param>
        /// <param name="restoreNullValues">Set to true if you want null values to be loaded</param>
        public AutoSetting(bool saveNullValues, bool restoreNullValues)
        {
            SaveNullValues = saveNullValues;
            RestoreNullValues = restoreNullValues;
        }

        /// <summary>
        /// The [AutoSetting] attribute is used to mark properties for automatic save/load 
        /// through the ViewModelBase.SaveAutoState() and ViewModelBase.LoadAutoState() methods
        /// </summary>
        /// <param name="defaultValue">
        /// The default value for a property if the settings store does 
        /// not contain an entry for the property
        /// </param>
        /// <param name="saveNullValues">Set to true if you want null property values to be saved</param>
        /// <param name="restoreNullValues">Set to true if you want null values to be loaded</param>
        public AutoSetting(object defaultValue, bool saveNullValues, bool restoreNullValues)
        {
            DefaultValue = defaultValue;
            SaveNullValues = saveNullValues;
            RestoreNullValues = restoreNullValues;
        }
    }
}