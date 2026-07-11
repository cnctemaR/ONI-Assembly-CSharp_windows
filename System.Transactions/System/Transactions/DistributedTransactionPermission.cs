using System;
using System.Security;
using System.Security.Permissions;
using Unity;

namespace System.Transactions
{
	[Serializable]
	public sealed class DistributedTransactionPermission : CodeAccessPermission, IUnrestrictedPermission
	{
		public DistributedTransactionPermission(PermissionState state)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		public override IPermission Copy()
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		public override void FromXml(SecurityElement securityElement)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		public override IPermission Intersect(IPermission target)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		public override bool IsSubsetOf(IPermission target)
		{
			ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		public bool IsUnrestricted()
		{
			ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		public override SecurityElement ToXml()
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
