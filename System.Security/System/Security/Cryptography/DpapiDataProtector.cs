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
			ThrowStub.ThrowNotSupportedException();
		}

		protected override bool PrependHashedPurposeToPlaintext
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		public DataProtectionScope Scope
		{
			[CompilerGenerated]
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return DataProtectionScope.CurrentUser;
			}
			[CompilerGenerated]
			set
			{
				ThrowStub.ThrowNotSupportedException();
			}
		}

		public override bool IsReprotectRequired(byte[] encryptedData)
		{
			ThrowStub.ThrowNotSupportedException();
			return default(bool);
		}

		[SecuritySafeCritical]
		[DataProtectionPermission(SecurityAction.Assert, ProtectData = true)]
		protected override byte[] ProviderProtect(byte[] userData)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		[SecuritySafeCritical]
		[DataProtectionPermission(SecurityAction.Assert, UnprotectData = true)]
		protected override byte[] ProviderUnprotect(byte[] encryptedData)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
