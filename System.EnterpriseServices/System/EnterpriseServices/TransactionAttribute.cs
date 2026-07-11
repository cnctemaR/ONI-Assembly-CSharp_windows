using System;
using System.Runtime.InteropServices;

namespace System.EnterpriseServices
{
	[AttributeUsage(AttributeTargets.Class)]
	[ComVisible(false)]
	public sealed class TransactionAttribute : Attribute
	{
		public TransactionAttribute()
			: this(TransactionOption.Required)
		{
		}

		public TransactionAttribute(TransactionOption val)
		{
			this.isolation = TransactionIsolationLevel.Serializable;
			this.timeout = -1;
			this.val = val;
		}

		public TransactionIsolationLevel Isolation
		{
			get
			{
				return this.isolation;
			}
			set
			{
				this.isolation = value;
			}
		}

		public int Timeout
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

		public TransactionOption Value
		{
			get
			{
				return this.val;
			}
		}

		private TransactionIsolationLevel isolation;

		private int timeout;

		private TransactionOption val;
	}
}
