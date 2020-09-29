using NetCoreTransactable.Domain.NetCoreProxy.Interfaces;
using System.Collections.Generic;

namespace NetCoreTransactable.Strategies
{
    /// <summary>
    /// Sequential Ordering Strategy for Interceptors
    /// </summary>
    public sealed class SequentialOrderStrategy : IOrderingStrategy
    {
        /// <inheritdoc />
        public IEnumerable<InvocationContext> OrderBeforeInterception(IEnumerable<InvocationContext> interceptors) =>
            interceptors;

        /// <inheritdoc />
        public IEnumerable<InvocationContext> OrderAfterInterception(IEnumerable<InvocationContext> interceptors) =>
            interceptors;
    }
}
