using Castle.DynamicProxy;
using NetCoreTransactable.Domain.NetCoreProxy.Configuration;
using NetCoreTransactable.Domain.NetCoreProxy.Interfaces;
using NetCoreTransactable.Domain.NetCoreProxy.Internal.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreTransactable.Domain.NetCoreProxy.Internal
{
    /// <summary>
    /// The Master Interceptor Class wraps a proxied object and handles all of its interceptions
    /// </summary>
    internal class CoreInterceptor : IInterceptor
    {
        private readonly CoreProxyConfiguration _proxyConfiguration;
        private readonly IServiceProvider _serviceProvider;

        public CoreInterceptor(IServiceProvider serviceProvider,
            CoreProxyConfiguration proxyConfiguration)
        {
            _serviceProvider = serviceProvider;
            _proxyConfiguration = proxyConfiguration;
        }

        public void Intercept(IInvocation invocation)
        {
            List<InvocationContext> invocationContextList = InvocationExtensions
                .GetInterceptorContextForMethod(invocation, _serviceProvider, _proxyConfiguration);

            if (invocationContextList == null || !invocationContextList.Any())
            {
                invocation.Proceed();
                return;
            }

            IOrderingStrategy orderingStrategy = _proxyConfiguration.OrderingStrategy;

            foreach (InvocationContext invocationContext in orderingStrategy.OrderBeforeInterception(invocationContextList))
                invocationContext.Interceptor.BeforeInvoke(invocationContext);

            try
            {
                if (invocationContextList.Any(ic => ic.InvocationIsBypassed))
                    return;

                invocation.Proceed();

                if (InvocationExtensions.CheckIfMethodIsAsync(invocation))
                    Task.WaitAll((Task)invocation.ReturnValue);
            }
            catch
            {
                foreach (InvocationContext invocationContext in orderingStrategy.OrderAfterInterception(invocationContextList))
                    invocationContext.SetInvocationInErrorState();

                throw;
            }
            finally
            {
                foreach (InvocationContext invocationContext in orderingStrategy.OrderAfterInterception(invocationContextList))
                    invocationContext.Interceptor.AfterInvoke(invocationContext, invocationContext.GetMethodReturnValue());
            }
        }
    }
}
