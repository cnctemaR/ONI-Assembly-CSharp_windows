using System;

namespace System.Transactions
{
	public struct TransactionOptions
	{
		internal TransactionOptions(IsolationLevel level, TimeSpan timeout)
		{
			this.level = level;
			this.timeout = timeout;
		}

		public IsolationLevel IsolationLevel
		{
			get
			{
				return this.level;
			}
			set
			{
				this.level = value;
			}
		}

		public TimeSpan Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		public static bool operator ==(TransactionOptions x, TransactionOptions y)
		{
			return x.level == y.level && x.timeout == y.timeout;
		}

		public static bool operator !=(TransactionOptions x, TransactionOptions y)
		{
			return x.level != y.level || x.timeout != y.timeout;
		}

		public override bool Equals(object obj)
		{
			return obj is TransactionOptions && this == (TransactionOptions)obj;
		}

		public override int GetHashCode()
		{
			return (int)(this.level ^ (IsolationLevel)this.timeout.GetHashCode());
		}

		private IsolationLevel level;

		private TimeSpan timeout;
	}
}
