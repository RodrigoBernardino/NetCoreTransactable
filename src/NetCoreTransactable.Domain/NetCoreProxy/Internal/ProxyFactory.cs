using Castle.DynamicProxy;
using NetCoreTransactable.Domain.NetCoreProxy.Configuration;
using System;

namespace NetCoreTransactable.Domain.NetCoreProxy.Internal
{
    internal static class ProxyFactory
    {
        /// <summary>
        /// Creates a Interface Proxy Object of the object passed in
        /// </summary>
        /// <param name="serviceProvider">Services Collection</param>
        /// <param name="proxyGenerator">Proxy Generator Instance</param>
        /// <param name="proxyConfiguration">Proxy Configuration</param>
        /// <param name="originalObject">The Object to be Proxied</param>
        /// <returns>The Proxied Object</returns>
        public static T CreateInterfaceProxy<T>(IServiceProvider serviceProvider, IProxyGenerator proxyGenerator,
            CoreProxyConfiguration proxyConfiguration, T originalObject)
        {
            // Proxy the Original Object
            var masterInterceptor = new CoreInterceptor(serviceProvider, proxyConfiguration);
            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget(typeof(T), originalObject, masterInterceptor);
            // Make sure the Proxy was created correctly
            if (proxy == null)
                throw new ArgumentNullException(nameof(proxy));

            // Return the Proxied Object
            return (T)proxy;
        }

        /// <summary>
        /// Creates a Interface Proxy Object of the object passed in
        /// </summary>
        /// <param name="serviceProvider">Services Collection</param>
        /// <param name="proxyGenerator">Proxy Generator Instance</param>
        /// <param name="proxyConfiguration">Proxy Configuration</param>
        /// <param name="serviceType">The interface Type</param>
        /// <param name="originalObject">The Object to be Proxied</param>
        /// <returns>The Proxied Object</returns>
        public static object CreateInterfaceProxy(IServiceProvider serviceProvider, IProxyGenerator proxyGenerator,
            CoreProxyConfiguration proxyConfiguration, Type serviceType, object originalObject)
        {
            // Proxy the Original Object
            var masterInterceptor = new CoreInterceptor(serviceProvider, proxyConfiguration);
            var proxy = proxyGenerator.CreateInterfaceProxyWithTarget(serviceType, originalObject, masterInterceptor);
            // Make sure the Proxy was created correctly
            if (proxy == null)
                throw new ArgumentNullException(nameof(proxy));

            // Return the Proxied Object
            return proxy;
        }

        /// <summary>
        /// Creates a Class Proxy Object of the object passed in
        /// </summary>
        /// <param name="serviceProvider">Services Collection</param>
        /// <param name="proxyGenerator">Proxy Generator Instance</param>
        /// <param name="proxyConfiguration">Proxy Configuration</param>
        /// <returns>The Proxied Object</returns>
        [Obsolete("This don't work with classes that doesn't have a parameterless constructor", true)]
        public static T CreateClassProxy<T>(IServiceProvider serviceProvider, IProxyGenerator proxyGenerator,
            CoreProxyConfiguration proxyConfiguration, T originalObject)
        {
            // Proxy the Original Object
            var masterInterceptor = new CoreInterceptor(serviceProvider, proxyConfiguration);
            var proxy = proxyGenerator.CreateClassProxyWithTarget(typeof(T), originalObject, masterInterceptor);
            // Make sure the Proxy was created correctly
            if (proxy == null)
                throw new ArgumentNullException(nameof(proxy));

            // Return the Proxied Object
            return (T)proxy;
        }
    }
}
