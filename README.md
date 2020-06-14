# RArcher.UAP.Toolkit
Framework used in the creation of WinRT-based Windows and Windows Phone apps.

Archived Visual Studio 2015 project that was previously held in Team Foundation Server.

As of May 21st 2016, support for this toolkit has been discontinued.

## What is it?

RArcher.WinRT.Toolkit is an experimental lightweight toolkit for creating Universal WinRT 8.1 apps, based on the MVVM design pattern.

The toolkit was originally designed for WP8 @Html.ActionLink("(RArcher.Phone.Toolkit)", "Archive", "Toolkit") as a set of reusable classes for use with the roundup app, with the intention being that these classes be generically reusable in future WP8 projects. After the initial release of roundup, I decided to refactor all the reusable stuff into a collection of toolkit packages which became RArcher.Phone.Toolkit.*. With the release of WinRT 8.1 I refactored the toolkit and renamed it RArcher.WinRT.Toolkit.

An obvious question at this point is, "Why don't you use MVVM Light, Caliburn Micro, or a similar well-established MVVM framework?" The answer is simple: I want the knowledge and insights to be gained from creating my own (simple) MVVM framework.

So, is my toolkit a "better" alternative to MVVM Light? No, definitely not! If you want an industry-standard MVVM-enabling toolkit for WinRT projects, you should go for MVVM Light.

However, by creating RArcher.Phone.Toolkit and RArcher.WinRT.Toolkit, I did gain a better understanding of what it takes to create MVVM-based apps for Windows 8.1 and Windows Phone 8.1. And the toolkit also provides some useful things I haven't seen elsewhere.

I've found maintaining app state to be an error-prone chore. The toolkit provides classes which enables view models and other models to automatically save/restore properties as and when required. For example, all you need to do is decorate a view model property with the [AutoState] attribute and ViewModelBase and ModelBase take care of all the load/save mechanics. I did a write-up on the principles of the mechanism previously. In version 1.3-onwards, the toolkit also supports the automatic management of global settings.

using RArcher.WinRT.Toolkit.Common;

namespace UniversalMvvmApp1.ViewModel
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        [AutoState(DefaultValue = "Hello WinRT World!")]
        public string HelloMsg
        {
            get { return _helloMsg; }
            set { _helloMsg = value; OnPropertyChanged(); }
        }

        private string _helloMsg;
    }
}
Where do I get it?

The toolkit consists of the following packages, which are available via NuGet:

RArcher.WinRT.Toolkit - Available now
RArcher.WinRT.Toolkit.Push - Soon: Abstraction of Microsoft push notification services
RArcher.WinRT.Toolkit.Location - Soon: Abstraction of Microsoft Location services
RArcher.WinRT.Toolkit.Network - Soon: Network-related classes
RArcher.WinRT.Toolkit.Store - Soon: Windows and Windows Phone store-based classes
For example, to install the core toolkit package, simply open the Package Manager Console in Visual Studio and type:

install-package RArcher.WinRT.Toolkit -Pre
If you're working on a Universal app, make sure you add the packages to both the Windows and Windows Phone project by using the Default project dropdown in the Package Manager Console.

However, the easiest way to get started with the toolkit is by using the RArcher.WinRT.Toolkit New Project template for Visual Studio 2013.

How to use the RArcher.WinRT.Toolkit New Project template for Visual Studio 2013

Installing the template

