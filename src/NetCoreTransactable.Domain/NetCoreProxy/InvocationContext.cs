using Castle.DynamicProxy;
using NetCoreTransactable.Domain.NetCoreProxy.Internal.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NetCoreTransactable
{
    // <summary>
    /// Metadata Class that describes the Current Invocation
    /// </summary>
    public class InvocationContext
    {
        /// <summary>
        /// Gets or sets the Invocation Context
        /// </summary>
        internal IInvocation Invocation { get; set; }

        /// <summary>
        /// Gets or sets the Attribute that triggered the interceptor
        /// </summary>
        internal MethodInterceptionAttribute Attribute { get; set; }

        /// <summary>
        /// Gets or sets the Interceptor that is triggered for this method
        /// </summary>
        internal IMethodInterceptor Interceptor { get; set; }

        /// <summary>
        /// Stores data which can be passed between interceptors
        /// </summary>
        internal ConcurrentDictionary<string, object> TempData { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IServiceProvider"/>
        /// </summary>
        internal IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Gets or sets the Order Priority of this invocation
        /// </summary>
        internal int Order { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether invocation of the underlying method is bypassed
        /// </summary>
        internal bool InvocationIsBypassed { get; set; }

        // <summary>
        /// Gets or sets a value that indicates whether invocation is in an error state
        /// </summary>
        internal bool InvocationIsInErrorState { get; set; }

        /// <summary>
        /// Initialises a new instance of InvocationContext
        /// </summary>
        /// <param name="attribute">Intercepted method's attribute</param>
        /// <param name="interceptor">Intercepted method's interceptor</param>
        /// <param name="invocation">Intercepted method's invocation</param>
        /// <param name="order">Order priority of intercepted method's invocation</param>
        /// <param name="serviceProvider">Service Provider</param>
        public InvocationContext(MethodInterceptionAttribute attribute,
            IMethodInterceptor interceptor,
            IInvocation invocation,
            int order,
            IServiceProvider serviceProvider)
        {
            TempData = new ConcurrentDictionary<string, object>();

            Attribute = attribute;
            Interceptor = interceptor;
            Invocation = invocation;
            Order = order;
            ServiceProvider = serviceProvider;
        }

        /// <summary>
        /// Gets the attribute that initiated the interception
        /// </summary>
        /// <returns><see cref="MethodInterceptionAttribute"/></returns>
        public MethodInterceptionAttribute GetOwningAttribute() =>
            Attribute;

        /// <summary>
        /// Gets the type that owns the executing method
        /// </summary>
        /// <returns><see cref="Type"/></returns>
        public Type GetOwningType() =>
            Invocation.Method.DeclaringType;

        /// <summary>
        /// Gets the Service Provider for resolving dependencies
        /// </summary>
        /// <returns><see cref="IServiceProvider"/></returns>
        public IServiceProvider GetServiceProvider() =>
            ServiceProvider;

        /// <summary>
        /// Gets the position of the specified type in the methods parameter list
        /// </summary>
        /// <typeparam name="T">Type of value to get</typeparam>
        /// <param name="parameterPosition">Parameter Position</param>
        /// <returns>Returns the value of the Parameter at the given position as {T}</returns>
        public T GetParameterValue<T>(int parameterPosition) =>
            (T)Invocation.GetArgumentValue(parameterPosition);

        /// <summary>
        /// Gets the position of the specified type in the methods parameter list
        /// </summary>
        /// <param name="parameterPosition">Parameter Position</param>
        /// <returns>Returns the value of the Parameter at the given position</returns>
        public object GetParameterValue(int parameterPosition) =>
            Invocation.GetArgumentValue(parameterPosition);

        /// <summary>
        /// Sets the value of the parameter at the specified location
        /// </summary>
        /// <param name="parameterPosition">Parameter Position</param>
        /// <param name="newValue">New Value</param>
        public void SetParameterValue(int parameterPosition, object newValue) =>
            Invocation.SetArgumentValue(parameterPosition, newValue);

        /// <summary>
        /// Adds temporary data to the context
        /// </summary>
        /// <param name="name">Name to identify the data</param>
        /// <param name="value">Data Value</param>
        public void SetTemporaryData(string name, object value) =>
            TempData.TryAdd(name, value);

        /// <summary>
        /// Gets temporary data from the context
        /// </summary>
        /// <param name="name">Name to identify the data</param>
        /// <returns></returns>
        public object GetTemporaryData(string name) =>
            TempData.GetValueOrDefault(name);

        /// <summary>
        /// This does not work with async methods. Sets the Invocation of the underlying method to be bypassed
        /// </summary>
        public void BypassInvocation() =>
            InvocationIsBypassed = true;

        /// <summary>
        /// Sets the Invocation of the underlying method to an error state
        /// </summary>
        public void SetInvocationInErrorState() =>
            InvocationIsInErrorState = true;

        /// <summary>
        /// Checks if the Invocation of the underlying method is in an error state
        /// </summary>
        public bool CheckIfInvocationInErrorState() =>
            InvocationIsInErrorState;

        /// <summary>
        /// Gets the Executing Method Info
        /// </summary>
        /// <returns>Returns the position of the type in the method parameters. Returns -1 if not found</returns>
        public MethodInfo GetExecutingMethodInfo() =>
            Invocation.Method;

        /// <summary>
        /// Gets the Executing Method Name
        /// </summary>
        /// <returns>The Name of the executing method</returns>
        public string GetExecutingMethodName() =>
            Invocation.Method.Name;

        /// <summary>
        /// Gets the specified attribute from the executing method
        /// </summary>
        public TAttribute GetAttributeFromMethod<TAttribute>() where TAttribute : Attribute =>
            Invocation.MethodInvocationTarget.GetCustomAttributes().OfType<TAttribute>().FirstOrDefault();

        /// <summary>
        /// Gets the position of the specified type in the methods parameter list
        /// </summary>
        /// <typeparam name="TTypeToFind">Type to find</typeparam>
        /// <returns>Returns the position of the type in the method parameters. Returns -1 if not found</returns>
        public int GetParameterPosition<TTypeToFind>() =>
            GetParameterPosition(typeof(TTypeToFind));

        /// <summary>
        /// This does not work with async methods. Overrides the return value for the method being called. Usually called with [BypassInvocation] to shortcut interception
        /// </summary>
        /// <returns>Method Return Value</returns>
        public void OverrideMethodReturnValue(object returnValue)
        {
            if (InvocationExtensions.CheckIfMethodIsAsync(Invocation))
                Invocation.ReturnValue = Task.FromResult<dynamic>(returnValue);
            else
                Invocation.ReturnValue = returnValue;
        }

        /// <summary>
        /// Gets the return value from the method that was called
        /// </summary>
        /// <returns>Method Return Value</returns>
        public object GetMethodReturnValue()
        {
            if (CheckIfInvocationInErrorState())
                return null;

            if (InvocationExtensions.CheckIfMethodIsAsync(Invocation))
            {
                var returnValue = (dynamic)Invocation.ReturnValue;
                Type returnValueType = Invocation.ReturnValue.GetType();

                if (returnValueType.GetGenericTypeDefinition() == typeof(Task<>)
                    && returnValueType.GetProperty("Result").PropertyType.FullName != "System.Threading.Tasks.VoidTaskResult")
                {
                    return GetAsyncMethodResult(returnValue).Result;
                }
                else //method result is Task void
                {
                    AwaitVoidAsyncMethod(returnValue);
                    return null;
                }
            }

            return Invocation.ReturnValue;
        }

        /// <summary>
        /// Method to try and identify the position of the specified type in the methods parameter list
        /// </summary>
        /// <param name="typeToFind">Type To Find</param>
        /// <returns>Returns the position of the type in the method parameters. Returns -1 if not found</returns>
        public int GetParameterPosition(Type typeToFind)
        {
            var method = Invocation.Method;
            if (method == null)
                throw new ArgumentNullException(nameof(method));

            int len = method.GetParameters().Length;
            for (int i = len - 1; i >= 0; i--)
            {
                var paramType = method.GetParameters()[i].ParameterType;
                if (paramType != typeToFind)
                    continue;

                return i;
            }

            return -1;
        }

        private async Task<T> GetAsyncMethodResult<T>(Task<T> task)
        {
            T result = await task.ConfigureAwait(false);

            return result;
        }

        private async Task AwaitVoidAsyncMethod(Task task)
        {
            await task.ConfigureAwait(false);
        }
    }
}
