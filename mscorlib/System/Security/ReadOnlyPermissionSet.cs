using System;
using System.Collections;
using Unity;

namespace System.Security
{
	[Serializable]
	public sealed class ReadOnlyPermissionSet : PermissionSet
	{
		public ReadOnlyPermissionSet(SecurityElement permissionSetXml)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		protected override IPermission AddPermissionImpl(IPermission perm)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		protected override IEnumerator GetEnumeratorImpl()
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		protected override IPermission GetPermissionImpl(Type permClass)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		protected override IPermission RemovePermissionImpl(Type permClass)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		protected override IPermission SetPermissionImpl(IPermission perm)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