Close Visual Studio, then download the New Project Template and save the zip (don't unzip it) to the following directory - if the diretory structure doesn't exist, create it:

C:\Users\user\Documents\Visual Studio 2013\Templates\ProjectTemplates\Visual C#\Store Apps\Universal Apps
Creating a new Universal App using the RArcher.WinRT.Toolkit template

Re-start Visual Studio 2013 and create a new project using the template. It will be found in Visual C# > Store Apps > Universal Apps:


The first thing to is build the solution, as this will automatically get the RArcher.WinRT.Toolkit and Microsoft.Practices.Unity nuget packages.

Updating Package.appmanifest

Although at this point the solution will build correctly, neither the Phone or Windows apps will deploy properly as we need to update their Package.appmanifest files.

First, open the Windows project Package.appxmanifest file:

On the Application tab, update the Display name, Entry point and Description fields:

On the Packaging tab:
Update Package name by adding a valid GUID.
This can be generated using Visual Studio Tools > Create GUID. Updating the package name will also automatically update the Package family name field

Update Package display name
Update the Publisher field by clicking Choose Certificate.
Create your own publisher certificate via the "Configure Certificate..." dropdown (i.e. you don't want to use the temporary default certificate I created if you intend to publish the app to the store)

Now repeat the process (except for the certification configuration) for the Phone app's Package.appmanifest

Solution structure


Notice that the Shared project contains App.xaml and App.xaml.cs. It also has a folder containing view model classes for two views (MainViewModel.cs and View2ViewModel.cs), along with a ViewModelLocator class.

The Windows and Windows Phone projects just contain two views each. In this example, the two sets of views are identical in both projects.

The following diagram shows at a high-level some of the main parts of the app:


In App.xaml we create an instance of system-wide resource of type ViewModelLocator with the key "ViewModelLocator". This object is rather like a "lookup table" as it provides public properties for each view model in the app. It makes use of the toolkit's IocContainer to return an instance of the required view model (more on this in a minute). Each view sets its view model as the data context. This means that controls in view can data bind with properties in the view model. In addition, the view's code behind can access the data context, allowing it to call LoadState() and SaveState() in the view model to participate in the auto-state mechanism.

App.xaml.cs

Let's take a closer look at what happens in App.xaml.cs:

namespace UniversalMvvmApp1
{
    public sealed partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
        }

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Initialize the IoC container type bindings
            InitializeIocBindings();

            var rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame {CacheSize = 2};

                // Restore the saved session state only when appropriate. If the app's previous
                // execution state was Running or Suspended then there's no need to load state 
                // as all app state will have been preserved in-memory. Therefore, state is loaded 
                // when the previous execution state is either Terminated, ClosedByUser or NotRunning

                if( e.PreviousExecutionState != ApplicationExecutionState.Running && 
                    e.PreviousExecutionState != ApplicationExecutionState.Suspended)
                {
                    try
                    {
                        await PersistentStateHelper.LoadAsync();
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine("PersistentStateHelper was unable to load state: {0}", ex);
                    }
                }

                // Place the frame in the current Window.
                Window.Current.Content = rootFrame;
            }

            // Restore navigation state and navigate to the previous current view. If that fails,
            // navigate to the default initial view specified
            if (rootFrame.Content == null) 
                PersistentStateHelper.RestoreNavigationState(rootFrame, typeof(MainView), e.Arguments);

            // Ensure the current window is active.
            Window.Current.Activate();
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await PersistentStateHelper.SaveAsync();
            deferral.Complete();
        }

        private void InitializeIocBindings()
        {
            IocContainer.Container.RegisterType(
                typeof(IMainViewModel),
                typeof(MainViewModel),
                null,
                new ContainerControlledLifetimeManager());

            IocContainer.Container.RegisterType(
                typeof(IView2ViewModel),
                typeof(View2ViewModel),
                null,
                new ContainerControlledLifetimeManager());

            // Register more views and other types here
        }
    }
}
First, we override the OnLaunched handler and hook into the Suspending event. In OnLaunched we setup our IoC bindings (the mappings between interfaces and their concrete implementations). The IocContainer class in RArcher.WinRT.Toolkit provides a simple encapsulation of the Microsoft Unity inversion of control container.

Note that we configure all our view models to have a ContainerControlledLifetimeManager. This essentially means the IoC container will create a singleton object (single instance) for each view model and return a reference to it each time.

In OnLaunched an async call is made to PersistentStateHelper.LoadAsync(). This loads app state, including the navigation state, from persistent storage (an xml file in the app's LocalState folder) into an in-memory cache. We then call PersistentStateHelper.RestoreNavigationState() to restore the app's previous navigation state and current view. If there was no previous current view, or there's an error attemtping to navigate to it, a default view is navigated to.

In OnSuspending an async call is made to PersistentStateHelper.SaveAsync(). This takes the contents of the in-memory state cache and saves it to persistent storage. Notice how we get a deferral to prevent the await statement causing the method to return (and the app to suspend) before we get a chance to complete to state save process. Once the save is complete we signal the deferral as complete and the app suspends.

App Lifecycle

The following diagram shows how the toolkit fits into the app lifecycle to facilitate the automatic load and save of state:


Although it looks complex, essentially the auto-state mechanism works like this:

When the app is launched global app state is loaded from disk into a cache held by PersistentStateHelper
When a view is navigated to, its OnNavigatedTo() method is called. It calls LoadState(), which is a virtual method inherited from ViewModelBase.
This method in turn calls ModelBase.LoadAuto<AutoState>(), which uses reflection to inspect the view model's public properties.

For each property decorated with the [AutoState] attribute, the ViewModelStateHelper held by ViewModelBase is used to get a state value from the global cache and write it to the property:


The class hirearchy of ViewModel->ViewModelBase->ModelBase allows for flexibility so that, for example, ModelBase can load both state and gloabl settings (as we'll see later) using the same code. By using a common interface (IStateHelper), and having ModelBase code to the interface, we can easily "plug-in" any kind of state helper we like
When a view is navigated from (e.g. when suspending or navigating to another view), its OnNavigatedFrom() method is called. It then calls SaveState() in the view model (which is a virtual method inherited from ViewModelBase). For each property decorated with the [AutoState] attribute, ViewModelStateHelper is used to write the property's value to the global cache
When the app is suspended global app state is written to disk from the cache held by PersistentStateHelper
The view model

Each view model has a similar structure. And in the example app I've shared the view models between both the Windows and Windows Phone projects. Here's what MainViewModel.cs looks like:

namespace UniversalMvvmApp1.ViewModel
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        [AutoState(DefaultValue = "Hello WinRT World!")]
        public string HelloMsg
        {
            get { return _helloMsg; }
            set { _helloMsg = value; OnPropertyChanged(); }
        }

        private string _helloMsg;
    }
}
Notice the clean-looking code: there's no "plumbing" to load and save state. If required, you can override the base class ViewModelBase.LoadState() and ViewModelBase.SaveState() methods if you want to hook into the auto-state mechanism to do other things. If you do, make sure to call the base class:

