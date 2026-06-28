using System;

namespace System.Transactions
{
	public class SinglePhaseEnlistment : Enlistment
	{
		internal SinglePhaseEnlistment(Transaction tx, ISinglePhaseNotification enlisted)
		{
			this.tx = tx;
			this.enlisted = enlisted;
		}

		public void Aborted()
		{
			this.Aborted(null);
		}

		public void Aborted(Exception e)
		{
			this.tx.Rollback(e, this.enlisted);
		}

		[MonoTODO]
		public void Committed()
		{
		}

		[MonoTODO("Not implemented")]
		public void InDoubt()
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Not implemented")]
		public void InDoubt(Exception e)
		{
			throw new NotImplementedException();
		}

		private Transaction tx;

		private ISinglePhaseNotification enlisted;
	}
}
