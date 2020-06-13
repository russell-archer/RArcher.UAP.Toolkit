using System;

namespace RArcher.UAP.Toolkit.Common
{
    /// <summary>AutoState attribute</summary>
    /// <remarks>
    /// 
    /// * Your view model should derive from ViewModelBase. This provides base methods 
    ///   that work with PersistentStateHelper to enable the saving/loading of properties 
    ///   to/from persistent (disk) storage. ViewModelBase also defines the abstract methods 
    ///   SaveState() and LoadState(), which you must override in your own view model. 
    ///   Normally, these overridden methods will simply contain calls to base class
    ///   SaveState() and LoadState() methods
    /// 
    /// * Any view model wishing to auto-save/load its properties must hook into
    ///   the main Save/Load mechanism that is triggered when the View handles 
    ///   the OnNavigatedTo/OnNavigatedFrom events. When these events are handled, 
    ///   the View should call the ViewModel's SaveState() and LoadState() methods.
    ///   The ViewModel SaveState() and LoadState() methods should then call the
    ///   ViewModelBase SaveAutoState()/LoadAutoState() methods:
    /// 
    ///     + View XAML:
    /// 
    ///         <Page.DataContext>
    ///             <Binding Source="{StaticResource ViewModelLocator}" Path="MainViewModel" />
    ///         </Page.DataContext>
    /// 
    ///     + View C#:
    /// 
    ///         public sealed partial class MainView : Page
    ///         {
    ///             public IMainViewModel ViewModel { get; set; }
    ///         
    ///             public MainView()
    ///             {
    ///                 this.InitializeComponent();
    ///         
    ///                 // Get a reference to our view model, which has been set in XAML
    ///                 ViewModel = this.DataContext as IMainViewModel;
    ///                 if(ViewModel == null) throw new NullReferenceException();
    ///             }
    ///             
    ///             protected override void OnNavigatedTo(NavigationEventArgs e)
    ///             {
    ///                 ViewModel.LoadState();
    ///             }
    ///         
    ///             protected override void OnNavigatedFrom(NavigationEventArgs e)
    ///             {
    ///                 ViewModel.SaveState();
    ///             }
    ///         }
    /// 
    ///     + The ViewModel:
    /// 
    ///         public class MainViewModel : ViewModelBase, IMainViewModel, INotifyPropertyChanged
    ///         {
    ///             [AutoState]
    ///             public string HelloMsg
    ///             {
    ///                 get { return _helloMsg; }
    ///                 set { _helloMsg = value; OnPropertyChanged(); }
    ///             }
    ///         
    ///             private string _helloMsg;
    ///         }
    /// 
    /// * You may provide default values for properties in your view model as in the 
    ///   following examples: 
    ///     
    ///     [AutoState(DefaultValue: -1)]
    ///     public int MyInt  { get; set; }
    /// 
    ///     [AutoState(DefaultValue: "Hello World")]
    ///     public string MyString { get; set; }
    /// 
    /// * You may specifiy how you want to deal with null values through the use
    ///   the SaveNullValues and RestoreNullValues parameters. For example:
    /// 
    ///     [AutoState(SaveNullValues = true, RestoreNullValues = true)]
    /// 
    /// * If a property is a collection of a custom type, that type must implement
    ///   the ISerialize interface, which provides two methods Serialize() and 
    ///   Deserialize(). These methods are called by ViewModelBase during the 
    ///   save/load process to restore themselves from a flattened (serialized) 
    ///   string representation of the object
    ///   
    /// * Properties decorated with [AutoState] will be persisted and restored from 
    ///   persistent storage as follows:
    /// 
    ///     + Handled automatically:
    ///         - Fundamental types (string, int, bool, etc.)
    ///         - Collections of fundametal types (e.g. string, int, etc.). 
    ///         - Custom types with properties of fundamental type
    ///         - Custom types with a mix of custom and fundamental properties
    /// 
    ///     + Handled with help from ISerialize
    ///         - Generic collections (e.g. List, ObservableCollection) of custom type
    /// 
    ///     + Not handled:
    ///         - RelayCommand types
    ///         - Arrays (implement as a collection)
    /// </remarks>  
    [AttributeUsage(AttributeTargets.Property)]
    public class AutoState : Attribute, IAutoAttribute
    {
        /// <summary>
        /// The default value for a property if the state store does not contain an 
        /// entry for the property
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>True if you want null values to be saved, false otherwise</summary>
        public bool SaveNullValues { get; set; }

        /// <summary>True if you want null values to be restored, false otherwise</summary>
        public bool RestoreNullValues { get; set; }

        /// <summary>
        /// The [AutoState] attribute is used to mark properties for automatic state save/restore 
        /// through the ViewModelBase.SaveAutoState() and ViewModelBase.LoadAutoState() methods
        /// </summary>
        public AutoState()
        {
        }

        /// <summary>
        /// The [AutoState] attribute is used to mark properties for automatic state save/load 
        /// through the ViewModelBase.SaveAutoState() and ViewModelBase.LoadAutoState() methods
        /// </summary>
        /// <param name="defaultValue">
        /// The default value for a property if the state store does not contain an entry 
        /// for the property
        /// </param>
        public AutoState(object defaultValue)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// The [AutoState] attribute is used to mark properties for automatic state save/load 
        /// through the ViewModelBase.SaveAutoState() and ViewModelBase.LoadAutoState() methods
        /// </summary>
        /// <param name="saveNullValues">Set to true if you want null property values to be saved</param>
        public AutoState(bool saveNullValues)
        {
            SaveNullValues = saveNullValues;
        }

        /// <summary>
        /// The [AutoState] attribute is used to mark properties for automatic state save/load 
        /// through the ViewModelBase.SaveAutoState() and ViewModelBase.LoadAutoState() methods
        /// </summary>
        /// <param name="saveNullValues">Set to true if you want null property values to be saved</param>
        /// <param name="restoreNullValues">Set to true if you want null values to be loaded</param>
        public AutoState(bool saveNullValues, bool restoreNullValues)
        {
            SaveNullValues = saveNullValues;
            RestoreNullValues = restoreNullValues;
        }

        /// <summary>
        /// The [AutoState] attribute is used to mark properties for automatic state save/load 
        /// through the ViewModelBase.SaveAutoState() and ViewModelBase.LoadAutoState() methods
        /// </summary>
        /// <param name="defaultValue">
        /// The default value for a property if the state store does 
        /// not contain an entry for the property
        /// </param>
        /// <param name="saveNullValues">Set to true if you want null property values to be saved</param>
        /// <param name="restoreNullValues">Set to true if you want null values to be loaded</param>
        public AutoState(object defaultValue, bool saveNullValues, bool restoreNullValues)
        {
            DefaultValue = defaultValue;
            SaveNullValues = saveNullValues;
            RestoreNullValues = restoreNullValues;
        }
    }
}