public override void LoadState()
{
    // Do custom on-load work here
    // :

    base.LoadState();  // Call the base class to load state
}
MainView.xaml

The XAML for MainView looks as shown below. The two key things to note are:

The view's data context is set to the MainViewModel object returned by the ViewModelLocator
We data bind directly to objects in the view model. In this example, the Text property of the TextBox is bound to the HelloMsg property in MainViewModel.cs
<Page
    x:Class="UniversalMvvmApp1.View.MainView"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
    xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
    mc:Ignorable="d"
    Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">

    <Page.DataContext>
        <Binding Source="{StaticResource ViewModelLocator}" Path="MainViewModel" />
    </Page.DataContext>

    <Grid Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
        <StackPanel>
            <TextBox 
                Text="{Binding HelloMsg, Mode=TwoWay}" 
                Margin="20"
                FontSize="32"
                Foreground="Red"/>

            <Button 
                Content="Goto View2"
                Margin="20" 
                Click="OnGotoView2"/>
        </StackPanel>
    </Grid>
</Page>
MainView.xaml.cs

The code-behind for MainView.xaml is as below. Notice how we access our view's data context (which was set in XAML). This allows us to call ViewModel.LoadState() and ViewModel.SaveState() in the OnNavigatedTo() and OnNavigatedFrom() handlers:

namespace UniversalMvvmApp1.View
{
    public sealed partial class MainView : Page
    {
        public IMainViewModel ViewModel { get; set; }

        public MainView()
        {
            this.InitializeComponent();

            // Get a reference to our view model, which has been set in XAML
            ViewModel = this.DataContext as IMainViewModel;
            if(ViewModel == null) throw new NullReferenceException();
        }
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.LoadState();  // Load ViewModel state
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.SaveState();  // Save ViewModel state
        }

        private void OnGotoView2(object sender, RoutedEventArgs e)
        {
            // Notice that here I have a button click event handler in the view, rather than
            // a RelayCommand in the ViewModel. This is intentional: navigation between
            // views is the responsibility of a view, not the view model. The view model
            // will be automatically informed of the navigation via its SaveState() handler

            if(Frame.CanGoForward) Frame.GoForward();
            else Frame.Navigate(typeof(View2));
        }
    }
}
Support for Global Settings

The following provides an overview of how the toolkit supports the automatic load and save of settings:


The auto-settings mechanism works in a very similar manner to that of auto-state. Here, we have a settings model that inherits from SettingsModelBase (which itself inherits from ModelBase). It creates a SettingsHelper that implements the IStateHelper interface so that ModelBase can access local or roaming settings. As with the auto-state mechanism, all the real work is done by ModelBase. Any property in the settings model that is decorated with the [AutoSetting] attribute is automatically loaded/saved as required.

Here's what the simple settings model in the sample UniversalMvvmApp1 app looks like:

namespace UniversalMvvmApp1.Model
{
    public class GlobalSettingsModel : SettingsModelBase, IGlobalSettingsModel
    {
        [AutoSetting(DefaultValue = "en.us")]
        public string LanguagePref
        {
            get { return _languagePref; }
            set { _languagePref = value; OnPropertyChanged(); }
        }

        private string _languagePref;    
    }
}
And we hook the auto-settings mechanism into the app lifecycle using the following code in App.xaml.cs (some code unrelated to the auto-settings mechanism has been removed for clarity):

namespace UniversalMvvmApp1
{
    public sealed partial class App : Application
    {
        // App-wide accessible model that gives r/w access to the settings store
        public static IGlobalSettingsModel Settings { get; set; }

        protected async override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Initialize the IoC container type bindings
            InitializeIocBindings();

            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame {CacheSize = 2};

                if( e.PreviousExecutionState != ApplicationExecutionState.Running && 
                    e.PreviousExecutionState != ApplicationExecutionState.Suspended)
                {
                    try
                    {
                        // Get the model object responsible for loading/saving global app settings
                        Settings = IocContainer.Get<IGlobalSettingsModel>();

                        // Load settings from persistent storage into model
                        Settings.LoadState();  

                        await PersistentStateHelper.LoadAsync();  
                    }
                    catch(Exception ex)
                    {
                    }
                }

                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null) 
                PersistentStateHelper.RestoreNavigationState(rootFrame, typeof(MainView), e.Arguments);

            Window.Current.Activate();
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            // Write state from cache to persistent storage
            await PersistentStateHelper.SaveAsync();

            // Write settings model to persistent storage
            Settings.SaveState();  

            deferral.Complete();
        }

        private void InitializeIocBindings()
        {
            :
            :

            // Register the global settings model type as a singleton
            IocContainer.Container.RegisterType(
                typeof(IGlobalSettingsModel),
                typeof(GlobalSettingsModel),
                null,
                new ContainerControlledLifetimeManager());

            // Register more view models and other types here...
        }
    }
}
