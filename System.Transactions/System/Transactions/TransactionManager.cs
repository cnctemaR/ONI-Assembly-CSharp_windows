using System;
using System.Configuration;
using System.Transactions.Configuration;

namespace System.Transactions
{
	public static class TransactionManager
	{
		public static TimeSpan DefaultTimeout
		{
			get
			{
				if (TransactionManager.defaultSettings != null)
				{
					return TransactionManager.defaultSettings.Timeout;
				}
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
				if (TransactionManager.machineSettings != null)
				{
					return TransactionManager.machineSettings.MaxTimeout;
				}
				return TransactionManager.maxTimeout;
			}
		}

		[MonoTODO("Not implemented")]
		public static void RecoveryComplete(Guid resourceManagerIdentifier)
		{
			throw new NotImplementedException();
		}

		[MonoTODO("Not implemented")]
		public static Enlistment Reenlist(Guid resourceManagerIdentifier, byte[] recoveryInformation, IEnlistmentNotification enlistmentNotification)
		{
			throw new NotImplementedException();
		}

		public static event TransactionStartedEventHandler DistributedTransactionStarted;

		private static DefaultSettingsSection defaultSettings = ConfigurationManager.GetSection("system.transactions/defaultSettings") as DefaultSettingsSection;

		private static MachineSettingsSection machineSettings = ConfigurationManager.GetSection("system.transactions/machineSettings") as MachineSettingsSection;

		private static TimeSpan defaultTimeout = new TimeSpan(0, 1, 0);

		private static TimeSpan maxTimeout = new TimeSpan(0, 10, 0);
	}
}
