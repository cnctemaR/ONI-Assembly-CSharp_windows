using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Mono.Security.Cryptography;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public sealed class RSACryptoServiceProvider : RSA, ICspAsymmetricAlgorithm
	{
		public RSACryptoServiceProvider()
		{
			this.Common(1024, null);
		}

		public RSACryptoServiceProvider(CspParameters parameters)
		{
			this.Common(1024, parameters);
		}

		public RSACryptoServiceProvider(int dwKeySize)
		{
			this.Common(dwKeySize, null);
		}

		public RSACryptoServiceProvider(int dwKeySize, CspParameters parameters)
		{
			this.Common(dwKeySize, parameters);
		}

		private void Common(int dwKeySize, CspParameters p)
		{
			this.LegalKeySizesValue = new KeySizes[1];
			this.LegalKeySizesValue[0] = new KeySizes(384, 16384, 8);
			base.KeySize = dwKeySize;
			this.rsa = new RSAManaged(this.KeySize);
			this.rsa.KeyGenerated += this.OnKeyGenerated;
			this.persistKey = p != null;
			if (p == null)
			{
				p = new CspParameters(1);
				if (RSACryptoServiceProvider.useMachineKeyStore)
				{
					p.Flags |= CspProviderFlags.UseMachineKeyStore;
				}
				this.store = new KeyPairPersistence(p);
			}
			else
			{
				this.store = new KeyPairPersistence(p);
				this.store.Load();
				if (this.store.KeyValue != null)
				{
					this.persisted = true;
					this.FromXmlString(this.store.KeyValue);
				}
			}
		}

		public static bool UseMachineKeyStore
		{
			get
			{
				return RSACryptoServiceProvider.useMachineKeyStore;
			}
			set
			{
				RSACryptoServiceProvider.useMachineKeyStore = value;
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

		public override string SignatureAlgorithm
		{
			get
			{
				return "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
			}
		}

		public byte[] Decrypt(byte[] rgb, bool fOAEP)
		{
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
			return this.rsa.ExportParameters(includePrivateParameters);
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
				hashAlgorithm = HashAlgorithm.Create((string)halg);
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
			return hashAlgorithm;
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
			if (oid != null)
			{
				if (RSACryptoServiceProvider.<>f__switch$map2D == null)
				{
					RSACryptoServiceProvider.<>f__switch$map2D = new Dictionary<string, int>(2)
					{
						{ "1.3.14.3.2.26", 0 },
						{ "1.2.840.113549.2.5", 1 }
					};
				}
				int num;
				if (RSACryptoServiceProvider.<>f__switch$map2D.TryGetValue(oid, out num))
				{
					if (num == 0)
					{
						return "SHA1";
					}
					if (num == 1)
					{
						return "MD5";
					}
				}
			}
			throw new NotSupportedException(oid + " is an unsupported hash algorithm for RSA signing");
		}

		public byte[] SignHash(byte[] rgbHash, string str)
		{
			if (rgbHash == null)
			{
				throw new ArgumentNullException("rgbHash");
			}
			string text = ((str != null) ? this.GetHashNameFromOID(str) : "SHA1");
			HashAlgorithm hashAlgorithm = HashAlgorithm.Create(text);
			return PKCS1.Sign_v15(this, hashAlgorithm, rgbHash);
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
			string text = ((str != null) ? this.GetHashNameFromOID(str) : "SHA1");
			HashAlgorithm hashAlgorithm = HashAlgorithm.Create(text);
			return PKCS1.Verify_v15(this, hashAlgorithm, rgbHash, rgbSignature);
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
		[MonoTODO("Always return null")]
		public CspKeyContainerInfo CspKeyContainerInfo
		{
			get
			{
				return null;
			}
		}

		[ComVisible(false)]
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
			array[5] = 164;
			return array;
		}

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
		}

		private const int PROV_RSA_FULL = 1;

		private KeyPairPersistence store;

		private bool persistKey;

		private bool persisted;

		private bool privateKeyExportable = true;

		private bool m_disposed;

		private RSAManaged rsa;

		private static bool useMachineKeyStore;
	}
}
