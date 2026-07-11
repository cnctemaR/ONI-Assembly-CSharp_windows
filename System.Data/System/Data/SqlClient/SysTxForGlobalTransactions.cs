using System;
using System.Reflection;
using System.Transactions;

namespace System.Data.SqlClient
{
	internal static class SysTxForGlobalTransactions
	{
		public static MethodInfo EnlistPromotableSinglePhase
		{
			get
			{
				return SysTxForGlobalTransactions._enlistPromotableSinglePhase.Value;
			}
		}

		public static MethodInfo SetDistributedTransactionIdentifier
		{
			get
			{
				return SysTxForGlobalTransactions._setDistributedTransactionIdentifier.Value;
			}
		}

		public static MethodInfo GetPromotedToken
		{
			get
			{
				return SysTxForGlobalTransactions._getPromotedToken.Value;
			}
		}

		private static readonly Lazy<MethodInfo> _enlistPromotableSinglePhase = new Lazy<MethodInfo>(() => typeof(Transaction).GetMethod("EnlistPromotableSinglePhase", new Type[]
		{
			typeof(IPromotableSinglePhaseNotification),
			typeof(Guid)
		}));

		private static readonly Lazy<MethodInfo> _setDistributedTransactionIdentifier = new Lazy<MethodInfo>(() => typeof(Transaction).GetMethod("SetDistributedTransactionIdentifier", new Type[]
		{
			typeof(IPromotableSinglePhaseNotification),
			typeof(Guid)
		}));

		private static readonly Lazy<MethodInfo> _getPromotedToken = new Lazy<MethodInfo>(() => typeof(Transaction).GetMethod("GetPromotedToken"));
	}
}
