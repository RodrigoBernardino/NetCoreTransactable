using System;

namespace NetCoreTransactable.Domain.NetCoreProxy.Internal.Interfaces
{
    /// <summary>
    /// Interceptor Mapping Interface
    /// </summary>
    internal interface IInterceptorMapping
    {
        /// <summary>
        /// Attribute Type
        /// </summary>
        Type AttributeType { get; }

        /// <summary>
        /// Interceptor Type
        /// </summary>
        Type InterceptorType { get; }
    }
}
