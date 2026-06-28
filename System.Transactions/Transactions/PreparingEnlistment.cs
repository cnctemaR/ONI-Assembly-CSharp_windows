using System;

namespace System.Transactions
{
	public class PreparingEnlistment : Enlistment
	{
		internal PreparingEnlistment(Transaction tx, IEnlistmentNotification enlisted)
		{
			this.tx = tx;
			this.enlisted = enlisted;
		}

		public void ForceRollback()
		{
			this.ForceRollback(null);
		}

		[MonoTODO]
		public void ForceRollback(Exception ex)
		{
			this.tx.Rollback(ex, this.enlisted);
		}

		[MonoTODO]
		public void Prepared()
		{
			this.prepared = true;
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

		private bool prepared;

		private Transaction tx;

		private IEnlistmentNotification enlisted;
	}
}
