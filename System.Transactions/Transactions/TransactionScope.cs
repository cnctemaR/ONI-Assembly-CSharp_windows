using System;

namespace System.Transactions
{
	public sealed class TransactionScope : IDisposable
	{
		public TransactionScope()
			: this(TransactionScopeOption.Required, TransactionManager.DefaultTimeout)
		{
		}

		public TransactionScope(Transaction transaction)
			: this(transaction, TransactionManager.DefaultTimeout)
		{
		}

		public TransactionScope(Transaction transaction, TimeSpan timeout)
			: this(transaction, timeout, EnterpriseServicesInteropOption.None)
		{
		}

		[MonoTODO("EnterpriseServicesInteropOption not supported.")]
		public TransactionScope(Transaction transaction, TimeSpan timeout, EnterpriseServicesInteropOption opt)
		{
			this.Initialize(TransactionScopeOption.Required, transaction, TransactionScope.defaultOptions, opt, timeout);
		}

		public TransactionScope(TransactionScopeOption option)
			: this(option, TransactionManager.DefaultTimeout)
		{
		}

		[MonoTODO("No TimeoutException is thrown")]
		public TransactionScope(TransactionScopeOption option, TimeSpan timeout)
		{
			this.Initialize(option, null, TransactionScope.defaultOptions, EnterpriseServicesInteropOption.None, timeout);
		}

		public TransactionScope(TransactionScopeOption scopeOption, TransactionOptions options)
			: this(scopeOption, options, EnterpriseServicesInteropOption.None)
		{
		}

		[MonoTODO("EnterpriseServicesInteropOption not supported")]
		public TransactionScope(TransactionScopeOption scopeOption, TransactionOptions options, EnterpriseServicesInteropOption opt)
		{
			this.Initialize(scopeOption, null, options, opt, TransactionManager.DefaultTimeout);
		}

		private void Initialize(TransactionScopeOption scopeOption, Transaction tx, TransactionOptions options, EnterpriseServicesInteropOption interop, TimeSpan timeout)
		{
			this.completed = false;
			this.isRoot = false;
			this.nested = 0;
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
			if (Transaction.CurrentInternal != this.transaction)
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
		}

		private static TransactionOptions defaultOptions = new TransactionOptions(IsolationLevel.Serializable, TransactionManager.DefaultTimeout);

		private Transaction transaction;

		private Transaction oldTransaction;

		private TransactionScope parentScope;

		private int nested;

		private bool disposed;

		private bool completed;

		private bool isRoot;
	}
}
