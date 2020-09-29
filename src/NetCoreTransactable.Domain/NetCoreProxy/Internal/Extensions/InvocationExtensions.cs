using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using NetCoreTransactable.Domain.NetCoreProxy.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreTransactable.Domain.NetCoreProxy.Internal.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IInvocation"/>
    /// </summary>
    internal static class InvocationExtensions
    {
        /// <summary>
        /// Gets a collection of all the interceptors associated with the current executing method
        /// </summary>
        /// <param name="invocation">Current Invocation</param>
        /// <param name="serviceProvider">Service Provider Instance</param>
        /// <param name="proxyConfiguration">Proxy Configuration</param>
        /// <returns><see cref="Dictionary{TKey,TValue}"/> of all configured interceptors for this method</returns>
        internal static List<InvocationContext> GetInterceptorContextForMethod(IInvocation invocation,
            IServiceProvider serviceProvider, CoreProxyConfiguration proxyConfiguration)
        {
            var invocationContextList = new List<InvocationContext>();

            #region IMPROVEMENT
            //fazer recursividade com GetInterfaces
            //alterar o InvocationContext para receber o methodInfo da interface (com os atributos)

            /*Type interfaces = invocation.MethodInvocationTarget.DeclaringType.GetInterfaces().FirstOrDefault();
            var interfaceMethodInfo = interfaces.GetMethod(invocation.MethodInvocationTarget.Name);
            List<MethodInterceptionAttribute> methodAttributes = interfaceMethodInfo
                .GetCustomAttributes(true)
                .Where(att => att.GetType().IsSubclassOf(typeof(MethodInterceptionAttribute)))
                .Cast<MethodInterceptionAttribute>()
                .ToList();*/
            #endregion

            List<MethodInterceptionAttribute> methodAttributes = invocation
                .MethodInvocationTarget
                .GetCustomAttributes(true)
                .Where(att => att.GetType().IsSubclassOf(typeof(MethodInterceptionAttribute)))
                .Cast<MethodInterceptionAttribute>()
                .ToList();

            int index = 0;
            foreach (MethodInterceptionAttribute methodAttribute in methodAttributes)
            {
                Type interceptorType = proxyConfiguration.ConfiguredInterceptors
                    .FirstOrDefault(i => i.AttributeType == methodAttribute.GetType())
                    ?.InterceptorType;

                if (interceptorType == null)
                {
                    if (proxyConfiguration.IgnoreInvalidInterceptors)
                        continue;

                    throw new Exception($"The Interceptor Attribute '{methodAttribute}' is applied to the method, but there is no configured interceptor to handle it");
                }

                var interceptorInstance = (IMethodInterceptor)ActivatorUtilities
                    .CreateInstance(serviceProvider, interceptorType);

                var invocationContext = new InvocationContext(methodAttribute, interceptorInstance,
                    invocation, index, serviceProvider);

                invocationContextList.Add(invocationContext);
                index++;
            }

            return invocationContextList;
        }

        /// <summary>
        /// Checks if intercepted method is async
        /// </summary>
        internal static bool CheckIfMethodIsAsync(IInvocation invocation) =>
            invocation.Method.ReturnType == typeof(Task)
                || (invocation.Method.ReturnType.IsGenericType
                    && invocation.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));
    }
}
