using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NetCoreTransactable.Domain.NetCoreProxy.Configuration;
using NetCoreTransactable.Domain.NetCoreProxy.Internal;
using System;

namespace NetCoreTransactable.Extensions
{
    /// <summary>
    /// Extension Methods for the <see cref="IServiceCollection"/>
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Enables Proxy Generation for Services registered in <see cref="IServiceCollection"/>
        /// </summary>
        /// <param name="services">Services Collection</param>
        /// <param name="options">Proxy Configuration Options</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection EnableCoreProxy(this IServiceCollection serviceCollection,
            Action<CoreProxyConfiguration> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            // Store the Proxy Configuration
            serviceCollection.Configure(options);

            // Proxy Generator needs to be registered as a Singleton for performance reasons
            serviceCollection.AddSingleton<IProxyGenerator, ProxyGenerator>();

            // Return the IServiceCollection for chaining configuration
            return serviceCollection;
        }

        /// <summary>
        /// Adds a Scoped Service to the <see cref="ServiceCollection"/> that is wrapped in a Proxy
        /// </summary>
        /// <typeparam name="TInterface">Interface Type</typeparam>
        /// <typeparam name="TService">Implementation Type</typeparam>
        /// <param name="serviceCollection">Services Collection</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddScopedWithProxy<TService, TImplementation>(
            this IServiceCollection serviceCollection)
            where TImplementation : TService
        {
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            CoreProxyConfiguration proxyConfiguration = serviceCollection.GetProxyConfiguration();
            IProxyGenerator proxyGenerator = serviceProvider.GetService<IProxyGenerator>();
            TImplementation proxyInstance = ActivatorUtilities.CreateInstance<TImplementation>(serviceProvider);

            serviceCollection.AddScoped(typeof(TService), p => ProxyFactory
                .CreateInterfaceProxy<TService>(serviceProvider, proxyGenerator, proxyConfiguration, proxyInstance));

            return serviceCollection;
        }

        /// <summary>
        /// Adds a Scoped Service to the <see cref="ServiceCollection"/> that is wrapped in a Proxy
        /// </summary>
        /// <param name="serviceType">Interface Type</typeparam>
        /// <param name="implementationType">Implementation Type</typeparam>
        /// <param name="serviceCollection">Services Collection</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddScopedWithProxy(
            this IServiceCollection serviceCollection, Type serviceType, Type implementationType)
        {
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            CoreProxyConfiguration proxyConfiguration = serviceCollection.GetProxyConfiguration();
            IProxyGenerator proxyGenerator = serviceProvider.GetService<IProxyGenerator>();
            var proxyInstance = ActivatorUtilities.CreateInstance(serviceProvider, implementationType);

            serviceCollection.AddScoped(serviceType, p => ProxyFactory
                .CreateInterfaceProxy(serviceProvider, proxyGenerator, proxyConfiguration, serviceType, proxyInstance));

            return serviceCollection;
        }

        /// <summary>
        /// Adds a Scoped Service to the <see cref="ServiceCollection"/> that is wrapped in a Proxy
        /// </summary>
        /// <typeparam name="TService">Implementation Type</typeparam>
        /// <param name="serviceCollection">Services Collection</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        [Obsolete("This don't work with classes that doesn't have a parameterless constructor", true)]
        public static IServiceCollection AddScopedWithProxy<TImplementation>(
            this IServiceCollection serviceCollection)
        {
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            CoreProxyConfiguration proxyConfiguration = serviceCollection.GetProxyConfiguration();
            IProxyGenerator proxyGenerator = serviceProvider.GetService<IProxyGenerator>();
            TImplementation proxyInstance = ActivatorUtilities.CreateInstance<TImplementation>(serviceProvider);

            serviceCollection.AddScoped(typeof(TImplementation), p => ProxyFactory
                .CreateClassProxy<TImplementation>(serviceProvider, proxyGenerator, proxyConfiguration, proxyInstance));

            return serviceCollection;
        }

        /// <summary>
        /// Adds a Transient Service to the <see cref="ServiceCollection"/> that is wrapped in a Proxy
        /// </summary>
        /// <typeparam name="TInterface">Interface Type</typeparam>
        /// <typeparam name="TService">Implementation Type</typeparam>
        /// <param name="serviceCollection">Services Collection</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddTransientWithProxy<TService, TImplementation>(
            this IServiceCollection serviceCollection)
            where TImplementation : TService
        {
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            CoreProxyConfiguration proxyConfiguration = serviceCollection.GetProxyConfiguration();
            IProxyGenerator proxyGenerator = serviceProvider.GetService<IProxyGenerator>();
            TImplementation proxyInstance = ActivatorUtilities.CreateInstance<TImplementation>(serviceProvider);

            serviceCollection.AddTransient(typeof(TService), p => ProxyFactory
                .CreateInterfaceProxy<TService>(serviceProvider, proxyGenerator, proxyConfiguration, proxyInstance));

            return serviceCollection;
        }

        /// <summary>
        /// Adds a Transient Service to the <see cref="ServiceCollection"/> that is wrapped in a Proxy
        /// </summary>
        /// <typeparam name="TService">Implementation Type</typeparam>
        /// <param name="serviceCollection">Services Collection</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        [Obsolete("This don't work with classes that doesn't have a parameterless constructor", true)]
        public static IServiceCollection AddTransientWithProxy<TImplementation>(
            this IServiceCollection serviceCollection)
        {
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            CoreProxyConfiguration proxyConfiguration = serviceCollection.GetProxyConfiguration();
            IProxyGenerator proxyGenerator = serviceProvider.GetService<IProxyGenerator>();
            TImplementation proxyInstance = ActivatorUtilities.CreateInstance<TImplementation>(serviceProvider);

            serviceCollection.AddTransient(typeof(TImplementation), p => ProxyFactory
                .CreateClassProxy<TImplementation>(serviceProvider, proxyGenerator, proxyConfiguration, proxyInstance));

            return serviceCollection;
        }

        /// <summary>
        /// Adds a Singleton Service to the <see cref="ServiceCollection"/> that is wrapped in a Proxy
        /// </summary>
        /// <typeparam name="TInterface">Interface Type</typeparam>
        /// <typeparam name="TService">Implementation Type</typeparam>
        /// <param name="serviceCollection">Services Collection</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        public static IServiceCollection AddSingletonWithProxy<TService, TImplementation>(
            this IServiceCollection serviceCollection)
            where TImplementation : TService
        {
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            CoreProxyConfiguration proxyConfiguration = serviceCollection.GetProxyConfiguration();
            IProxyGenerator proxyGenerator = serviceProvider.GetService<IProxyGenerator>();
            TImplementation proxyInstance = ActivatorUtilities.CreateInstance<TImplementation>(serviceProvider);

            serviceCollection.AddSingleton(typeof(TService), p => ProxyFactory
                .CreateInterfaceProxy<TService>(serviceProvider, proxyGenerator, proxyConfiguration, proxyInstance));

            return serviceCollection;
        }

        /// <summary>
        /// Adds a Singleton Service to the <see cref="ServiceCollection"/> that is wrapped in a Proxy
        /// </summary>
        /// <typeparam name="TService">Implementation Type</typeparam>
        /// <param name="serviceCollection">Services Collection</param>
        /// <returns><see cref="IServiceCollection"/></returns>
        [Obsolete("This don't work with classes that doesn't have a parameterless constructor", true)]
        public static IServiceCollection AddSingletonWithProxy<TImplementation>(
            this IServiceCollection serviceCollection)
        {
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            CoreProxyConfiguration proxyConfiguration = serviceCollection.GetProxyConfiguration();
            IProxyGenerator proxyGenerator = serviceProvider.GetService<IProxyGenerator>();
            TImplementation proxyInstance = ActivatorUtilities.CreateInstance<TImplementation>(serviceProvider);


            serviceCollection.AddSingleton(typeof(TImplementation), p => ProxyFactory
                .CreateClassProxy<TImplementation>(serviceProvider, proxyGenerator, proxyConfiguration, proxyInstance));

            return serviceCollection;
        }

        private static CoreProxyConfiguration GetProxyConfiguration(this IServiceCollection services)
        {
            return services.BuildServiceProvider().GetRequiredService<IOptions<CoreProxyConfiguration>>().Value;
        }
    }
}
