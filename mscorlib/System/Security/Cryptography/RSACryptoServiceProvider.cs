using System;
using System.IO;
using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class RSACryptoServiceProvider : RSA, ICspAsymmetricAlgorithm
	{
		public override string SignatureAlgorithm
		{
			get
			{
				return "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
			}
		}

		public static bool UseMachineKeyStore
		{
			get
			{
				return RSACryptoServiceProvider.s_UseMachineKeyStore == CspProviderFlags.UseMachineKeyStore;
			}
			set
			{
				RSACryptoServiceProvider.s_UseMachineKeyStore = (value ? CspProviderFlags.UseMachineKeyStore : CspProviderFlags.NoFlags);
			}
		}

		[SecuritySafeCritical]
		protected override byte[] HashData(byte[] data, int offset, int count, HashAlgorithmName hashAlgorithm)
		{
			return HashAlgorithm.Create(hashAlgorithm.Name).ComputeHash(data, offset, count);
		}

		[SecuritySafeCritical]
		protected override byte[] HashData(Stream data, HashAlgorithmName hashAlgorithm)
		{
			return HashAlgorithm.Create(hashAlgorithm.Name).ComputeHash(data);
		}

		private static int GetAlgorithmId(HashAlgorithmName hashAlgorithm)
		{
			string name = hashAlgorithm.Name;
			if (name == "MD5")
			{
				return 32771;
			}
			if (name == "SHA1")
			{
				return 32772;
			}
			if (name == "SHA256")
			{
				return 32780;
			}
			if (name == "SHA384")
			{
				return 32781;
			}
			if (!(name == "SHA512"))
			{
				throw new CryptographicException(Environment.GetResourceString("'{0}' is not a known hash algorithm.", new object[] { hashAlgorithm.Name }));
			}
			return 32782;
		}

		public override byte[] Encrypt(byte[] data, RSAEncryptionPadding padding)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (padding == null)
			{
				throw new ArgumentNullException("padding");
			}
			if (padding == RSAEncryptionPadding.Pkcs1)
			{
				return this.Encrypt(data, false);
			}
			if (padding == RSAEncryptionPadding.OaepSHA1)
			{
				return this.Encrypt(data, true);
			}
			throw RSACryptoServiceProvider.PaddingModeNotSupported();
		}

		public override byte[] Decrypt(byte[] data, RSAEncryptionPadding padding)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (padding == null)
			{
				throw new ArgumentNullException("padding");
			}
			if (padding == RSAEncryptionPadding.Pkcs1)
			{
				return this.Decrypt(data, false);
			}
			if (padding == RSAEncryptionPadding.OaepSHA1)
			{
				return this.Decrypt(data, true);
			}
			throw RSACryptoServiceProvider.PaddingModeNotSupported();
		}

		public override byte[] SignHash(byte[] hash, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			if (hash == null)
			{
				throw new ArgumentNullException("hash");
			}
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw RSA.HashAlgorithmNameNullOrEmpty();
			}
			if (padding == null)
			{
				throw new ArgumentNullException("padding");
			}
			if (padding != RSASignaturePadding.Pkcs1)
			{
				throw RSACryptoServiceProvider.PaddingModeNotSupported();
			}
			return this.SignHash(hash, RSACryptoServiceProvider.GetAlgorithmId(hashAlgorithm));
		}

		public override bool VerifyHash(byte[] hash, byte[] signature, HashAlgorithmName hashAlgorithm, RSASignaturePadding padding)
		{
			if (hash == null)
			{
				throw new ArgumentNullException("hash");
			}
			if (signature == null)
			{
				throw new ArgumentNullException("signature");
			}
			if (string.IsNullOrEmpty(hashAlgorithm.Name))
			{
				throw RSA.HashAlgorithmNameNullOrEmpty();
			}
			if (padding == null)
			{
				throw new ArgumentNullException("padding");
			}
			if (padding != RSASignaturePadding.Pkcs1)
			{
				throw RSACryptoServiceProvider.PaddingModeNotSupported();
			}
			return this.VerifyHash(hash, RSACryptoServiceProvider.GetAlgorithmId(hashAlgorithm), signature);
		}

		private static Exception PaddingModeNotSupported()
		{
			return new CryptographicException(Environment.GetResourceString("Specified padding mode is not valid for this algorithm."));
		}

		public RSACryptoServiceProvider()
			: this(1024)
		{
		}

		public RSACryptoServiceProvider(CspParameters parameters)
			: this(1024, parameters)
		{
		}

		public RSACryptoServiceProvider(int dwKeySize)
		{
			this.privateKeyExportable = true;
			base..ctor();
			this.Common(dwKeySize, false);
		}

		public RSACryptoServiceProvider(int dwKeySize, CspParameters parameters)
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
			this.LegalKeySizesValue[0] = new KeySizes(384, 16384, 8);
			base.KeySize = dwKeySize;
			this.rsa = new RSAManaged(this.KeySize);
			this.rsa.KeyGenerated += this.OnKeyGenerated;
			this.persistKey = parameters;
			if (parameters)
			{
				return;
			}
			CspParameters cspParameters = new CspParameters(1);
			if (RSACryptoServiceProvider.UseMachineKeyStore)
			{
				cspParameters.Flags |= CspProviderFlags.UseMachineKeyStore;
			}
			this.store = new KeyPairPersistence(cspParameters);
		}

		private void Common(CspParameters p)
		{
			this.store = new KeyPairPersistence(p);
			bool flag = this.store.Load();
			bool flag2 = (p.Flags & CspProviderFlags.UseExistingKey) > CspProviderFlags.NoFlags;
			this.privateKeyExportable = (p.Flags & CspProviderFlags.UseNonExportableKey) == CspProviderFlags.NoFlags;
			if (flag2 && !flag)
			{
				throw new CryptographicException("Keyset does not exist");
			}
			if (this.store.KeyValue != null)
			{
				this.persisted = true;
				this.FromXmlString(this.store.KeyValue);
			}
		}

		~RSACryptoServiceProvider()
		{
			this.Dispose(false);
		}

		public override string KeyExchangeAlgorithm
		{
			get
			{
				return "RSA-PKCS1-KeyEx";
			}
		}

		public override int KeySize
		{
			get
			{
				if (this.rsa == null)
				{
					return this.KeySizeValue;
				}
				return this.rsa.KeySize;
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
				if (this.persistKey)
				{
					this.OnKeyGenerated(this.rsa, null);
				}
			}
		}

		[ComVisible(false)]
		public bool PublicOnly
		{
			get
			{
				return this.rsa.PublicOnly;
			}
		}

		public byte[] Decrypt(byte[] rgb, bool fOAEP)
		{
			if (rgb == null)
			{
				throw new ArgumentNullException("rgb");
			}
			if (rgb.Length > this.KeySize / 8)
			{
				throw new CryptographicException(Environment.GetResourceString("The data to be decrypted exceeds the maximum for this modulus of {0} bytes.", new object[] { this.KeySize / 8 }));
			}
			if (this.m_disposed)
			{
				throw new ObjectDisposedException("rsa");
			}
			AsymmetricKeyExchangeDeformatter asymmetricKeyExchangeDeformatter;
			if (fOAEP)
			{
				asymmetricKeyExchangeDeformatter = new RSAOAEPKeyExchangeDeformatter(this.rsa);
			}
			else
			{
				asymmetricKeyExchangeDeformatter = new RSAPKCS1KeyExchangeDeformatter(this.rsa);
			}
			return asymmetricKeyExchangeDeformatter.DecryptKeyExchange(rgb);
		}

		public override byte[] DecryptValue(byte[] rgb)
		{
			if (!this.rsa.IsCrtPossible)
			{
				throw new CryptographicException("Incomplete private key - missing CRT.");
			}
			return this.rsa.DecryptValue(rgb);
		}

		public byte[] Encrypt(byte[] rgb, bool fOAEP)
		{
			AsymmetricKeyExchangeFormatter asymmetricKeyExchangeFormatter;
			if (fOAEP)
			{
				asymmetricKeyExchangeFormatter = new RSAOAEPKeyExchangeFormatter(this.rsa);
			}
			else
			{
				asymmetricKeyExchangeFormatter = new RSAPKCS1KeyExchangeFormatter(this.rsa);
			}
			return asymmetricKeyExchangeFormatter.CreateKeyExchange(rgb);
		}

		public override byte[] EncryptValue(byte[] rgb)
		{
			return this.rsa.EncryptValue(rgb);
		}

		public override RSAParameters ExportParameters(bool includePrivateParameters)
		{
			if (includePrivateParameters && !this.privateKeyExportable)
			{
				throw new CryptographicException("cannot export private key");
			}
			RSAParameters rsaparameters = this.rsa.ExportParameters(includePrivateParameters);
			if (includePrivateParameters)
			{
				if (rsaparameters.D == null)
				{
					throw new ArgumentNullException("Missing D parameter for the private key.");
				}
				if (rsaparameters.P == null || rsaparameters.Q == null || rsaparameters.DP == null || rsaparameters.DQ == null || rsaparameters.InverseQ == null)
				{
					throw new CryptographicException("Missing some CRT parameters for the private key.");
				}
			}
			return rsaparameters;
		}

		public override void ImportParameters(RSAParameters parameters)
		{
			this.rsa.ImportParameters(parameters);
		}

		private HashAlgorithm GetHash(object halg)
		{
			if (halg == null)
			{
				throw new ArgumentNullException("halg");
			}
			HashAlgorithm hashAlgorithm;
			if (halg is string)
			{
				hashAlgorithm = this.GetHashFromString((string)halg);
			}
			else if (halg is HashAlgorithm)
			{
				hashAlgorithm = (HashAlgorithm)halg;
			}
			else
			{
				if (!(halg is Type))
				{
					throw new ArgumentException("halg");
				}
				hashAlgorithm = (HashAlgorithm)Activator.CreateInstance((Type)halg);
			}
			if (hashAlgorithm == null)
			{
				throw new ArgumentException("Could not find provider for halg='" + halg + "'.", "halg");
			}
			return hashAlgorithm;
		}

		private HashAlgorithm GetHashFromString(string name)
		{
			HashAlgorithm hashAlgorithm = HashAlgorithm.Create(name);
			if (hashAlgorithm != null)
			{
				return hashAlgorithm;
			}
			HashAlgorithm hashAlgorithm2;
			try
			{
				hashAlgorithm2 = HashAlgorithm.Create(this.GetHashNameFromOID(name));
			}
			catch (CryptographicException ex)
			{
				throw new ArgumentException(ex.Message, "halg", ex);
			}
			return hashAlgorithm2;
		}

		public byte[] SignData(byte[] buffer, object halg)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			return this.SignData(buffer, 0, buffer.Length, halg);
		}

		public byte[] SignData(Stream inputStream, object halg)
		{
			HashAlgorithm hash = this.GetHash(halg);
			byte[] array = hash.ComputeHash(inputStream);
			return PKCS1.Sign_v15(this, hash, array);
		}

		public byte[] SignData(byte[] buffer, int offset, int count, object halg)
		{
			HashAlgorithm hash = this.GetHash(halg);
			byte[] array = hash.ComputeHash(buffer, offset, count);
			return PKCS1.Sign_v15(this, hash, array);
		}

		private string GetHashNameFromOID(string oid)
		{
			if (oid == "1.3.14.3.2.26")
			{
				return "SHA1";
			}
			if (oid == "1.2.840.113549.2.5")
			{
				return "MD5";
			}
			if (oid == "2.16.840.1.101.3.4.2.1")
			{
				return "SHA256";
			}
			if (oid == "2.16.840.1.101.3.4.2.2")
			{
				return "SHA384";
			}
			if (!(oid == "2.16.840.1.101.3.4.2.3"))
			{
				throw new CryptographicException(oid + " is an unsupported hash algorithm for RSA signing");
			}
			return "SHA512";
		}

		public byte[] SignHash(byte[] rgbHash, string str)
		{
			if (rgbHash == null)
			{
				throw new ArgumentNullException("rgbHash");
			}
			HashAlgorithm hashAlgorithm = HashAlgorithm.Create((str == null) ? "SHA1" : this.GetHashNameFromOID(str));
			return PKCS1.Sign_v15(this, hashAlgorithm, rgbHash);
		}

		private byte[] SignHash(byte[] rgbHash, int calgHash)
		{
			return PKCS1.Sign_v15(this, RSACryptoServiceProvider.InternalHashToHashAlgorithm(calgHash), rgbHash);
		}

		private static HashAlgorithm InternalHashToHashAlgorithm(int calgHash)
		{
			if (calgHash == 32771)
			{
				return MD5.Create();
			}
			if (calgHash == 32772)
			{
				return SHA1.Create();
			}
			switch (calgHash)
			{
			case 32780:
				return SHA256.Create();
			case 32781:
				return SHA384.Create();
			case 32782:
				return SHA512.Create();
			default:
				throw new NotImplementedException(calgHash.ToString());
			}
		}

		public bool VerifyData(byte[] buffer, object halg, byte[] signature)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (signature == null)
			{
				throw new ArgumentNullException("signature");
			}
			HashAlgorithm hash = this.GetHash(halg);
			byte[] array = hash.ComputeHash(buffer);
			return PKCS1.Verify_v15(this, hash, array, signature);
		}

		public bool VerifyHash(byte[] rgbHash, string str, byte[] rgbSignature)
		{
			if (rgbHash == null)
			{
				throw new ArgumentNullException("rgbHash");
			}
			if (rgbSignature == null)
			{
				throw new ArgumentNullException("rgbSignature");
			}
			HashAlgorithm hashAlgorithm = HashAlgorithm.Create((str == null) ? "SHA1" : this.GetHashNameFromOID(str));
			return PKCS1.Verify_v15(this, hashAlgorithm, rgbHash, rgbSignature);
		}

		private bool VerifyHash(byte[] rgbHash, int calgHash, byte[] rgbSignature)
		{
			return PKCS1.Verify_v15(this, RSACryptoServiceProvider.InternalHashToHashAlgorithm(calgHash), rgbHash, rgbSignature);
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.m_disposed)
			{
				if (this.persisted && !this.persistKey)
				{
					this.store.Remove();
				}
				if (this.rsa != null)
				{
					this.rsa.Clear();
				}
				this.m_disposed = true;
			}
		}

		private void OnKeyGenerated(object sender, EventArgs e)
		{
			if (this.persistKey && !this.persisted)
			{
				this.store.KeyValue = this.ToXmlString(!this.rsa.PublicOnly);
				this.store.Save();
				this.persisted = true;
			}
		}

		[ComVisible(false)]
		public CspKeyContainerInfo CspKeyContainerInfo
		{
			[SecuritySafeCritical]
			get
			{
				return new CspKeyContainerInfo(this.store.Parameters);
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
			array[5] = ((this.store != null && this.store.Parameters.KeyNumber == 2) ? 36 : 164);
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
			RSA rsa = CryptoConvert.FromCapiKeyBlob(keyBlob);
			if (rsa is RSACryptoServiceProvider)
			{
				RSAParameters rsaparameters = rsa.ExportParameters(!(rsa as RSACryptoServiceProvider).PublicOnly);
				this.ImportParameters(rsaparameters);
			}
			else
			{
				try
				{
					RSAParameters rsaparameters2 = rsa.ExportParameters(true);
					this.ImportParameters(rsaparameters2);
				}
				catch
				{
					RSAParameters rsaparameters3 = rsa.ExportParameters(false);
					this.ImportParameters(rsaparameters3);
				}
			}
			CspParameters cspParameters = new CspParameters(1);
			cspParameters.KeyNumber = ((keyBlob[5] == 36) ? 2 : 1);
			if (RSACryptoServiceProvider.UseMachineKeyStore)
			{
				cspParameters.Flags |= CspProviderFlags.UseMachineKeyStore;
			}
			this.store = new KeyPairPersistence(cspParameters);
		}

		private static volatile CspProviderFlags s_UseMachineKeyStore;

		private const int PROV_RSA_FULL = 1;

		private const int AT_KEYEXCHANGE = 1;

		private const int AT_SIGNATURE = 2;

		private KeyPairPersistence store;

		private bool persistKey;

		private bool persisted;

		private bool privateKeyExportable;

		private bool m_disposed;

		private RSAManaged rsa;
	}
}
