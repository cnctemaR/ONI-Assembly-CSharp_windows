using System;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Transactions
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	public sealed class DistributedTransactionPermissionAttribute : CodeAccessSecurityAttribute
	{
		public DistributedTransactionPermissionAttribute(SecurityAction action)
		{
		}

		public override IPermission CreatePermission()
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
