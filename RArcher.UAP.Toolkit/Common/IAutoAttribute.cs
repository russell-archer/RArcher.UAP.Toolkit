namespace RArcher.UAP.Toolkit.Common
{
    /// <summary>IAutoAttribute interface</summary>
    public interface IAutoAttribute
    {
        /// <summary>
        /// The default value for a property if the store does not contain an 
        /// entry for the property
        /// </summary>
        object DefaultValue { get; set; }

        /// <summary>True if you want null values to be saved, false otherwise</summary>
        bool SaveNullValues { get; set; }

        /// <summary>True if you want null values to be restored, false otherwise</summary>
        bool RestoreNullValues { get; set; }     
    }
}