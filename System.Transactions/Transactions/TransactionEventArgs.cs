using System;

namespace System.Transactions
{
	public class TransactionEventArgs : EventArgs
	{
		public TransactionEventArgs()
		{
		}

		internal TransactionEventArgs(Transaction transaction)
			: this()
		{
			this.transaction = transaction;
		}

		public Transaction Transaction
		{
			get
			{
				return this.transaction;
			}
		}

		private Transaction transaction;
	}
}
