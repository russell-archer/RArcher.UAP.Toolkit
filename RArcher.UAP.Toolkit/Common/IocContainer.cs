// Example of how to register a type and a concrete implementation:
//   IocContainer.Container.RegisterType<IMainViewModel>().To<MainViewModel>().InSingletonScope();
//
// Example of how to resolve a type to an instance of it:
//   var mainViewModel = IocContainer.Get<IMainViewModel>();

namespace RArcher.UAP.Toolkit.Common
{
    /// <summary>
    /// Inversion of Control container. Used to facilitate loose coupling between services 
    /// and dependent objects
    /// </summary>
    public static class IocContainer
    {
        /// <summary>The actual Ioc object</summary>
        public static readonly SimpleIoc Container = new SimpleIoc();

        /// <summary>
        /// Gets an instance of a concrete implementation of the specified interface. 
        /// The actual type returned depends on the binding which has been established 
        /// between the interface and a concrete type.
        /// </summary>
        /// <typeparam name="T">The interface for the required type</typeparam>
        /// <returns>Returns an instance of the concrete implementation of the specified interface</returns>
        public static T Get<T>() where T : class
        {
            return Container.Get<T>();
        }
    }
}