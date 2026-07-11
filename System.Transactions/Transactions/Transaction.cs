using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Transactions
{
	[Serializable]
	public class Transaction : IDisposable, ISerializable
	{
		internal Transaction()
		{
			this.info = new TransactionInformation();
			this.level = IsolationLevel.Serializable;
		}

		internal Transaction(Transaction other)
		{
			this.level = other.level;
			this.info = other.info;
			this.dependents = other.dependents;
		}

		public event TransactionCompletedEventHandler TransactionCompleted;

		[MonoTODO]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
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
		public DependentTransaction DependentClone(DependentCloneOption option)
		{
			DependentTransaction dependentTransaction = new DependentTransaction(this, option);
			this.dependents.Add(dependentTransaction);
			return dependentTransaction;
		}

		[MonoTODO("Only SinglePhase commit supported for durable resource managers.")]
		[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"/>\n")]
		public Enlistment EnlistDurable(Guid manager, IEnlistmentNotification notification, EnlistmentOptions options)
		{
			throw new NotImplementedException("Only SinglePhase commit supported for durable resource managers.");
		}

		[MonoTODO("Only Local Transaction Manager supported. Cannot have more than 1 durable resource per transaction. Only EnlistmentOptions.None supported yet.")]
		[PermissionSet((SecurityAction)14, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\"/>\n")]
		public Enlistment EnlistDurable(Guid manager, ISinglePhaseNotification notification, EnlistmentOptions options)
		{
			if (this.durables.Count == 1)
			{
				throw new NotImplementedException("Only LTM supported. Cannot have more than 1 durable resource per transaction.");
			}
			Transaction.EnsureIncompleteCurrentScope();
			if (options != EnlistmentOptions.None)
			{
				throw new NotImplementedException("Implement me");
			}
			this.durables.Add(notification);
			return new Enlistment();
		}

		[MonoTODO]
		public bool EnlistPromotableSinglePhase(IPromotableSinglePhaseNotification notification)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("EnlistmentOptions being ignored")]
		public Enlistment EnlistVolatile(IEnlistmentNotification notification, EnlistmentOptions options)
		{
			return this.EnlistVolatileInternal(notification, options);
		}

		[MonoTODO("EnlistmentOptions being ignored")]
		public Enlistment EnlistVolatile(ISinglePhaseNotification notification, EnlistmentOptions options)
		{
			return this.EnlistVolatileInternal(notification, options);
		}

		private Enlistment EnlistVolatileInternal(IEnlistmentNotification notification, EnlistmentOptions options)
		{
			Transaction.EnsureIncompleteCurrentScope();
			this.volatiles.Add(notification);
			return new Enlistment();
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as Transaction);
		}

		private bool Equals(Transaction t)
		{
			return object.ReferenceEquals(t, this) || (!object.ReferenceEquals(t, null) && this.level == t.level && this.info == t.info);
		}

		public override int GetHashCode()
		{
			return (int)(this.level ^ (IsolationLevel)this.info.GetHashCode() ^ (IsolationLevel)this.dependents.GetHashCode());
		}

		public void Rollback()
		{
			this.Rollback(null);
		}

		public void Rollback(Exception ex)
		{
			Transaction.EnsureIncompleteCurrentScope();
			this.Rollback(ex, null);
		}

		internal void Rollback(Exception ex, IEnlistmentNotification enlisted)
		{
			if (this.aborted)
			{
				return;
			}
			if (this.info.Status == TransactionStatus.Committed)
			{
				throw new TransactionException("Transaction has already been committed. Cannot accept any new work.");
			}
			this.innerException = ex;
			Enlistment enlistment = new Enlistment();
			foreach (IEnlistmentNotification enlistmentNotification in this.volatiles)
			{
				if (enlistmentNotification != enlisted)
				{
					enlistmentNotification.Rollback(enlistment);
				}
			}
			if (this.durables.Count > 0 && this.durables[0] != enlisted)
			{
				this.durables[0].Rollback(enlistment);
			}
			this.Aborted = true;
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
			this.DoCommit();
		}

		private void DoCommit()
		{
			if (this.Scope != null)
			{
				this.Rollback(null, null);
				this.CheckAborted();
			}
			if (this.volatiles.Count == 1 && this.durables.Count == 0)
			{
				ISinglePhaseNotification singlePhaseNotification = this.volatiles[0] as ISinglePhaseNotification;
				if (singlePhaseNotification != null)
				{
					this.DoSingleCommit(singlePhaseNotification);
					this.Complete();
					return;
				}
			}
			if (this.volatiles.Count > 0)
			{
				this.DoPreparePhase();
			}
			if (this.durables.Count > 0)
			{
				this.DoSingleCommit(this.durables[0]);
			}
			if (this.volatiles.Count > 0)
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

		private void DoPreparePhase()
		{
			foreach (IEnlistmentNotification enlistmentNotification in this.volatiles)
			{
				PreparingEnlistment preparingEnlistment = new PreparingEnlistment(this, enlistmentNotification);
				enlistmentNotification.Prepare(preparingEnlistment);
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
			foreach (IEnlistmentNotification enlistmentNotification in this.volatiles)
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
			SinglePhaseEnlistment singlePhaseEnlistment = new SinglePhaseEnlistment(this, single);
			single.SinglePhaseCommit(singlePhaseEnlistment);
			this.CheckAborted();
		}

		private void CheckAborted()
		{
			if (this.aborted)
			{
				throw new TransactionAbortedException("Transaction has aborted", this.innerException);
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

		public static bool operator ==(Transaction x, Transaction y)
		{
			if (object.ReferenceEquals(x, null))
			{
				return object.ReferenceEquals(y, null);
			}
			return x.Equals(y);
		}

		public static bool operator !=(Transaction x, Transaction y)
		{
			return !(x == y);
		}

		[ThreadStatic]
		private static Transaction ambient;

		private IsolationLevel level;

		private TransactionInformation info;

		private ArrayList dependents = new ArrayList();

		private List<IEnlistmentNotification> volatiles = new List<IEnlistmentNotification>();

		private List<ISinglePhaseNotification> durables = new List<ISinglePhaseNotification>();

		private Transaction.AsyncCommit asyncCommit;

		private bool committing;

		private bool committed;

		private bool aborted;

		private TransactionScope scope;

		private Exception innerException;

		private delegate void AsyncCommit();
	}
}
