using System;
using System.Diagnostics;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace RArcher.UAP.Toolkit.Command
{
    /// <summary>SelectionEventToCommand</summary>
    public class SelectionEventToCommand
    {
        /// <summary>Get the ICommand object</summary>
        public static ICommand GetCommand(DependencyObject obj) { return (ICommand)obj.GetValue(CommandProperty); }

        /// <summary>Set the ICommand object</summary>
        public static void SetCommand(DependencyObject obj, ICommand value) { obj.SetValue(CommandProperty, value); }

        /// <summary>CommandProperty</summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(SelectionEventToCommand),
            new PropertyMetadata(null, OnPropertyChanged));

        /// <summary>Raises the PropertyChanged event</summary>
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var selector = d as Selector;
            if (selector == null) return;

            try
            {
                // Hook into the relevant event...
                selector.SelectionChanged += (sender, args) =>
                {
                    var sel = (Selector) sender;
                    var command = sel.GetValue(SelectionEventToCommand.CommandProperty) as ICommand;
                    var param = sel.GetValue(SelectionEventToCommand.CommandParameterProperty);

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
            typeof(SelectionEventToCommand),
            new PropertyMetadata(null));         
    }
}