using System;

namespace System.Transactions
{
	public class SinglePhaseEnlistment : Enlistment
	{
		internal SinglePhaseEnlistment()
		{
		}

		internal SinglePhaseEnlistment(Transaction tx, object abortingEnlisted)
		{
			this.tx = tx;
			this.abortingEnlisted = abortingEnlisted;
		}

		public void Aborted()
		{
			this.Aborted(null);
		}

		public void Aborted(Exception e)
		{
			if (this.tx != null)
			{
				this.tx.Rollback(e, this.abortingEnlisted);
			}
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

		private object abortingEnlisted;
	}
}
