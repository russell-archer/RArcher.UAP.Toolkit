namespace RArcher.UAP.Toolkit.Common
{
    /// <summary>Interface defining the base class for view models</summary>
    public interface IViewModelBase
    {
        /// <summary>Saves state for all properties marked with the [AutoState] attribute for the class that inherits from ViewModelBase</summary>
        void SaveState();

        /// <summary>Loads state for all properties marked with an [AutoState] attribute</summary>
        void LoadState();
    }
}