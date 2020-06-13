using System;
using System.Diagnostics;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace RArcher.UAP.Toolkit.Command
{
    /// <summary>Maps a tap event to a RelayCommand</summary>
    public static class TapEventToCommand
    {
        /// <summary>Get the ICommand object</summary>
        public static ICommand GetCommand(DependencyObject obj) { return (ICommand)obj.GetValue(CommandProperty); }

        /// <summary>Set the ICommand object</summary>
        public static void SetCommand(DependencyObject obj, ICommand value) { obj.SetValue(CommandProperty, value); }

        /// <summary>CommandProperty</summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(TapEventToCommand),
            new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>Raises the PropertyChanged event</summary>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = d as FrameworkElement;
            if (frameworkElement == null) return;

            try
            {
                // Hook into the relevant event...
                frameworkElement.Tapped += (sender, args) =>
                {
                    var fe = (FrameworkElement) sender;
                    var command = fe.GetValue(TapEventToCommand.CommandProperty) as ICommand;
                    var param = fe.GetValue(TapEventToCommand.CommandParameterProperty);

                    // Call the ViewModel method that registered to handle this command...
                    if(command != null && command.CanExecute(param)) command.Execute(param);
                };
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);                
            }
        }

        /// <summary>CommandParameter attached property</summary>
        public static object GetCommandParameter(DependencyObject obj) { return obj.GetValue(CommandParameterProperty); }

        /// <summary>Command parameter</summary>
        public static void SetCommandParameter(DependencyObject obj, object value) { obj.SetValue(CommandParameterProperty, value); }

        /// <summary>Command paramter dependency property</summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(TapEventToCommand),
            new PropertyMetadata(null));
    }
}