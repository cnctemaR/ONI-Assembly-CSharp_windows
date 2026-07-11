using System;
using System.Threading;
using Unity;

namespace System.Transactions
{
	public class PreparingEnlistment : Enlistment
	{
		internal PreparingEnlistment(Transaction tx, IEnlistmentNotification enlisted)
		{
			this.tx = tx;
			this.enlisted = enlisted;
			this.waitHandle = new ManualResetEvent(false);
		}

		public void ForceRollback()
		{
			this.ForceRollback(null);
		}

		internal override void InternalOnDone()
		{
			this.Prepared();
		}

		[MonoTODO]
		public void ForceRollback(Exception e)
		{
			this.tx.Rollback(e, this.enlisted);
			((ManualResetEvent)this.waitHandle).Set();
		}

		[MonoTODO]
		public void Prepared()
		{
			this.prepared = true;
			((ManualResetEvent)this.waitHandle).Set();
		}

		[MonoTODO]
		public byte[] RecoveryInformation()
		{
			throw new NotImplementedException();
		}

		internal bool IsPrepared
		{
			get
			{
				return this.prepared;
			}
		}

		internal WaitHandle WaitHandle
		{
			get
			{
				return this.waitHandle;
			}
		}

		internal IEnlistmentNotification EnlistmentNotification
		{
			get
			{
				return this.enlisted;
			}
		}

		internal Exception Exception
		{
			get
			{
				return this.ex;
			}
			set
			{
				this.ex = value;
			}
		}

		internal PreparingEnlistment()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private bool prepared;

		private Transaction tx;

		private IEnlistmentNotification enlisted;

		private WaitHandle waitHandle;

		private Exception ex;
	}
}
