using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RArcher.UAP.Toolkit.Common
{
    /// <summary>Provides helper methods to load/save data to/from the persistent state store</summary>
    public static class PersistentStateHelper
    {
        private const string _stateFileName = "AppState.xml";
        private const string _navigationStateKey = "__global.NavigationState";
        private static Dictionary<string, object> _globalStateCache;

        /// <summary>
        /// The name of the file used to store app state. 
        /// The file is stored in Windows.Storage.ApplicationData.Current.LocalFolder
        /// (e.g. C:\Users\username\AppData\Local\Packages\ae9258d9-3fcf-4151-9859-5e6bcc44a108_bb0zrez7bb02g\LocalState)
        /// </summary>
        public static string StateFileName { get { return _stateFileName; }}

        /// <summary>
        /// A dictionary of key/value pairs that represent app-wide state.
        /// The dictionary should not be accessed directly, but rather using ViewModelStateHelper, 
        /// which will allow access only to the appropriate view model's state
        /// </summary>
        public static Dictionary<string, object> GlobalStateCache
        {
            get { return _globalStateCache ?? (_globalStateCache = new Dictionary<string, object>()); }
            set { _globalStateCache = value; }
        }

        /// <summary>Returns true if state for the specified view model exists, false otherwise</summary>
        public static bool ViewModelStateExists(string viewModelName)
        {
            var searchTerm = viewModelName + ".";
            return GlobalStateCache.Any(stateItem => stateItem.Key.StartsWith(searchTerm));
        }

        /// <summary>
        /// Restore navigation state and navigate to the previous current view. If that fails,
        /// navigate to the default initial view. This method would normally be called in
        /// App.OnLaunched. See example
        /// </summary>
        /// <example>
        /// protected async override void OnLaunched(LaunchActivatedEventArgs e)
        /// {
        ///     InitializeIocBindings();
        /// 
        ///     var rootFrame = Window.Current.Content as Frame;
        ///     if (rootFrame == null)
        ///     {
        ///         rootFrame = new Frame {CacheSize = 2};
        ///         if(e.PreviousExecutionState != ApplicationExecutionState.Running && e.PreviousExecutionState != ApplicationExecutionState.Suspended)
        ///             await PersistentStateHelper.LoadAsync();
        /// 
        ///         Window.Current.Content = rootFrame;
        ///     }
        /// 
        ///     if (rootFrame.Content == null) PersistentStateHelper.RestoreNavigationState(rootFrame, typeof(MainView), e.Arguments);
        ///     Window.Current.Activate();
        /// }
        /// </example>
        /// <param name="rootFrame">The app's root frame. See OnLaunched in App.xaml.cs</param>
        /// <param name="defaultView">The default initial view to navigate to</param>
        /// <param name="launchArgs">Launch args. See LaunchActivatedEventArgs in OnLaunched in App.xaml.cs</param>
        public static void RestoreNavigationState(
            Frame rootFrame, 
            Type defaultView = null, 
            string launchArgs = "")
        {
            try
            {
                if(rootFrame == null) throw new ArgumentNullException(nameof(rootFrame));

                if(GlobalStateCache.ContainsKey(_navigationStateKey))
                {
                    // Restore navigation state. This results in the previous current view
                    // being navigated to and its OnNavigatedTo event handler being called
                    var navState = GlobalStateCache[_navigationStateKey] as string;
                    if(!string.IsNullOrEmpty(navState))
                    {
                        rootFrame.SetNavigationState(navState);
                        return; 
                    }
                }

                // We couldn't restore navigation state, so just restore the default view
                if(defaultView == null) throw new ArgumentNullException(nameof(defaultView));

                if(!rootFrame.Navigate(defaultView, launchArgs))
                    throw new Exception("Unable to navigate to initial view " + defaultView.Name);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("PersistentStateHelper.RestoreNavigationState: Error restoring navigation state: {0}", ex);
            } 
        }

        /// <summary>Loads state for all view models from persistent storage</summary>
        /// <returns>Returns true if state was loaded OK, false otherwise</returns>
        public static async Task<bool> LoadAsync()
        {
            try
            {
                StorageFile stateFile;

                try
                {
                    // Get the input stream for the SessionState file (which may not exist)
                    stateFile = await ApplicationData.Current.LocalFolder.GetFileAsync(StateFileName);
                }
                catch(FileNotFoundException)
                {
                    // Unable to load state because the state file doesn't exist in the local store. 
                    Debug.WriteLine("PersistentStateHelper.LoadAsync: State file doesn't exist");
                    return false;
                }
                catch(Exception ex)
                {
                    Debug.WriteLine("PersistentStateHelper.LoadAsync: Error loading state from persistent state store: {0}", ex);
                    return false;
                }

                using (var inStream = await stateFile.OpenSequentialReadAsync())
                {
                    var dataContractSerializer = new DataContractSerializer(typeof(Dictionary<string, object>));

                    // Load state for all view models into a single dictionary
                    GlobalStateCache = (Dictionary<string, object>)dataContractSerializer.ReadObject(inStream.AsStreamForRead());
                    return true;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("PersistentStateHelper.LoadAsync: Error loading state from persistent store: {0}", ex);
            }            
            return false;            
        }

        /// <summary>Saves state for all view models to persistent storage</summary>
        /// <returns>Returns true if state was saved OK, false otherwise</returns>
        public static async Task<bool> SaveAsync()
        {
            try
            {
                var currentView = Window.Current.Content as Frame;
                if(currentView != null)
                {
                    // Get navigation state info, this also triggers a call to the 
                    // current view's OnNavigatedFrom handler
                    var navState = currentView.GetNavigationState();

                    // Save the current navigation state (which includes info on the current view)
                    GlobalStateCache[_navigationStateKey] = navState;
                }
                else Debug.WriteLine("PersistentStateHelper.SaveAsync: Unable to get reference to current view");

                var stateStream = new MemoryStream();
                var dataContractSerializer = new DataContractSerializer(typeof(Dictionary<string, object>));
                dataContractSerializer.WriteObject(stateStream, GlobalStateCache);

                var stateFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(StateFileName, CreationCollisionOption.ReplaceExisting);
                using (var stream = await stateFile.OpenStreamForWriteAsync())
                {
                    stateStream.Seek(0, SeekOrigin.Begin);
                    await stateStream.CopyToAsync(stream);
                }

                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("PersistentStateHelper.SaveAsync: Error saving state cache state to persistent store: {0}", ex);
            }

            return false;
        }
    }
}