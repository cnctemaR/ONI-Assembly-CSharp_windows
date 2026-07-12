using System;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using Unity;

namespace System.Security.Cryptography
{
	public sealed class DpapiDataProtector : DataProtector
	{
		[SecuritySafeCritical]
		[DataProtectionPermission(SecurityAction.Demand, Unrestricted = true)]
		public DpapiDataProtector(string appName, string primaryPurpose, string[] specificPurpose)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public DataProtectionScope Scope
		{
			[CompilerGenerated]
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return DataProtectionScope.CurrentUser;
			}
			[CompilerGenerated]
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public override bool IsReprotectRequired(byte[] encryptedData)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		[SecuritySafeCritical]
		[DataProtectionPermission(SecurityAction.Assert, ProtectData = true)]
		protected override byte[] ProviderProtect(byte[] userData)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}

		[SecuritySafeCritical]
		[DataProtectionPermission(SecurityAction.Assert, UnprotectData = true)]
		protected override byte[] ProviderUnprotect(byte[] encryptedData)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
