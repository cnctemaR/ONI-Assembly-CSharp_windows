using System;

namespace System.Transactions
{
	[Serializable]
	public sealed class SubordinateTransaction : Transaction
	{
		public SubordinateTransaction(IsolationLevel isoLevel, ISimpleTransactionSuperior superior)
		{
			throw new NotImplementedException();
		}
	}
}
