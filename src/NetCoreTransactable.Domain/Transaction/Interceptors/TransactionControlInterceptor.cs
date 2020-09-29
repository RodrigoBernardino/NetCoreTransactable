using System;
using System.Transactions;

namespace NetCoreTransactable
{
    public class TransactionControlInterceptor : IMethodInterceptor
    {
        private TransactionScope _transactionScope;

        public void BeforeInvoke(InvocationContext invocationContext)
        {
            _transactionScope = GetTransactionScope(invocationContext);
        }

        public void AfterInvoke(InvocationContext invocationContext, object methodResult)
        {
            try
            {
                if (!invocationContext.CheckIfInvocationInErrorState())
                    _transactionScope.Complete();
            }
            finally
            {
                _transactionScope.Dispose();
            }
        }

        private TransactionScope GetTransactionScope(InvocationContext invocationContext)
        {
            TransactableAttribute transactableAttribute = invocationContext
                .GetAttributeFromMethod<TransactableAttribute>();

            IsolationLevel newTransactionIsolationLevel = transactableAttribute.IsolationLevel;
            Transaction currentTransaction = Transaction.Current;
            if (currentTransaction != null)
                newTransactionIsolationLevel = currentTransaction.IsolationLevel;

            var transactionScope = new TransactionScope(
                transactableAttribute.TransactionScopeOption,
                new TransactionOptions
                {
                    IsolationLevel = newTransactionIsolationLevel,
                    Timeout = TimeSpan.FromMilliseconds(transactableAttribute.TimeoutInMilliseconds)
                },
                transactableAttribute.TransactionScopeAsyncFlowOption);

            return transactionScope;
        }
    }
}
