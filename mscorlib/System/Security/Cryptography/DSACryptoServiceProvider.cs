using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class DSACryptoServiceProvider : DSA, ICspAsymmetricAlgorithm
	{
		public DSACryptoServiceProvider()
			: this(1024)
		{
		}

		public DSACryptoServiceProvider(CspParameters parameters)
			: this(1024, parameters)
		{
		}

		public DSACryptoServiceProvider(int dwKeySize)
		{
			this.privateKeyExportable = true;
			base..ctor();
			this.Common(dwKeySize, false);
		}

		public DSACryptoServiceProvider(int dwKeySize, CspParameters parameters)
		{
			this.privateKeyExportable = true;
			base..ctor();
			bool flag = parameters != null;
			this.Common(dwKeySize, flag);
			if (flag)
			{
				this.Common(parameters);
			}
		}

		private void Common(int dwKeySize, bool parameters)
		{
			this.LegalKeySizesValue = new KeySizes[1];
			this.LegalKeySizesValue[0] = new KeySizes(512, 1024, 64);
			this.KeySize = dwKeySize;
			this.dsa = new DSAManaged(dwKeySize);
			this.dsa.KeyGenerated += this.OnKeyGenerated;
			this.persistKey = parameters;
			if (parameters)
			{
				return;
			}
			CspParameters cspParameters = new CspParameters(13);
			if (DSACryptoServiceProvider.useMachineKeyStore)
			{
				cspParameters.Flags |= CspProviderFlags.UseMachineKeyStore;
			}
			this.store = new KeyPairPersistence(cspParameters);
		}

		private void Common(CspParameters parameters)
		{
			this.store = new KeyPairPersistence(parameters);
			this.store.Load();
			if (this.store.KeyValue != null)
			{
				this.persisted = true;
				this.FromXmlString(this.store.KeyValue);
			}
			this.privateKeyExportable = (parameters.Flags & CspProviderFlags.UseNonExportableKey) == CspProviderFlags.NoFlags;
		}

		~DSACryptoServiceProvider()
		{
			this.Dispose(false);
		}

		public override string KeyExchangeAlgorithm
		{
			get
			{
				return null;
			}
		}

		public override int KeySize
		{
			get
			{
				return this.dsa.KeySize;
			}
		}

		public bool PersistKeyInCsp
		{
			get
			{
				return this.persistKey;
			}
			set
			{
				this.persistKey = value;
			}
		}

		[ComVisible(false)]
		public bool PublicOnly
		{
			get
			{
				return this.dsa.PublicOnly;
			}
		}

		public override string SignatureAlgorithm
		{
			get
			{
				return "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
			}
		}

		public static bool UseMachineKeyStore
		{
			get
			{
				return DSACryptoServiceProvider.useMachineKeyStore;
			}
			set
			{
				DSACryptoServiceProvider.useMachineKeyStore = value;
			}
		}

		public override DSAParameters ExportParameters(bool includePrivateParameters)
		{
			if (includePrivateParameters && !this.privateKeyExportable)
			{
				throw new CryptographicException(Locale.GetText("Cannot export private key"));
			}
			return this.dsa.ExportParameters(includePrivateParameters);
		}

		public override void ImportParameters(DSAParameters parameters)
		{
			this.dsa.ImportParameters(parameters);
		}

		public override byte[] CreateSignature(byte[] rgbHash)
		{
			return this.dsa.CreateSignature(rgbHash);
		}

		public byte[] SignData(byte[] buffer)
		{
			byte[] array = SHA1.Create().ComputeHash(buffer);
			return this.dsa.CreateSignature(array);
		}

		public byte[] SignData(byte[] buffer, int offset, int count)
		{
			byte[] array = SHA1.Create().ComputeHash(buffer, offset, count);
			return this.dsa.CreateSignature(array);
		}

		public byte[] SignData(Stream inputStream)
		{
			byte[] array = SHA1.Create().ComputeHash(inputStream);
			return this.dsa.CreateSignature(array);
		}

		public byte[] SignHash(byte[] rgbHash, string str)
		{
			if (string.Compare(str, "SHA1", true, CultureInfo.InvariantCulture) != 0)
			{
				throw new CryptographicException(Locale.GetText("Only SHA1 is supported."));
			}
			return this.dsa.CreateSignature(rgbHash);
		}

		public bool VerifyData(byte[] rgbData, byte[] rgbSignature)
		{
			byte[] array = SHA1.Create().ComputeHash(rgbData);
			return this.dsa.VerifySignature(array, rgbSignature);
		}

		public bool VerifyHash(byte[] rgbHash, string str, byte[] rgbSignature)
		{
			if (str == null)
			{
				str = "SHA1";
			}
			if (string.Compare(str, "SHA1", true, CultureInfo.InvariantCulture) != 0)
			{
				throw new CryptographicException(Locale.GetText("Only SHA1 is supported."));
			}
			return this.dsa.VerifySignature(rgbHash, rgbSignature);
		}

		public override bool VerifySignature(byte[] rgbHash, byte[] rgbSignature)
		{
			return this.dsa.VerifySignature(rgbHash, rgbSignature);
		}

		protected override byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm)
		{
			if (hashAlgorithm != HashAlgorithmName.SHA1)
			{
				throw new CryptographicException(Environment.GetResourceString("'{0}' is not a known hash algorithm.", new object[] { hashAlgorithm.Name }));
			}
			return HashAlgorithm.Create(hashAlgorithm.Name).ComputeHash(data, offset, count);
		}

		protected override byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm)
		{
			if (hashAlgorithm != HashAlgorithmName.SHA1)
			{
				throw new CryptographicException(Environment.GetResourceString("'{0}' is not a known hash algorithm.", new object[] { hashAlgorithm.Name }));
			}
			return HashAlgorithm.Create(hashAlgorithm.Name).ComputeHash(data);
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.m_disposed)
			{
				if (this.persisted && !this.persistKey)
				{
					this.store.Remove();
				}
				if (this.dsa != null)
				{
					this.dsa.Clear();
				}
				this.m_disposed = true;
			}
		}

		private void OnKeyGenerated(object sender, EventArgs e)
		{
			if (this.persistKey && !this.persisted)
			{
				this.store.KeyValue = this.ToXmlString(!this.dsa.PublicOnly);
				this.store.Save();
				this.persisted = true;
			}
		}

		[ComVisible(false)]
		[MonoTODO("call into KeyPairPersistence to get details")]
		public CspKeyContainerInfo CspKeyContainerInfo
		{
			[SecuritySafeCritical]
			get
			{
				return null;
			}
		}

		[ComVisible(false)]
		[SecuritySafeCritical]
		public byte[] ExportCspBlob(bool includePrivateParameters)
		{
			byte[] array;
			if (includePrivateParameters)
			{
				array = CryptoConvert.ToCapiPrivateKeyBlob(this);
			}
			else
			{
				array = CryptoConvert.ToCapiPublicKeyBlob(this);
			}
			return array;
		}

		[SecuritySafeCritical]
		[ComVisible(false)]
		public void ImportCspBlob(byte[] keyBlob)
		{
			if (keyBlob == null)
			{
				throw new ArgumentNullException("keyBlob");
			}
			DSA dsa = CryptoConvert.FromCapiKeyBlobDSA(keyBlob);
			if (dsa is DSACryptoServiceProvider)
			{
				DSAParameters dsaparameters = dsa.ExportParameters(!(dsa as DSACryptoServiceProvider).PublicOnly);
				this.ImportParameters(dsaparameters);
				return;
			}
			try
			{
				DSAParameters dsaparameters2 = dsa.ExportParameters(true);
				this.ImportParameters(dsaparameters2);
			}
			catch
			{
				DSAParameters dsaparameters3 = dsa.ExportParameters(false);
				this.ImportParameters(dsaparameters3);
			}
		}

		private const int PROV_DSS_DH = 13;

		private KeyPairPersistence store;

		private bool persistKey;

		private bool persisted;

		private bool privateKeyExportable;

		private bool m_disposed;

		private DSAManaged dsa;

		private static bool useMachineKeyStore;
	}
}
