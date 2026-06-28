using System;
using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public class DesignerTransactionCloseEventArgs : EventArgs
	{
		public DesignerTransactionCloseEventArgs(bool commit, bool lastTransaction)
		{
			this.commit = commit;
			this.last_transaction = lastTransaction;
		}

		[Obsolete("Use another constructor that indicates lastTransaction")]
		public DesignerTransactionCloseEventArgs(bool commit)
		{
			this.commit = commit;
		}

		public bool LastTransaction
		{
			get
			{
				return this.last_transaction;
			}
		}

		public bool TransactionCommitted
		{
			get
			{
				return this.commit;
			}
		}

		private bool commit;

		private bool last_transaction;
	}
}
