using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;
using Unity;

namespace System.Transactions
{
	[Serializable]
	public class Transaction : IDisposable, ISerializable
	{
		internal List<IEnlistmentNotification> Volatiles
		{
			get
			{
				if (this.volatiles == null)
				{
					this.volatiles = new List<IEnlistmentNotification>();
				}
				return this.volatiles;
			}
		}

		internal List<ISinglePhaseNotification> Durables
		{
			get
			{
				if (this.durables == null)
				{
					this.durables = new List<ISinglePhaseNotification>();
				}
				return this.durables;
			}
		}

		internal IPromotableSinglePhaseNotification Pspe
		{
			get
			{
				return this.pspe;
			}
		}

		internal Transaction(IsolationLevel isolationLevel)
		{
			this.dependents = new ArrayList();
			this.tag = Guid.NewGuid();
			base..ctor();
			this.info = new TransactionInformation();
			this.level = isolationLevel;
		}

		internal Transaction(Transaction other)
		{
			this.dependents = new ArrayList();
			this.tag = Guid.NewGuid();
			base..ctor();
			this.level = other.level;
			this.info = other.info;
			this.dependents = other.dependents;
			this.volatiles = other.Volatiles;
			this.durables = other.Durables;
			this.pspe = other.Pspe;
			this.TransactionCompletedInternal = other.TransactionCompletedInternal;
			this.internalTransaction = other;
		}

		[MonoTODO]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		internal event TransactionCompletedEventHandler TransactionCompletedInternal;

		public event TransactionCompletedEventHandler TransactionCompleted
		{
			add
			{
				if (this.internalTransaction != null)
				{
					this.internalTransaction.TransactionCompleted += value;
				}
				this.TransactionCompletedInternal += value;
			}
			remove
			{
				if (this.internalTransaction != null)
				{
					this.internalTransaction.TransactionCompleted -= value;
				}
				this.TransactionCompletedInternal -= value;
			}
		}

		public static Transaction Current
		{
			get
			{
				Transaction.EnsureIncompleteCurrentScope();
				return Transaction.CurrentInternal;
			}
			set
			{
				Transaction.EnsureIncompleteCurrentScope();
				Transaction.CurrentInternal = value;
			}
		}

		internal static Transaction CurrentInternal
		{
			get
			{
				return Transaction.ambient;
			}
			set
			{
				Transaction.ambient = value;
			}
		}

		public IsolationLevel IsolationLevel
		{
			get
			{
				Transaction.EnsureIncompleteCurrentScope();
				return this.level;
			}
		}

		public TransactionInformation TransactionInformation
		{
			get
			{
				Transaction.EnsureIncompleteCurrentScope();
				return this.info;
			}
		}

		public Transaction Clone()
		{
			return new Transaction(this);
		}

		public void Dispose()
		{
			if (this.TransactionInformation.Status == TransactionStatus.Active)
			{
				this.Rollback();
			}
		}

		[MonoTODO]
		public DependentTransaction DependentClone(DependentCloneOption cloneOption)
		{
			DependentTransaction dependentTransaction = new DependentTransaction(this, cloneOption);
			this.dependents.Add(dependentTransaction);
			return dependentTransaction;
		}

		[MonoTODO("Only SinglePhase commit supported for durable resource managers.")]
		[PermissionSet(SecurityAction.LinkDemand)]
		public Enlistment EnlistDurable(Guid resourceManagerIdentifier, IEnlistmentNotification enlistmentNotification, EnlistmentOptions enlistmentOptions)
		{
			throw new NotImplementedException("DTC unsupported, only SinglePhase commit supported for durable resource managers.");
		}

		[MonoTODO("Only Local Transaction Manager supported. Cannot have more than 1 durable resource per transaction. Only EnlistmentOptions.None supported yet.")]
		[PermissionSet(SecurityAction.LinkDemand)]
		public Enlistment EnlistDurable(Guid resourceManagerIdentifier, ISinglePhaseNotification singlePhaseNotification, EnlistmentOptions enlistmentOptions)
		{
			Transaction.EnsureIncompleteCurrentScope();
			if (this.pspe != null || this.Durables.Count > 0)
			{
				throw new NotImplementedException("DTC unsupported, multiple durable resource managers aren't supported.");
			}
			if (enlistmentOptions != EnlistmentOptions.None)
			{
				throw new NotImplementedException("EnlistmentOptions other than None aren't supported");
			}
			this.Durables.Add(singlePhaseNotification);
			return new Enlistment();
		}

		public bool EnlistPromotableSinglePhase(IPromotableSinglePhaseNotification promotableSinglePhaseNotification)
		{
			Transaction.EnsureIncompleteCurrentScope();
			if (this.pspe != null || this.Durables.Count > 0)
			{
				return false;
			}
			this.pspe = promotableSinglePhaseNotification;
			this.pspe.Initialize();
			return true;
		}

