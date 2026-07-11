using System;

namespace System.Transactions
{
	public static class TransactionManager
	{
		public static event TransactionStartedEventHandler DistributedTransactionStarted;

		public static TimeSpan DefaultTimeout
		{
			get
			{
				return TransactionManager.defaultTimeout;
			}
		}

		[MonoTODO("Not implemented")]
		public static HostCurrentTransactionCallback HostCurrentCallback
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public static TimeSpan MaximumTimeout
		{
			get
			{
				return TransactionManager.maxTimeout;
			}
		}

		[MonoTODO("Not implemented")]
		public static void RecoveryComplete(Guid manager)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Not implemented")]
		public static Enlistment Reenlist(Guid manager, byte[] recoveryInfo, IEnlistmentNotification notification)
		{
			throw new NotImplementedException();
		}

		private static TimeSpan defaultTimeout = new TimeSpan(0, 1, 0);

		private static TimeSpan maxTimeout = new TimeSpan(0, 10, 0);
	}
}
