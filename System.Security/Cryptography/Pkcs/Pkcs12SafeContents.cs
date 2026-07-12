using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace System.Security.Cryptography.Pkcs
{
	public sealed class Pkcs12SafeContents
	{
		public Pkcs12ConfidentialityMode ConfidentialityMode
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public bool IsReadOnly
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public Pkcs12CertBag AddCertificate(X509Certificate2 certificate)
		{
			throw new PlatformNotSupportedException();
		}

		public Pkcs12KeyBag AddKeyUnencrypted(AsymmetricAlgorithm key)
		{
			throw new PlatformNotSupportedException();
		}

		public Pkcs12SafeContentsBag AddNestedContents(Pkcs12SafeContents safeContents)
		{
			throw new PlatformNotSupportedException();
		}

		public void AddSafeBag(Pkcs12SafeBag safeBag)
		{
			throw new PlatformNotSupportedException();
		}

		public Pkcs12SecretBag AddSecret(Oid secretType, ReadOnlyMemory<byte> secretValue)
		{
			throw new PlatformNotSupportedException();
		}

		public Pkcs12ShroudedKeyBag AddShroudedKey(AsymmetricAlgorithm key, byte[] passwordBytes, PbeParameters pbeParameters)
		{
			throw new PlatformNotSupportedException();
		}

		public Pkcs12ShroudedKeyBag AddShroudedKey(AsymmetricAlgorithm key, ReadOnlySpan<byte> passwordBytes, PbeParameters pbeParameters)
		{
			throw new PlatformNotSupportedException();
		}

		public Pkcs12ShroudedKeyBag AddShroudedKey(AsymmetricAlgorithm key, ReadOnlySpan<char> password, PbeParameters pbeParameters)
		{
			throw new PlatformNotSupportedException();
		}

		public Pkcs12ShroudedKeyBag AddShroudedKey(AsymmetricAlgorithm key, string password, PbeParameters pbeParameters)
		{
			throw new PlatformNotSupportedException();
		}

		public void Decrypt(byte[] passwordBytes)
		{
			throw new PlatformNotSupportedException();
		}

		public void Decrypt(ReadOnlySpan<byte> passwordBytes)
		{
			throw new PlatformNotSupportedException();
		}

		public void Decrypt(ReadOnlySpan<char> password)
		{
			throw new PlatformNotSupportedException();
		}

		public void Decrypt(string password)
		{
			throw new PlatformNotSupportedException();
		}

		public IEnumerable<Pkcs12SafeBag> GetBags()
		{
			throw new PlatformNotSupportedException();
		}
	}
}
