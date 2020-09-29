﻿using NetCoreTransactable.Domain.NetCoreProxy.Internal.Interfaces;
using System;

namespace NetCoreTransactable.Domain.NetCoreProxy.Internal.Configuration
{
    /// <summary>
    /// Maps an interceptor to an attribute
    /// </summary>
    internal class InterceptorMapping<TAttribute, TInterceptor> : IInterceptorMapping
        where TAttribute : MethodInterceptionAttribute where TInterceptor : IMethodInterceptor
    {
        /// <summary>
        /// Gets the type of the Interceptor
        /// </summary>
        public Type InterceptorType { get; }

        /// <summary>
        /// Gets the instance of the attribute, which applies interceptor to a method.
        /// </summary>
        public Type AttributeType { get; }

        /// <summary>
        /// Initialises a new instance of the <see cref="InterceptorMapping{TAttribute,TInterceptor}"/>
        /// </summary>
        public InterceptorMapping()
        {
            InterceptorType = typeof(TInterceptor);
            AttributeType = typeof(TAttribute);
        }
    }
}
