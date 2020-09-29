using System;

namespace NetCoreTransactable
{
    /// <summary>
    /// Method Interception Attribute (inherit from this to create your interception attributes)
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class MethodInterceptionAttribute : Attribute
    {
    }
}
