using System;

namespace System.Transactions
{
	public sealed class TransactionScope : IDisposable
	{
		public TransactionScope()
			: this(TransactionScopeOption.Required, TransactionManager.DefaultTimeout)
		{
		}

		public TransactionScope(TransactionScopeAsyncFlowOption asyncFlowOption)
			: this(TransactionScopeOption.Required, TransactionManager.DefaultTimeout, asyncFlowOption)
		{
		}

		public TransactionScope(Transaction transactionToUse)
			: this(transactionToUse, TransactionManager.DefaultTimeout)
		{
		}

		public TransactionScope(Transaction transactionToUse, TimeSpan scopeTimeout)
			: this(transactionToUse, scopeTimeout, EnterpriseServicesInteropOption.None)
		{
		}

		[MonoTODO("EnterpriseServicesInteropOption not supported.")]
		public TransactionScope(Transaction transactionToUse, TimeSpan scopeTimeout, EnterpriseServicesInteropOption interopOption)
		{
			this.Initialize(TransactionScopeOption.Required, transactionToUse, TransactionScope.defaultOptions, interopOption, scopeTimeout, TransactionScopeAsyncFlowOption.Suppress);
		}

		public TransactionScope(TransactionScopeOption scopeOption)
			: this(scopeOption, TransactionManager.DefaultTimeout)
		{
		}

		public TransactionScope(TransactionScopeOption scopeOption, TimeSpan scopeTimeout)
			: this(scopeOption, scopeTimeout, TransactionScopeAsyncFlowOption.Suppress)
		{
		}

		public TransactionScope(TransactionScopeOption option, TransactionScopeAsyncFlowOption asyncFlow)
			: this(option, TransactionManager.DefaultTimeout, asyncFlow)
		{
		}

		public TransactionScope(TransactionScopeOption scopeOption, TimeSpan scopeTimeout, TransactionScopeAsyncFlowOption asyncFlow)
		{
			this.Initialize(scopeOption, null, TransactionScope.defaultOptions, EnterpriseServicesInteropOption.None, scopeTimeout, asyncFlow);
		}

		public TransactionScope(TransactionScopeOption scopeOption, TransactionOptions transactionOptions)
			: this(scopeOption, transactionOptions, EnterpriseServicesInteropOption.None)
		{
		}

		[MonoTODO("EnterpriseServicesInteropOption not supported")]
		public TransactionScope(TransactionScopeOption scopeOption, TransactionOptions transactionOptions, EnterpriseServicesInteropOption interopOption)
		{
			this.Initialize(scopeOption, null, transactionOptions, interopOption, TransactionManager.DefaultTimeout, TransactionScopeAsyncFlowOption.Suppress);
		}

		public TransactionScope(Transaction transactionToUse, TransactionScopeAsyncFlowOption asyncFlowOption)
		{
			throw new NotImplementedException();
		}

		public TransactionScope(Transaction transactionToUse, TimeSpan scopeTimeout, TransactionScopeAsyncFlowOption asyncFlowOption)
		{
			throw new NotImplementedException();
		}

		public TransactionScope(TransactionScopeOption scopeOption, TransactionOptions transactionOptions, TransactionScopeAsyncFlowOption asyncFlowOption)
		{
			throw new NotImplementedException();
		}

		private void Initialize(TransactionScopeOption scopeOption, Transaction tx, TransactionOptions options, EnterpriseServicesInteropOption interop, TimeSpan scopeTimeout, TransactionScopeAsyncFlowOption asyncFlow)
		{
			this.completed = false;
			this.isRoot = false;
			this.nested = 0;
			this.asyncFlowEnabled = asyncFlow == TransactionScopeAsyncFlowOption.Enabled;
			if (scopeTimeout < TimeSpan.Zero)
			{
				throw new ArgumentOutOfRangeException("scopeTimeout");
			}
			this.timeout = scopeTimeout;
			this.oldTransaction = Transaction.CurrentInternal;
			Transaction.CurrentInternal = (this.transaction = this.InitTransaction(tx, scopeOption));
			if (this.transaction != null)
			{
				this.transaction.InitScope(this);
			}
			if (this.parentScope != null)
			{
				this.parentScope.nested++;
			}
		}

