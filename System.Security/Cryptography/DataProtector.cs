using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Security.Cryptography
{
	public abstract class DataProtector
	{
		protected DataProtector(string applicationName, string primaryPurpose, string[] specificPurposes)
		{
			if (string.IsNullOrWhiteSpace(applicationName))
			{
				throw new ArgumentException("Invalid application name and/or purpose", "applicationName");
			}
			if (string.IsNullOrWhiteSpace(primaryPurpose))
			{
				throw new ArgumentException("Invalid application name and/or purpose", "primaryPurpose");
			}
			if (specificPurposes != null)
			{
				for (int i = 0; i < specificPurposes.Length; i++)
				{
					if (string.IsNullOrWhiteSpace(specificPurposes[i]))
					{
						throw new ArgumentException("Invalid application name and/or purpose", "specificPurposes");
					}
				}
			}
			this.m_applicationName = applicationName;
			this.m_primaryPurpose = primaryPurpose;
			List<string> list = new List<string>();
			if (specificPurposes != null)
			{
				list.AddRange(specificPurposes);
			}
			this.m_specificPurposes = list;
		}

		protected string ApplicationName
		{
			get
			{
				return this.m_applicationName;
			}
		}

		protected virtual bool PrependHashedPurposeToPlaintext
		{
			get
			{
				return true;
			}
		}

		protected virtual byte[] GetHashedPurpose()
		{
			if (this.m_hashedPurpose == null)
			{
				using (HashAlgorithm hashAlgorithm = HashAlgorithm.Create("System.Security.Cryptography.Sha256Cng"))
				{
					using (BinaryWriter binaryWriter = new BinaryWriter(new CryptoStream(new MemoryStream(), hashAlgorithm, CryptoStreamMode.Write), new UTF8Encoding(false, true)))
					{
						binaryWriter.Write(this.ApplicationName);
						binaryWriter.Write(this.PrimaryPurpose);
						foreach (string text in this.SpecificPurposes)
						{
							binaryWriter.Write(text);
						}
					}
					this.m_hashedPurpose = hashAlgorithm.Hash;
				}
			}
			return this.m_hashedPurpose;
		}

		public abstract bool IsReprotectRequired(byte[] encryptedData);

		protected string PrimaryPurpose
		{
			get
			{
				return this.m_primaryPurpose;
			}
		}

		protected IEnumerable<string> SpecificPurposes
		{
			get
			{
				return this.m_specificPurposes;
			}
		}

		public static DataProtector Create(string providerClass, string applicationName, string primaryPurpose, params string[] specificPurposes)
		{
			if (providerClass == null)
			{
				throw new ArgumentNullException("providerClass");
			}
			return (DataProtector)CryptoConfig.CreateFromName(providerClass, new object[] { applicationName, primaryPurpose, specificPurposes });
		}

		public byte[] Protect(byte[] userData)
		{
			if (userData == null)
			{
				throw new ArgumentNullException("userData");
			}
			if (this.PrependHashedPurposeToPlaintext)
			{
				byte[] hashedPurpose = this.GetHashedPurpose();
				byte[] array = new byte[userData.Length + hashedPurpose.Length];
				Array.Copy(hashedPurpose, 0, array, 0, hashedPurpose.Length);
				Array.Copy(userData, 0, array, hashedPurpose.Length, userData.Length);
				userData = array;
			}
			return this.ProviderProtect(userData);
		}

		protected abstract byte[] ProviderProtect(byte[] userData);

		protected abstract byte[] ProviderUnprotect(byte[] encryptedData);

		[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
		public byte[] Unprotect(byte[] encryptedData)
		{
			if (encryptedData == null)
			{
				throw new ArgumentNullException("encryptedData");
			}
			if (!this.PrependHashedPurposeToPlaintext)
			{
				return this.ProviderUnprotect(encryptedData);
			}
			byte[] array = this.ProviderUnprotect(encryptedData);
			byte[] hashedPurpose = this.GetHashedPurpose();
			bool flag = array.Length >= hashedPurpose.Length;
			for (int i = 0; i < hashedPurpose.Length; i++)
			{
				if (hashedPurpose[i] != array[i % array.Length])
				{
					flag = false;
				}
			}
			if (!flag)
			{
				throw new CryptographicException("Invalid data protection purpose");
			}
			byte[] array2 = new byte[array.Length - hashedPurpose.Length];
			Array.Copy(array, hashedPurpose.Length, array2, 0, array2.Length);
			return array2;
		}

		private string m_applicationName;

		private string m_primaryPurpose;

		private IEnumerable<string> m_specificPurposes;

		private volatile byte[] m_hashedPurpose;
	}
}
