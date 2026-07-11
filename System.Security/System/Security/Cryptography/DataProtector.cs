using System;
using System.Collections.Generic;
using Unity;

namespace System.Security.Cryptography
{
	public abstract class DataProtector
	{
		protected DataProtector(string applicationName, string primaryPurpose, string[] specificPurposes)
		{
			ThrowStub.ThrowNotSupportedException();
		}

		protected string ApplicationName
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		protected virtual bool PrependHashedPurposeToPlaintext
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return default(bool);
			}
		}

		protected string PrimaryPurpose
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		protected IEnumerable<string> SpecificPurposes
		{
			get
			{
				ThrowStub.ThrowNotSupportedException();
				return 0;
			}
		}

		public static DataProtector Create(string providerClass, string applicationName, string primaryPurpose, string[] specificPurposes)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		protected virtual byte[] GetHashedPurpose()
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		public abstract bool IsReprotectRequired(byte[] encryptedData);

		public byte[] Protect(byte[] userData)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		protected abstract byte[] ProviderProtect(byte[] userData);

		protected abstract byte[] ProviderUnprotect(byte[] encryptedData);

		public byte[] Unprotect(byte[] encryptedData)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}
	}
}
