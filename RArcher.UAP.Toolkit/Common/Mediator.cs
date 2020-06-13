using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace RArcher.UAP.Toolkit.Common
{
    /// <summary>Helper class that can be used to send messages between view models</summary>
    /// <example>
    /// 
    /// In MainViewModel:
    /// 
    ///     :
    ///     Mediator.Register("OurCommonValue", MyCallback);
    ///     :
    ///     public void MyCallback(object o)
    ///     {
    ///         Debug.WriteLine("MainViewModel.MyCallback = {0}", o);  // Displays 99
    ///     }
    /// 
    /// In SettingsViewModel:
    /// 
    ///     private void DoSendMessageCommand(object obj)
    ///     {
    ///         Mediator.SendMessage("OurCommonValue", 99);
    ///     }
    /// 
    /// </example>
    static public class Mediator
    {
        static readonly IDictionary<string, List<Action<object>>> Dictionary = new Dictionary<string, List<Action<object>>>();

        /// <summary>Registers a message key and callback for a listener</summary>
        /// <param name="key">The key used to identify the message</param>
        /// <param name="callback">The delegate to register</param>
        public static bool Register(string key, Action<object> callback)
        {
            try
            {
                if(!CheckParams(key, callback)) return false;
                if(!Dictionary.ContainsKey(key)) Dictionary.Add(key, new List<Action<object>> {callback});
                else
                {
                    foreach(var item in Dictionary[key].Where(item => item.GetMethodInfo().Name != callback.GetMethodInfo().Name))
                        Dictionary[key].Add(callback);
                }

                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Mediator.Register: Error register callback for message key '{0}': {1}", key, ex);
                return false;
            }
        }

        /// <summary>Unregisters a message</summary>
        /// <param name="key">The key used to identify the message</param>
        /// <param name="callback">The delegate to unregister</param>
        public static bool Unregister(string key, Action<object> callback)
        {
            try
            {
                if(!CheckParams(key, callback)) return false;
                if(!Dictionary.ContainsKey(key)) return false;
                
                Dictionary[key].Remove(callback);
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Mediator.Unregister: Error unregistering callback for message key '{0}': {1}", key, ex);
                return false;
            }
        }

        /// <summary>Sends a message all registered listeners of a message key</summary>
        /// <param name="key">The key used to identify the message</param>
        /// <param name="args">Arguments to be passed with the message</param>
        public static bool SendMessage(string key, object args)
        {
            try
            {
                if(!CheckParams(key)) return false;
                if(!Dictionary.ContainsKey(key)) return false;

                foreach(var callback in Dictionary[key]) callback(args);
                return true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Mediator.SendMessage: Error sending message to callbacks for message key '{0}': {1}", key, ex);
                return false;
            }
        }

        private static bool CheckParams(string key, Action<object> callback)
        {
            if(string.IsNullOrEmpty(key))
            {
                Debug.WriteLine("Mediator.CheckParams: key is null");
                return false;
            }

            if(callback != null) return true;
            Debug.WriteLine("Mediator.CheckParams: callback is null");
            return false;
        }

        private static bool CheckParams(string key)
        {
            if(!string.IsNullOrEmpty(key)) return true;
            Debug.WriteLine("Mediator.CheckParams: key is null");
            return false;
        }
    }
}