		public void SetDistributedTransactionIdentifier(IPromotableSinglePhaseNotification promotableNotification, Guid distributedTransactionIdentifier)
		{
			throw new NotImplementedException();
		}

		public bool EnlistPromotableSinglePhase(IPromotableSinglePhaseNotification promotableSinglePhaseNotification, Guid promoterType)
		{
			throw new NotImplementedException();
		}

		public byte[] GetPromotedToken()
		{
			throw new NotImplementedException();
		}

		public Guid PromoterType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		[MonoTODO("EnlistmentOptions being ignored")]
		public Enlistment EnlistVolatile(IEnlistmentNotification enlistmentNotification, EnlistmentOptions enlistmentOptions)
		{
			return this.EnlistVolatileInternal(enlistmentNotification, enlistmentOptions);
		}

		[MonoTODO("EnlistmentOptions being ignored")]
		public Enlistment EnlistVolatile(ISinglePhaseNotification singlePhaseNotification, EnlistmentOptions enlistmentOptions)
		{
			return this.EnlistVolatileInternal(singlePhaseNotification, enlistmentOptions);
		}

		private Enlistment EnlistVolatileInternal(IEnlistmentNotification notification, EnlistmentOptions options)
		{
			Transaction.EnsureIncompleteCurrentScope();
			this.Volatiles.Add(notification);
			return new Enlistment();
		}

		[MonoTODO("Only Local Transaction Manager supported. Cannot have more than 1 durable resource per transaction.")]
		[PermissionSet(SecurityAction.LinkDemand)]
		public Enlistment PromoteAndEnlistDurable(Guid manager, IPromotableSinglePhaseNotification promotableNotification, ISinglePhaseNotification notification, EnlistmentOptions options)
		{
			throw new NotImplementedException("DTC unsupported, multiple durable resource managers aren't supported.");
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as Transaction);
		}

		private bool Equals(Transaction t)
		{
			return t == this || (t != null && this.level == t.level && this.info == t.info);
		}

		public static bool operator ==(Transaction x, Transaction y)
		{
			if (x == null)
			{
				return y == null;
			}
			return x.Equals(y);
		}

		public static bool operator !=(Transaction x, Transaction y)
		{
			return !(x == y);
		}

		public override int GetHashCode()
		{
			return (int)(this.level ^ (IsolationLevel)this.info.GetHashCode() ^ (IsolationLevel)this.dependents.GetHashCode());
		}

		public void Rollback()
		{
			this.Rollback(null);
		}

		public void Rollback(Exception e)
		{
			Transaction.EnsureIncompleteCurrentScope();
			this.Rollback(e, null);
		}

		internal void Rollback(Exception ex, object abortingEnlisted)
		{
			if (this.aborted)
			{
				this.FireCompleted();
				return;
			}
			if (this.info.Status == TransactionStatus.Committed)
			{
				throw new TransactionException("Transaction has already been committed. Cannot accept any new work.");
			}
			this.innerException = ex;
			SinglePhaseEnlistment singlePhaseEnlistment = new SinglePhaseEnlistment();
			foreach (IEnlistmentNotification enlistmentNotification in this.Volatiles)
			{
				if (enlistmentNotification != abortingEnlisted)
				{
					enlistmentNotification.Rollback(singlePhaseEnlistment);
				}
			}
			List<ISinglePhaseNotification> list = this.Durables;
			if (list.Count > 0 && list[0] != abortingEnlisted)
			{
				list[0].Rollback(singlePhaseEnlistment);
			}
			if (this.pspe != null && this.pspe != abortingEnlisted)
			{
				this.pspe.Rollback(singlePhaseEnlistment);
			}
			this.Aborted = true;
			this.FireCompleted();
		}

		private bool Aborted
		{
			get
			{
				return this.aborted;
			}
			set
			{
				this.aborted = value;
				if (this.aborted)
				{
					this.info.Status = TransactionStatus.Aborted;
				}
			}
		}

		internal TransactionScope Scope
		{
			get
			{
				return this.scope;
			}
			set
			{
				this.scope = value;
			}
		}

		protected IAsyncResult BeginCommitInternal(AsyncCallback callback)
		{
			if (this.committed || this.committing)
			{
				throw new InvalidOperationException("Commit has already been called for this transaction.");
			}
			this.committing = true;
			this.asyncCommit = new Transaction.AsyncCommit(this.DoCommit);
			return this.asyncCommit.BeginInvoke(callback, null);
		}

		protected void EndCommitInternal(IAsyncResult ar)
		{
			this.asyncCommit.EndInvoke(ar);
		}

		internal void CommitInternal()
		{
			if (this.committed || this.committing)
			{
				throw new InvalidOperationException("Commit has already been called for this transaction.");
			}
			this.committing = true;
			try
			{
				this.DoCommit();
			}
			catch (TransactionException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new TransactionAbortedException("Transaction failed", ex);
			}
		}

