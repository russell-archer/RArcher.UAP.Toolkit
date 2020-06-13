namespace RArcher.UAP.Toolkit.Common
{
    /// <summary>
    /// The ISerialize interface is used by custom objects which are part of arrays or collections as it allows
    /// their properties to be flattened (for saving) and rehydrated (for loading) from application state storage.
    /// </summary>
    public interface ISerialize
    {
        /// <summary>Flattens an instance of the object to a string that can be saved to app state storage</summary>
        /// <returns>Returns a flattened instance of the object that can be saved to app state storage</returns>
        string Serialize();

        /// <summary>Repopulates the object from a flattened string representation of its properties</summary>
        /// <param name="sObject">A flat string representation of the object's properties</param>
        /// <returns>Returns an instance of the object if its properties were successfully rehydrated from a flattened string representation</returns>
        object Deserialize(string sObject);
    }
}