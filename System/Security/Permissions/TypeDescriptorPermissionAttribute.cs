using System;
using Unity;

namespace System.Security.Permissions
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[Serializable]
	public sealed class TypeDescriptorPermissionAttribute : CodeAccessSecurityAttribute
	{
		public TypeDescriptorPermissionAttribute(SecurityAction action)
		{
		}

		public TypeDescriptorPermissionFlags Flags
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return TypeDescriptorPermissionFlags.NoFlags;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public bool RestrictedRegistrationAccess
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public override IPermission CreatePermission()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
