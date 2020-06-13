namespace RArcher.UAP.Toolkit.Common
{
    /// <summary>Base class for view models</summary>
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
    public abstract class ViewModelBase : ModelBase, IViewModelBase
    {
        /// <summary>Constructor</summary>
        protected ViewModelBase()
        {
            // This will use the name of the class that inherits from ViewModelBase to be the
            // key we use to define if state has been saved for this object. So individual
            // state items will have keys like "LocationService.TrackLocation"
            StateSavedId = GetType().Name + ".";

            // Create the default state helper that will load/save this model's state
            StateHelper = new ViewModelStateHelper(this);
        }

        /// <summary>Loads state for all properties marked with an [AutoState] attribute</summary>
        public virtual void LoadState()
        {
            LoadAuto<AutoState>();
        }

        /// <summary>Saves state for all properties marked with the [AutoState] attribute for the class that inherits from ViewModelBase</summary>
        public virtual void SaveState()
        {
            SaveAuto<AutoState>();
        }
    }
}