		private void DoCommit()
		{
			if (this.Scope != null && (!this.Scope.IsComplete || !this.Scope.IsDisposed))
			{
				this.Rollback(null, null);
				this.CheckAborted();
			}
			List<IEnlistmentNotification> list = this.Volatiles;
			List<ISinglePhaseNotification> list2 = this.Durables;
			if (list.Count == 1 && list2.Count == 0)
			{
				ISinglePhaseNotification singlePhaseNotification = list[0] as ISinglePhaseNotification;
				if (singlePhaseNotification != null)
				{
					this.DoSingleCommit(singlePhaseNotification);
					this.Complete();
					return;
				}
			}
			if (list.Count > 0)
			{
				this.DoPreparePhase();
			}
			if (list2.Count > 0)
			{
				this.DoSingleCommit(list2[0]);
			}
			if (this.pspe != null)
			{
				this.DoSingleCommit(this.pspe);
			}
			if (list.Count > 0)
			{
				this.DoCommitPhase();
			}
			this.Complete();
		}

		private void Complete()
		{
			this.committing = false;
			this.committed = true;
			if (!this.aborted)
			{
				this.info.Status = TransactionStatus.Committed;
			}
			this.FireCompleted();
		}

		internal void InitScope(TransactionScope scope)
		{
			this.CheckAborted();
			if (this.committed)
			{
				throw new InvalidOperationException("Commit has already been called on this transaction.");
			}
			this.Scope = scope;
		}

		private static void PrepareCallbackWrapper(object state)
		{
			PreparingEnlistment preparingEnlistment = state as PreparingEnlistment;
			try
			{
				preparingEnlistment.EnlistmentNotification.Prepare(preparingEnlistment);
			}
			catch (Exception ex)
			{
				preparingEnlistment.Exception = ex;
				if (!preparingEnlistment.IsPrepared)
				{
					((ManualResetEvent)preparingEnlistment.WaitHandle).Set();
				}
			}
		}

		private void DoPreparePhase()
		{
			foreach (IEnlistmentNotification enlistmentNotification in this.Volatiles)
			{
				PreparingEnlistment preparingEnlistment = new PreparingEnlistment(this, enlistmentNotification);
				ThreadPool.QueueUserWorkItem(new WaitCallback(Transaction.PrepareCallbackWrapper), preparingEnlistment);
				TimeSpan timeSpan = ((this.Scope != null) ? this.Scope.Timeout : TransactionManager.DefaultTimeout);
				if (!preparingEnlistment.WaitHandle.WaitOne(timeSpan, true))
				{
					this.Aborted = true;
					throw new TimeoutException("Transaction timedout");
				}
				if (preparingEnlistment.Exception != null)
				{
					this.innerException = preparingEnlistment.Exception;
					this.Aborted = true;
					break;
				}
				if (!preparingEnlistment.IsPrepared)
				{
					this.Aborted = true;
					break;
				}
			}
			this.CheckAborted();
		}

		private void DoCommitPhase()
		{
			foreach (IEnlistmentNotification enlistmentNotification in this.Volatiles)
			{
				Enlistment enlistment = new Enlistment();
				enlistmentNotification.Commit(enlistment);
			}
		}

		private void DoSingleCommit(ISinglePhaseNotification single)
		{
			if (single == null)
			{
				return;
			}
			single.SinglePhaseCommit(new SinglePhaseEnlistment(this, single));
			this.CheckAborted();
		}

		private void DoSingleCommit(IPromotableSinglePhaseNotification single)
		{
			if (single == null)
			{
				return;
			}
			single.SinglePhaseCommit(new SinglePhaseEnlistment(this, single));
			this.CheckAborted();
		}

		private void CheckAborted()
		{
			if (this.aborted || (this.Scope != null && this.Scope.IsAborted))
			{
				throw new TransactionAbortedException("Transaction has aborted", this.innerException);
			}
		}

		private void FireCompleted()
		{
			if (this.TransactionCompletedInternal != null)
			{
				this.TransactionCompletedInternal(this, new TransactionEventArgs(this));
			}
		}

		private static void EnsureIncompleteCurrentScope()
		{
			if (Transaction.CurrentInternal == null)
			{
				return;
			}
			if (Transaction.CurrentInternal.Scope != null && Transaction.CurrentInternal.Scope.IsComplete)
			{
				throw new InvalidOperationException("The current TransactionScope is already complete");
			}
		}

		internal Transaction()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		[ThreadStatic]
		private static Transaction ambient;

		private Transaction internalTransaction;

		private IsolationLevel level;

		private TransactionInformation info;

		private ArrayList dependents;

		private List<IEnlistmentNotification> volatiles;

		private List<ISinglePhaseNotification> durables;

		private IPromotableSinglePhaseNotification pspe;

		private Transaction.AsyncCommit asyncCommit;

		private bool committing;

		private bool committed;

		private bool aborted;

		private TransactionScope scope;

		private Exception innerException;

		private Guid tag;

		private delegate void AsyncCommit();
	}
}
