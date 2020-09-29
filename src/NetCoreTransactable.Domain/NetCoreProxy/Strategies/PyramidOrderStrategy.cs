using NetCoreTransactable.Domain.NetCoreProxy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetCoreTransactable.Strategies
{
    /// <summary>
    /// Pyramid Ordering Strategy for Interceptors
    /// </summary>
    public sealed class PyramidOrderStrategy : IOrderingStrategy
    {
        /// <inheritdoc />
        public IEnumerable<InvocationContext> OrderBeforeInterception(IEnumerable<InvocationContext> interceptors) =>
            interceptors;

        /// <inheritdoc />
        public IEnumerable<InvocationContext> OrderAfterInterception(IEnumerable<InvocationContext> interceptors) =>
            interceptors.Reverse();
    }
}