		private Transaction InitTransaction(Transaction tx, TransactionScopeOption scopeOption)
		{
			if (tx != null)
			{
				return tx;
			}
			if (scopeOption == TransactionScopeOption.Suppress)
			{
				if (Transaction.CurrentInternal != null)
				{
					this.parentScope = Transaction.CurrentInternal.Scope;
				}
				return null;
			}
			if (scopeOption != TransactionScopeOption.Required)
			{
				if (Transaction.CurrentInternal != null)
				{
					this.parentScope = Transaction.CurrentInternal.Scope;
				}
				this.isRoot = true;
				return new Transaction();
			}
			if (Transaction.CurrentInternal == null)
			{
				this.isRoot = true;
				return new Transaction();
			}
			this.parentScope = Transaction.CurrentInternal.Scope;
			return Transaction.CurrentInternal;
		}

		public void Complete()
		{
			if (this.completed)
			{
				throw new InvalidOperationException("The current TransactionScope is already complete. You should dispose the TransactionScope.");
			}
			this.completed = true;
		}

		internal bool IsComplete
		{
			get
			{
				return this.completed;
			}
		}

		internal TimeSpan Timeout
		{
			get
			{
				return this.timeout;
			}
		}

		public void Dispose()
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			if (this.parentScope != null)
			{
				this.parentScope.nested--;
			}
			if (this.nested > 0)
			{
				this.transaction.Rollback();
				throw new InvalidOperationException("TransactionScope nested incorrectly");
			}
			if (Transaction.CurrentInternal != this.transaction && !this.asyncFlowEnabled)
			{
				if (this.transaction != null)
				{
					this.transaction.Rollback();
				}
				if (Transaction.CurrentInternal != null)
				{
					Transaction.CurrentInternal.Rollback();
				}
				throw new InvalidOperationException("Transaction.Current has changed inside of the TransactionScope");
			}
			if (this.asyncFlowEnabled)
			{
				if (this.oldTransaction != null)
				{
					this.oldTransaction.Scope = this.parentScope;
				}
				Transaction currentInternal = Transaction.CurrentInternal;
				if (this.transaction == null && currentInternal == null)
				{
					return;
				}
				currentInternal.Scope = this.parentScope;
				Transaction.CurrentInternal = this.oldTransaction;
				this.transaction.Scope = null;
				if (!this.IsComplete)
				{
					this.transaction.Rollback();
					currentInternal.Rollback();
					return;
				}
				if (!this.isRoot)
				{
					return;
				}
				currentInternal.CommitInternal();
				this.transaction.CommitInternal();
				return;
			}
			else
			{
				if (Transaction.CurrentInternal == this.oldTransaction && this.oldTransaction != null)
				{
					this.oldTransaction.Scope = this.parentScope;
				}
				Transaction.CurrentInternal = this.oldTransaction;
				if (this.transaction == null)
				{
					return;
				}
				this.transaction.Scope = null;
				if (!this.IsComplete)
				{
					this.transaction.Rollback();
					return;
				}
				if (!this.isRoot)
				{
					return;
				}
				this.transaction.CommitInternal();
				return;
			}
		}

		private static TransactionOptions defaultOptions = new TransactionOptions(IsolationLevel.Serializable, TransactionManager.DefaultTimeout);

		private Transaction transaction;

		private Transaction oldTransaction;

		private TransactionScope parentScope;

		private TimeSpan timeout;

		private int nested;

		private bool disposed;

		private bool completed;

		private bool isRoot;

		private bool asyncFlowEnabled;
	}
}
