using System.Transactions;

namespace NetCoreTransactable
{
    public sealed class TransactableAttribute : MethodInterceptionAttribute
    {
        public TransactionScopeOption TransactionScopeOption { get; set; }
        public IsolationLevel IsolationLevel { get; set; }
        public TransactionScopeAsyncFlowOption TransactionScopeAsyncFlowOption { get; set; }
        public int TimeoutInMilliseconds { get; set; }

        public TransactableAttribute()
        {
            //default is TransactionScopeOption.Required
            TransactionScopeOption = TransactionScopeOption.Required;

            //default is IsolationLevel.Serializable;
            //SQL Server and Oracle default isolation level is ReadCommitted.
            IsolationLevel = IsolationLevel.ReadCommitted;

            //default is 60000 milliseconds
            TimeoutInMilliseconds = 60000;

            TransactionScopeAsyncFlowOption = TransactionScopeAsyncFlowOption.Enabled;
        }
    }
}
